using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Server._Citadel.Overmaps.Components;
using Content.Shared._Citadel.Overmaps.Systems;
using Robust.Shared.Map.Components;
using Robust.Shared.Utility;

namespace Content.Server._Citadel.Overmaps.Systems;

/// <inheritdoc/>
public sealed class OvermapBindingsSystem : SharedOvermapBindingsSystem
{
    private EntityQuery<OvermapBindingsComponent> _overmapBindingsQuery;

    /// <summary>
    /// Dictionary of map entity to overmap binding entity
    /// </summary>
    private readonly Dictionary<EntityUid, EntityUid> _sectorLookup = new();

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        _overmapBindingsQuery = GetEntityQuery<OvermapBindingsComponent>();

        SubscribeLocalEvent<OvermapBindingsComponent, ComponentInit>(OnBindingsComponentInit);
        SubscribeLocalEvent<OvermapBindingsComponent, ComponentRemove>(OnBindingsComponentRemove);
        SubscribeLocalEvent<MapComponent, ComponentRemove>(OnMapComponentRemove);
    }

    public void OnBindingsComponentInit(Entity<OvermapBindingsComponent> entity, ref ComponentInit args)
    {
        foreach (var map in entity.Comp.BoundMapEntities)
        {
            _sectorLookup[map] = entity.Owner;
        }
    }

    public void OnBindingsComponentRemove(Entity<OvermapBindingsComponent> entity, ref ComponentRemove args)
    {
        foreach (var map in entity.Comp.BoundMapEntities)
        {
            if (_sectorLookup[map] == entity.Owner)
            {
                _sectorLookup.Remove(map);
            }
        }
    }

    public void OnMapComponentRemove(Entity<MapComponent> entity, ref ComponentRemove args)
    {
        if (TryGetOvermapEntityByMap(entity, out var overmapEntity))
        {
            RemoveBoundMapId(overmapEntity.Value, entity);
        }
    }

    public void AddBoundMapId(EntityUid overmapEntity, EntityUid mapId)
    {
        // TODO: impl
    }

    public void RemoveBoundMapId(EntityUid overmapEntity, EntityUid mapId)
    {
        // TODO: impl
    }

    public bool TryGetOvermapEntityByMap([NotNullWhen(true)] EntityUid? mapId,
        [NotNullWhen(true)] out EntityUid? overmapEntityId)
    {
        if (mapId != null && _sectorLookup.TryGetValue(mapId.Value, out var map))
        {
            overmapEntityId = map;
            return true;
        }
        else
        {
            overmapEntityId = null;
            return false;
        }
    }

    public bool TryGetBoundMapRandomOrNull([NotNullWhen(true)] EntityUid? entity, out EntityUid? mapEntityOut)
    {
        if (_overmapBindingsQuery.TryGetComponent(entity, out var overmapBindings))
        {
            mapEntityOut = overmapBindings.BoundMapEntities.FirstOrNull();
            return true;
        }
        else
        {
            mapEntityOut = null;
            return false;
        }
    }

    public bool TryGetBoundMapAnyOrNull([NotNullWhen(true)] EntityUid? entity,
        out EntityUid? mapEntityOut)
    {
        if (_overmapBindingsQuery.TryGetComponent(entity, out var overmapBindings))
        {
            mapEntityOut = overmapBindings.BoundMapEntities.FirstOrNull();
            return true;
        }
        else
        {
            mapEntityOut = null;
            return false;
        }
    }

    /// <summary>
    /// Returns bound maps as enumerable.
    /// </summary>
    /// <param name="entity">Entity UID of the on-overmap entity with bindings</param>
    /// <param name="enumerableOut"></param>
    public bool TryGetBoundMapsEnumerable([NotNullWhen(true)] EntityUid? entity,
        [NotNullWhen(true)] out IEnumerable<EntityUid>? enumerableOut)
    {
        if (_overmapBindingsQuery.TryGetComponent(entity, out var overmapBindings))
        {
            enumerableOut = overmapBindings.BoundMapEntities.AsEnumerable();
            return true;
        }
        else
        {
            enumerableOut = null;
            return false;
        }
    }

    /// <summary>
    /// Returns bound maps in ascending order as enumerable.
    /// </summary>
    /// <param name="entity">Entity UID of the on-overmap entity with bindings</param>
    /// <param name="enumerableOut"></param>
    public bool TryGetBoundMapsEnumerableZAscending([NotNullWhen(true)] EntityUid? entity,
        [NotNullWhen(true)] out IEnumerable<EntityUid>? enumerableOut)
    {
        if (_overmapBindingsQuery.TryGetComponent(entity, out var overmapBindings))
        {
            enumerableOut = overmapBindings.BoundMapEntities.AsEnumerable();
            return true;
        }
        else
        {
            enumerableOut = null;
            return false;
        }
    }
}
