using Content.Server._Citadel.Worldgen.Components.Debris;
using Content.Shared.Maps;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Random;

namespace Content.Server._Citadel.Worldgen.Systems.Debris;

/// <summary>
/// This handles populating simple structures, simply using a loot table for each tile.
/// </summary>
public sealed class SimpleFloorPlanPopulatorSystem : BaseWorldSystem
{
    [Dependency] private readonly ITileDefinitionManager _tileDefinition = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<SimpleFloorPlanPopulatorComponent, LocalStructureLoadedEvent>(OnFloorPlanBuilt);
    }

    private void OnFloorPlanBuilt(EntityUid uid, SimpleFloorPlanPopulatorComponent component, LocalStructureLoadedEvent args)
    {
        var placables = new List<string?>(4);
        var grid = Comp<MapGridComponent>(uid);
        foreach (var tile in grid.GetAllTiles())
        {
            var coords = grid.GridTileToLocal(tile.GridIndices);
            var selector = tile.Tile.GetContentTileDefinition(_tileDefinition).ID;
            if (!component.Caches.TryGetValue(selector, out var cache))
                continue;

            placables.Clear();
            cache.GetSpawns(_random, ref placables);

            foreach (var proto in placables)
            {
                if (proto is null)
                    continue;

                Spawn(proto, coords);
            }
        }
    }
}
