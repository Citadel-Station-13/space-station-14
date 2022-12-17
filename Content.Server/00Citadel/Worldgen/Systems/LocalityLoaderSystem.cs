using Content.Server._00OuterRim.Worldgen2.Components;

namespace Content.Server._00OuterRim.Worldgen2.Systems;

/// <summary>
/// This handles loading in objects based on distance from player, using some metadata on chunks.
/// </summary>
public sealed class LocalityLoaderSystem : BaseWorldSystem
{
    public override void Update(float frameTime)
    {
        var e = EntityQueryEnumerator<LocalityLoaderComponent, TransformComponent>();
        var loadedQuery = GetEntityQuery<LoadedChunkComponent>();
        var xformQuery = GetEntityQuery<TransformComponent>();

        while (e.MoveNext(out var loadable, out var xform))
        {
            var coords = GetChunkCoords(xform.Owner, xform);
            var chunk = GetOrCreateChunk(coords, xform.MapUid!.Value);
            if (!loadedQuery.TryGetComponent(chunk, out var loaded) || loaded.Loaders is null)
                continue;

            foreach (var loader in loaded.Loaders)
            {
                if (!xformQuery.TryGetComponent(loader, out var loaderXform))
                    continue;

                if ((loaderXform.WorldPosition - xform.WorldPosition).Length > loadable.LoadingDistance)
                    continue;

                RaiseLocalEvent(loadable.Owner, new LocalStructureLoadedEvent());
                break;
            }
        }
    }
}

public record struct LocalStructureLoadedEvent();
