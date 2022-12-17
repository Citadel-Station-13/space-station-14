using System.Linq;
using Content.Server._00OuterRim.Worldgen2.Components;
using Content.Server.Ghost.Components;
using Content.Server.Mind.Components;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Server._00OuterRim.Worldgen2.Systems;

/// <summary>
/// This handles putting together chunk entities and notifying them about important changes.
/// </summary>
public sealed class WorldControllerSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly ILogManager _logManager = default!;

    private ISawmill _sawmill = default!;
    /// <inheritdoc/>
    public override void Initialize()
    {
        _sawmill = _logManager.GetSawmill("world");
        SubscribeLocalEvent<WorldChunkComponent, ComponentStartup>(OnChunkStartup);
        SubscribeLocalEvent<LoadedChunkComponent, ComponentStartup>(OnChunkLoadedCore);
        SubscribeLocalEvent<LoadedChunkComponent, ComponentShutdown>(OnChunkUnloadedCore);
    }

    /// <summary>
    /// Handles the inner logic of loading a chunk, i.e. events.
    /// </summary>
    private void OnChunkLoadedCore(EntityUid uid, LoadedChunkComponent component, ComponentStartup args)
    {
        var xform = Transform(uid);
        if (xform.MapUid is null)
            return;
        var coords = WorldGen.WorldToChunkCoords(xform.Coordinates.ToVector2i(EntityManager, _mapManager));
        var ev = new WorldChunkLoadedEvent(uid, coords);
        RaiseLocalEvent(xform.MapUid.Value, ref ev);
        RaiseLocalEvent(uid, ref ev);
        //_sawmill.Debug($"Loaded chunk {ToPrettyString(uid)} at {coords}");
    }

    /// <summary>
    /// Handles the inner logic of unloading a chunk, i.e. events.
    /// </summary>
    private void OnChunkUnloadedCore(EntityUid uid, LoadedChunkComponent component, ComponentShutdown args)
    {
        var xform = Transform(uid);
        if (xform.MapUid is null)
            return;
        var coords = WorldGen.WorldToChunkCoords(xform.Coordinates.ToVector2i(EntityManager, _mapManager));
        var ev = new WorldChunkUnloadedEvent(uid, coords);
        RaiseLocalEvent(xform.MapUid.Value, ref ev);
        RaiseLocalEvent(uid, ref ev);
        //_sawmill.Debug($"Unloaded chunk {ToPrettyString(uid)} at {coords}");
    }

    /// <summary>
    /// During setup, make sure the chunk gets attached to the WorldControllerComponent on the map.
    /// </summary>
    private void OnChunkStartup(EntityUid uid, WorldChunkComponent component, ComponentStartup args)
    {
        var xform = Transform(uid);
        var parent = xform.Coordinates.EntityId;

        if (!TryComp<WorldControllerComponent>(parent, out var worldController))
        {
            Logger.Error($"Badly initialized chunk {ToPrettyString(uid)}.");
            return;
        }

        var coords = WorldGen.WorldToChunkCoords(xform.Coordinates.ToVector2i(EntityManager, _mapManager));
        ref var chunks = ref worldController.Chunks;

        if (chunks.ContainsKey(coords))
        {
            Logger.Error($"Tried to initialize chunk {ToPrettyString(uid)} on top of existing chunk {ToPrettyString(chunks[coords])}.");
            return;
        }

        chunks[coords] = uid; // Add this entity to chunk index.

        var ev = new WorldChunkAddedEvent(uid, coords);
        RaiseLocalEvent(parent, ref ev);
    }

    private const int PlayerLoadRadius = 1;

    //TODO: This should be time-shared by an MC.
    /// <summary>
    /// Handles various tasks core to world generation, like chunk loading.
    /// </summary>
    /// <param name="frameTime"></param>
    public override void Update(float frameTime)
    {
        //TODO: Use struct enumerator for all entity queries here once available.
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

            var wc = xform.Coordinates.ToVector2i(EntityManager, _mapManager);
            var coords = WorldGen.WorldToChunkCoords(wc);
            var chunks = new GridPointsNearEnumerator(coords, (int)Math.Ceiling(worldLoader.Radius / (float)WorldGen.ChunkSize));

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

            var wc = xform.Coordinates.ToVector2i(EntityManager, _mapManager);
            var coords = WorldGen.WorldToChunkCoords(wc);
            var chunks = new GridPointsNearEnumerator(coords, PlayerLoadRadius);

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

        var loadedEnum = EntityQueryEnumerator<LoadedChunkComponent, TransformComponent>();
        // Make sure these chunks get unloaded at the end of the tick.
        while (loadedEnum.MoveNext(out var _, out var xform))
        {
            var mapOrNull = xform.MapUid;
            if (mapOrNull is null)
            {
                RemCompDeferred<LoadedChunkComponent>(xform.Owner);
                continue;
            }

            var map = mapOrNull.Value;
            if (!chunksToLoad.ContainsKey(map))
            {
                RemCompDeferred<LoadedChunkComponent>(xform.Owner);
                continue;
            }

            var coords = WorldGen.WorldToChunkCoords(xform.Coordinates.ToVector2i(EntityManager, _mapManager));

            if (!chunksToLoad[map].ContainsKey(coords))
                RemCompDeferred<LoadedChunkComponent>(xform.Owner);
        }

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
            _sawmill.Info($"Loaded {count} chunks in {timeSpan.TotalMilliseconds:N2}ms.");
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
        var coords = new EntityCoordinates(map, WorldGen.ChunkToWorldCoords(chunkCoords));
        var chunk =  Spawn("ORChunk", coords);
        var md = MetaData(chunk);
        md.EntityName = $"S-{chunkCoords.X}/{chunkCoords.Y}";
        return chunk;
    }
}

[ByRefEvent]
public readonly record struct WorldChunkAddedEvent(EntityUid Chunk, Vector2i Coords);

[ByRefEvent]
public readonly record struct WorldChunkLoadedEvent(EntityUid Chunk, Vector2i Coords);

[ByRefEvent]
public readonly record struct WorldChunkUnloadedEvent(EntityUid Chunk, Vector2i Coords);
