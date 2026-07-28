using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Server._Citadel.Overmaps.Components;
using Content.Server._Citadel.Overmaps.Events;
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

    /// <summary>
    /// Adds a binding to an overmap entity.
    /// This will fail if the entity does not have an OvermapBindingsComponent.
    /// </summary>
    /// <param name="overmapEntity"></param>
    /// <param name="mapId"></param>
    /// <returns>'true' if the binding is on the overmap entity at the end of the call,
    /// including if it was already there.</returns>
    public bool AddBoundMapId(EntityUid overmapEntity, EntityUid mapId)
    {
        if (_overmapBindingsQuery.TryGetComponent(overmapEntity, out var comp))
        {
            if (comp.BoundMapEntities.Contains(mapId))
            {
                return true;
            }

            comp.BoundMapEntities.Add(mapId);
            var sig = new OvermapBindingAddedEvent(mapId);
            RaiseLocalEvent(overmapEntity, ref sig, true);
            _sectorLookup.Add(mapId, overmapEntity);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Ensures a binding is not on an overmap entity.
    /// </summary>
    /// <param name="overmapEntity"></param>
    /// <param name="mapId"></param>
    /// <returns>'true' if the binding is not on the overmap entity at the end of the call,
    /// including if it never existed.</returns>
    public bool RemoveBoundMapId(EntityUid overmapEntity, EntityUid mapId)
    {
        if (_overmapBindingsQuery.TryGetComponent(overmapEntity, out var comp))
        {
            if (!comp.BoundMapEntities.Contains(mapId))
            {
                return true;
            }

            comp.BoundMapEntities.Remove(mapId);
            var sig = new OvermapBindingRemovedEvent(mapId);
            _sectorLookup.Remove(mapId);
            RaiseLocalEvent(overmapEntity, ref sig, true);
        }

        return true;
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
            // TODO: this should be random
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
