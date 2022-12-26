using System.Linq;
using Content.Server._Citadel.Worldgen.Components;
using Content.Server._Citadel.Worldgen.Components.Debris;
using Content.Server._Citadel.Worldgen.Systems.GC;
using Content.Server._Citadel.Worldgen.Tools;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Random;

namespace Content.Server._Citadel.Worldgen.Systems.Debris;

/// <summary>
/// This handles placing debris within the world evenly with rng, primarily for structures like asteroid fields.
/// </summary>
public sealed class DebrisFeaturePlacerSystem : BaseWorldSystem
{
    [Dependency] private readonly DeferredSpawnSystem _deferred = default!;
    [Dependency] private readonly GCQueueSystem _gc = default!;
    [Dependency] private readonly NoiseIndexSystem _noiseIndex = default!;
    [Dependency] private readonly PoissonDiskSampler _sampler = default!;
    [Dependency] private readonly ILogManager _logManager = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    private ISawmill _sawmill = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        _sawmill = _logManager.GetSawmill("world.debris.feature_placer");
        SubscribeLocalEvent<DebrisFeaturePlacerControllerComponent, WorldChunkLoadedEvent>(OnChunkLoaded);
        SubscribeLocalEvent<DebrisFeaturePlacerControllerComponent, WorldChunkUnloadedEvent>(OnChunkUnloaded);
        SubscribeLocalEvent<OwnedDebrisComponent, ComponentShutdown>(OnDebrisShutdown);
        SubscribeLocalEvent<OwnedDebrisComponent, MoveEvent>(OnDebrisMove);
        SubscribeLocalEvent<SimpleDebrisSelectorComponent, TryGetPlaceableDebrisFeatureEvent>(OnTryGetPlacableDebrisEvent);
        SubscribeLocalEvent<TieDebrisToFeaturePlacerEvent>(OnDeferredDone);
    }

    private void OnDebrisMove(EntityUid uid, OwnedDebrisComponent component, ref MoveEvent args)
    {
        if (!HasComp<WorldChunkComponent>(component.OwningController))
            return; // Redundant logic, prolly needs it's own handler for your custom system.

        var placer = Comp<DebrisFeaturePlacerControllerComponent>(component.OwningController);
        var xform = Transform(uid);
        var ownerXform = Transform(component.OwningController);
        if (xform.MapUid is null || ownerXform.MapUid is null)
            return; // not our problem

        if (xform.MapUid != ownerXform.MapUid)
        {
            _sawmill.Error($"Somehow debris {uid} left it's expected map! Unparenting it to avoid issues.");
            RemCompDeferred<OwnedDebrisComponent>(uid);
            placer.OwnedDebris.Remove(component.LastKey);
            return;
        }

        placer.OwnedDebris.Remove(component.LastKey);
        var newChunk = GetOrCreateChunk(GetChunkCoords(uid), xform.MapUid!.Value);
        if (newChunk is null || !TryComp<DebrisFeaturePlacerControllerComponent>(newChunk, out var newPlacer))
        {
            // Whelp.
            RemCompDeferred<OwnedDebrisComponent>(uid);
            return;
        }

        newPlacer.OwnedDebris[xform.WorldPosition] = uid; // Change our owner.
        component.OwningController = newChunk.Value;
    }

    private void OnDebrisShutdown(EntityUid uid, OwnedDebrisComponent component, ComponentShutdown args)
    {
        if (!TryComp<DebrisFeaturePlacerControllerComponent>(component.OwningController, out var placer))
            return;
        placer.OwnedDebris[component.LastKey] = null;
    }

    private void OnChunkUnloaded(EntityUid uid, DebrisFeaturePlacerControllerComponent component, ref WorldChunkUnloadedEvent args)
    {
        foreach (var (_, debris) in component.OwnedDebris)
        {
            if (debris is not null)
                _gc.TryGCEntity(debris.Value); // gonb.
        }

        component.DoSpawns = true;
    }

    private void OnTryGetPlacableDebrisEvent(EntityUid uid, SimpleDebrisSelectorComponent component, ref TryGetPlaceableDebrisFeatureEvent args)
    {
        if (args.DebrisProto is not null)
            return;

        var l = new List<string?>(1);
        component.CachedDebrisTable.GetSpawns(_random, ref l);

        switch (l.Count)
        {
            case 0:
                return;
            case > 1:
                _sawmill.Warning($"Got more than one possible debris type from {uid}. List: {string.Join(", ", l)}");
                break;
        }

        args.DebrisProto = l[0];
    }

    private void OnDeferredDone(TieDebrisToFeaturePlacerEvent ev)
    {
        var placer = Comp<DebrisFeaturePlacerControllerComponent>(ev.DebrisPlacer);
        placer.OwnedDebris.Add(ev.Pos, ev.SpawnedEntity);
        var owned = EnsureComp<OwnedDebrisComponent>(ev.SpawnedEntity);
        owned.OwningController = ev.DebrisPlacer;
        owned.LastKey = ev.Pos;

        var xform = Transform(ev.SpawnedEntity);
        var realchunk = GetOrCreateChunk(GetChunkCoords(ev.SpawnedEntity), xform.MapUid!.Value);
        if (realchunk != ev.DebrisPlacer)
        {
            var chunk = Comp<WorldChunkComponent>(realchunk!.Value);
            _sawmill.Error($"Debris thinks it's in chunk {GetChunkCoords(ev.DebrisPlacer)} when it's actually in {GetChunkCoords(realchunk!.Value)}. {ev.Pos} vs {xform.WorldPosition - WorldGen.ChunkToWorldCoords(chunk.Coordinates)}");
        }
    }

    private void OnChunkLoaded(EntityUid uid, DebrisFeaturePlacerControllerComponent component, ref WorldChunkLoadedEvent args)
    {
        if (component.DoSpawns == false)
            return;

        component.DoSpawns = false; // Don't repeat yourself if this crashes.

        var chunk = Comp<WorldChunkComponent>(args.Chunk);
        var densityChannel = component.DensityNoiseChannel;
        var density = _noiseIndex.Evaluate(uid, densityChannel, chunk.Coordinates + new Vector2(0.5f, 0.5f));
        if (density == 0)
            return;

        List<Vector2>? points = null;

        // If we've been loaded before, reuse the same coordinates.
        if (component.OwnedDebris.Count != 0)
        {
            //TODO: Remove LINQ.
            points = component.OwnedDebris
                .Where(x => !Deleted(x.Value))
                .Select(static x => x.Key)
                .ToList();
        }

        points ??= GeneratePointsInChunk(args.Chunk, density, chunk.Coordinates, chunk.Map);

        var safetyBounds = Box2.UnitCentered.Enlarged(component.SafetyZoneRadius);
        var failures = 0; // Avoid severe log spam.
        foreach (var point in points)
        {
            var pointDensity = _noiseIndex.Evaluate(uid, densityChannel, WorldGen.WorldToChunkCoords(point));
            if ((pointDensity == 0 && component.DensityClip) || _random.Prob(component.RandomCancellationChance))
                continue;

            var coords = new EntityCoordinates(chunk.Map, point);

            if (_mapManager.FindGridsIntersecting(Comp<MapComponent>(chunk.Map).WorldMap, safetyBounds.Translated(point), false).Any())
                continue; // Oops, gonna collide.

            var preEv = new PrePlaceDebrisFeatureEvent(coords, args.Chunk);
            RaiseLocalEvent(uid, ref preEv);
            if (uid != args.Chunk)
                RaiseLocalEvent(args.Chunk, ref preEv);

            if (preEv.Cancelled)
                continue;

            var debrisFeatureEv = new TryGetPlaceableDebrisFeatureEvent(coords, args.Chunk);
            RaiseLocalEvent(uid, ref debrisFeatureEv);

            if (debrisFeatureEv.DebrisProto == null)
            {
                // Try on the chunk...?
                if (uid != args.Chunk)
                    RaiseLocalEvent(args.Chunk, ref debrisFeatureEv);

                if (debrisFeatureEv.DebrisProto == null)
                {
                    // Nope.
                    failures++;
                    continue;
                }
            }

            _deferred.SpawnEntityDeferred(debrisFeatureEv.DebrisProto, coords, new TieDebrisToFeaturePlacerEvent(args.Chunk, point));
        }

        if (failures > 0)
            _sawmill.Error($"Failed to place {failures} debris at chunk {args.Chunk}");

    }

    private List<Vector2> GeneratePointsInChunk(EntityUid chunk, float density, Vector2 coords, EntityUid map)
    {
        var offs = (int)((WorldGen.ChunkSize - (WorldGen.ChunkSize / 8)) / 2);
        var topLeft = (-offs, -offs);
        var lowerRight = (offs, offs);
        var debrisPoints = _sampler.SampleRectangle(topLeft, lowerRight, density);

        var realCenter = WorldGen.ChunkToWorldCoordsCentered(coords.Floored());

        for (var i = 0; i < debrisPoints.Count; i++)
        {
            debrisPoints[i] = realCenter + debrisPoints[i];
        }

        return debrisPoints;
    }

    private sealed class TieDebrisToFeaturePlacerEvent : DeferredSpawnDoneEvent
    {
        public EntityUid DebrisPlacer;
        public Vector2 Pos;

        public TieDebrisToFeaturePlacerEvent(EntityUid debrisPlacer, Vector2 pos)
        {
            DebrisPlacer = debrisPlacer;
            Pos = pos;
        }
    }
}

/// <summary>
/// Fired on the debris feature placer controller and the chunk, ahead of placing a debris piece.
/// </summary>
[ByRefEvent]
public record struct PrePlaceDebrisFeatureEvent(EntityCoordinates Coords, EntityUid Chunk, bool Cancelled = false);

[ByRefEvent]
public record struct TryGetPlaceableDebrisFeatureEvent(EntityCoordinates Coords, EntityUid Chunk, string? DebrisProto = null);
