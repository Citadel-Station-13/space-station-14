using Content.Shared._Citadel.Overmaps.Components;
using Content.Shared._Citadel.Overmaps.Systems;
using Robust.Shared.Map;

namespace Content.Server._Citadel.Overmaps.Systems;

/// <inheritdoc/>
public sealed class OvermapBindingsSystem : SharedOvermapBindingsSystem
{
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;

    private EntityQuery<OvermapBindingsComponent> _overmapBindingsQuery;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        _overmapBindingsQuery = GetEntityQuery<OvermapBindingsComponent>();
    }

    public void SetBoundMapId(EntityUid overmapEntity, MapId mapId)
    {

    }
}
