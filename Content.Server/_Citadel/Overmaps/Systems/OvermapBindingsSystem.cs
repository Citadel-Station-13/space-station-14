using System.Linq;
using Content.Shared._Citadel.Overmaps.Components;
using Content.Shared._Citadel.Overmaps.Systems;
using Robust.Shared.Map;

namespace Content.Server._Citadel.Overmaps.Systems;

/// <inheritdoc/>
public sealed class OvermapBindingsSystem : SharedOvermapBindingsSystem
{
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;

    private EntityQuery<OvermapBindingsComponent> _overmapBindingsQuery;

    private Dictionary<MapId, EntityUid> sectorLookup = new Dictionary<MapId, EntityUid>();

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        _overmapBindingsQuery = GetEntityQuery<OvermapBindingsComponent>();

        SubscribeLocalEvent<OvermapBindingsComponent, ComponentShutdown>();
    }

    public void OnBindingsComponentShutdown(Entity<OvermapBindingsComponent> entity, ref ComponentShutdown args)
    {

    }

    public void AddBoundMapId(EntityUid overmapEntity, MapId mapId)
    {

    }

    public void RemoveBoundMapId(EntityUid overmapEntity, MapId mapId)
    {

    }

    public EntityUid? TryGetOvermapEntityByMapId(MapId? mapId)
    {

    }

    public MapId? TryGetRandomBoundMapId(EntityUid? uid)
    {
        if (_overmapBindingsQuery.TryGetComponent(uid, out var overmapBindings))
        {
            return overmapBindings.MapIds.Count > 0 ? overmapBindings.MapIds[0] : null;
        }

        return null;
    }

    public MapId? TryGetFirstBoundMapId(EntityUid? uid)
    {
        if (_overmapBindingsQuery.TryGetComponent(uid, out var overmapBindings))
        {
            return overmapBindings.MapIds.Count > 0 ? overmapBindings.MapIds[0] : null;
        }

        return null;
    }

    /// <summary>
    /// Returns bound maps as enumerable.
    /// </summary>
    /// <param name="uid">Entity UID of the overmapped entity with bindings</param>
    public IEnumerable<MapId>? TryGetBoundMapIds (EntityUid? uid)
    {
        if (!_overmapBindingsQuery.TryGetComponent(uid, out var overmapBindings))
            return null;
        return overmapBindings.MapIds.AsEnumerable();
    }

    /// <summary>
    /// Returns bound maps in ascending order as enumerable.
    /// </summary>
    /// <param name="uid">Entity UID of the overmapped entity with bindings</param>
    public IEnumerable<MapId>? TryGetZAscendingBoundMapIds (EntityUid? uid)
    {
        if (!_overmapBindingsQuery.TryGetComponent(uid, out var overmapBindings))
            return null;
        return overmapBindings.MapIds.AsEnumerable();
    }
}
