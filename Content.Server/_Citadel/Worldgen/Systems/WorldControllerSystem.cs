using System.Linq;
using Content.Server._Citadel.Worldgen.Components;
using Content.Server.Ghost.Components;
using Content.Server.Mind.Components;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Server._Citadel.Worldgen.Systems;

/// <summary>
/// This handles putting together chunk entities and notifying them about important changes.
/// </summary>
public sealed class WorldControllerSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly ILogManager _logManager = default!;

    private ISawmill _sawmill = default!;
    /// <inheritdoc/>
    public override void Initialize()
    {
        _sawmill = _logManager.GetSawmill("world");
        SubscribeLocalEvent<LoadedChunkComponent, ComponentStartup>(OnChunkLoadedCore);
        SubscribeLocalEvent<LoadedChunkComponent, ComponentShutdown>(OnChunkUnloadedCore);
    }

    /// <summary>
    /// Handles the inner logic of loading a chunk, i.e. events.
    /// </summary>
    private void OnChunkLoadedCore(EntityUid uid, LoadedChunkComponent component, ComponentStartup args)
    {
        if (!TryComp<WorldChunkComponent>(uid, out var chunk))
            return;
        var ev = new WorldChunkLoadedEvent(uid, chunk.Coordinates);
        RaiseLocalEvent(chunk.Map, ref ev);
        RaiseLocalEvent(uid, ref ev);
        //_sawmill.Debug($"Loaded chunk {ToPrettyString(uid)} at {chunk.Coordinates}");
    }

    /// <summary>
    /// Handles the inner logic of unloading a chunk, i.e. events.
    /// </summary>
    private void OnChunkUnloadedCore(EntityUid uid, LoadedChunkComponent component, ComponentShutdown args)
    {
        if (!TryComp<WorldChunkComponent>(uid, out var chunk))
            return;
        var ev = new WorldChunkUnloadedEvent(uid, chunk.Coordinates);
        RaiseLocalEvent(chunk.Map, ref ev);
        RaiseLocalEvent(uid, ref ev);
        //_sawmill.Debug($"Unloaded chunk {ToPrettyString(uid)} at {coords}");
    }

    private const int PlayerLoadRadius = 2;

    //TODO: This should be time-shared by an MC.
    /// <summary>
    /// Handles various tasks core to world generation, like chunk loading.
    /// </summary>
    /// <param name="frameTime"></param>
    public override void Update(float frameTime)
    {
        //TODO: Maybe don't allocate a big collection every frame?
        var chunksToLoad = new Dictionary<EntityUid, Dictionary<Vector2i, List<EntityUid>>>();

        foreach (var controller in EntityQuery<WorldControllerComponent>())
        {
            chunksToLoad[controller.Owner] = new();

            List<Vector2i>? chunksToRemove = null;
            foreach (var (idx, chunk) in controller.Chunks)
            {
                if (!Deleted(chunk) && !Terminating(chunk))
                    continue;

                chunksToRemove ??= new(8);
                chunksToRemove.Add(idx);
            }

            if (chunksToRemove is not null)
            {
                foreach (var chunk in chunksToRemove)
                {
                    controller.Chunks.Remove(chunk);
                }
            }
        }

        var loaderEnum = EntityQueryEnumerator<WorldLoaderComponent, TransformComponent>();

        while (loaderEnum.MoveNext(out var worldLoader, out var xform))
        {
            var mapOrNull = xform.MapUid;
            if (mapOrNull is null)
                continue;
            var map = mapOrNull.Value;
            if (!chunksToLoad.ContainsKey(map))
                continue;

            var wc = xform.WorldPosition;
            var coords = WorldGen.WorldToChunkCoords(wc);
            var chunks = new GridPointsNearEnumerator(coords.Floored(), (int)Math.Ceiling(worldLoader.Radius / (float)WorldGen.ChunkSize));

            var set = chunksToLoad[map];

            while (chunks.MoveNext(out var chunk))
            {
                if (!set.TryGetValue(chunk.Value, out _))
                {
                    set[chunk.Value] = new(4);
                }
                set[chunk.Value].Add(worldLoader.Owner);
            }
        }

        var mindEnum = EntityQueryEnumerator<MindComponent, TransformComponent>();
        var ghostQuery = GetEntityQuery<GhostComponent>();

        // Mindful entities get special privilege as they're always a player and we don't want the illusion being broken around them.
        while (mindEnum.MoveNext(out var mind, out var xform))
        {
            if (!mind.HasMind)
                continue;
            if (ghostQuery.HasComponent(mind.Owner))
                continue;
            var mapOrNull = xform.MapUid;
            if (mapOrNull is null)
                continue;
            var map = mapOrNull.Value;
            if (!chunksToLoad.ContainsKey(map))
                continue;

            var wc = xform.WorldPosition;
            var coords = WorldGen.WorldToChunkCoords(wc);
            var chunks = new GridPointsNearEnumerator(coords.Floored(), PlayerLoadRadius);

            var set = chunksToLoad[map];

            while (chunks.MoveNext(out var chunk))
            {
                if (!set.TryGetValue(chunk.Value, out _))
                {
                    set[chunk.Value] = new(4);
                }
                set[chunk.Value].Add(mind.Owner);
            }
        }

        var loadedEnum = EntityQueryEnumerator<LoadedChunkComponent, WorldChunkComponent>();
        var chunksUnloaded = 0;

        // Make sure these chunks get unloaded at the end of the tick.
        while (loadedEnum.MoveNext(out var _, out var chunk))
        {
            var coords = chunk.Coordinates;

            if (!chunksToLoad[chunk.Map].ContainsKey(coords))
            {
                RemCompDeferred<LoadedChunkComponent>(chunk.Owner);
                chunksUnloaded++;
            }
        }

        if (chunksUnloaded > 0)
            _sawmill.Debug($"Queued {chunksUnloaded} chunks for unload.");

        if (chunksToLoad.All(x => x.Value.Count == 0))
            return;

        var startTime = _gameTiming.RealTime;
        var count = 0;
        var loadedQuery = GetEntityQuery<LoadedChunkComponent>();
        var controllerQuery = GetEntityQuery<WorldControllerComponent>();
        foreach (var (map, chunks) in chunksToLoad)
        {
            var controller = controllerQuery.GetComponent(map);
            foreach (var (chunk, loaders) in chunks)
            {
                var ent = GetOrCreateChunk(chunk, map, controller); // Ensure everything loads.
                LoadedChunkComponent? c = null;
                if (ent is not null && !loadedQuery.TryGetComponent(ent.Value, out c))
                {
                    c = AddComp<LoadedChunkComponent>(ent.Value);
                    count += 1;
                }

                if (c is not null)
                    c.Loaders = loaders;
            }
        }

        if (count > 0)
        {
            var timeSpan = _gameTiming.RealTime - startTime;
            _sawmill.Debug($"Loaded {count} chunks in {timeSpan.TotalMilliseconds:N2}ms.");
        }
    }

    public EntityUid? GetOrCreateChunk(Vector2i chunk, EntityUid map, WorldControllerComponent? controller = null)
    {
        if (!Resolve(map, ref controller))
        {
            throw new Exception($"Tried to use {ToPrettyString(map)} as a world map, without actually being one.");
        }

        if (controller.Chunks.TryGetValue(chunk, out var ent))
        {
            return ent;
        }
        else
        {
            return CreateChunkEntity(chunk, map);
        }
    }

    private EntityUid CreateChunkEntity(Vector2i chunkCoords, EntityUid map)
    {
        //var coords = new EntityCoordinates(map, WorldGen.ChunkToWorldCoordsCentered(chunkCoords));
        var coords = MapCoordinates.Nullspace;
        var chunk =  Spawn("CitadelChunk", coords);
        StartupChunkEntity(chunk, chunkCoords, map);
        var md = MetaData(chunk);
        md.EntityName = $"S-{chunkCoords.X}/{chunkCoords.Y}";
        return chunk;
    }

    private void StartupChunkEntity(EntityUid chunk, Vector2i coords, EntityUid map)
    {
        if (!TryComp<WorldControllerComponent>(map, out var worldController))
        {
            _sawmill.Error($"Badly initialized chunk {ToPrettyString(chunk)}.");
            return;
        }

        if (!TryComp<WorldChunkComponent>(chunk, out var chunkComponent))
        {
            _sawmill.Error($"Chunk {chunk} is missing WorldChunkComponent.");
            return;
        }

        ref var chunks = ref worldController.Chunks;

        chunks[coords] = chunk; // Add this entity to chunk index.
        chunkComponent.Coordinates = coords;
        chunkComponent.Map = map;
        var ev = new WorldChunkAddedEvent(chunk, coords);
        RaiseLocalEvent(map, ref ev);
    }
}

[ByRefEvent]
public readonly record struct WorldChunkAddedEvent(EntityUid Chunk, Vector2i Coords);

[ByRefEvent]
public readonly record struct WorldChunkLoadedEvent(EntityUid Chunk, Vector2i Coords);

[ByRefEvent]
public readonly record struct WorldChunkUnloadedEvent(EntityUid Chunk, Vector2i Coords);
