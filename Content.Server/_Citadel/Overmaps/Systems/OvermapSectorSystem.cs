using System.Numerics;
using Content.Shared._Citadel.Overmaps.Components;
using Content.Shared._Citadel.Overmaps.Systems;
using Robust.Shared.Map;

namespace Content.Server._Citadel.Overmaps.Systems;

/// <inheritdoc/>
public sealed class OvermapSectorSystem : SharedOvermapSectorSystem
{
    [Dependency] private readonly EntityLookupSystem _entityLookupSystem = default!;
    [Dependency] private readonly SharedTransformSystem _transformSystem = default!;

    private EntityQuery<OvermapSectorComponent> _sectorQuery;
    private EntityQuery<TransformComponent> _transformQuery;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        _sectorQuery = GetEntityQuery<OvermapSectorComponent>();
        _transformQuery = GetEntityQuery<TransformComponent>();
    }

    /// <summary>
    /// Gets sectors an overmap entity in ephemeral travel should merge with.
    /// A sector is to be merged with if that entity would touch its influence range.
    /// </summary>
    /// <param name="overmapEntity"></param>
    /// <param name="predictedInfluenceRadius"></param>
    /// <returns>A list of sectors.</returns>
    public IEnumerable<EntityUid> GetMergeableSectors(EntityUid overmapEntity, float predictedInfluenceRadius)
    {
        if (!_sectorQuery.TryGetComponent(overmapEntity, out var sectorComponent))
        {
            return [];
        }

        if (!_transformQuery.HasComponent(overmapEntity))
        {
            return [];
        }

        var mergeable = new List<EntityUid>();
        var ourPosition = _transformSystem.GetWorldPosition(overmapEntity);
        foreach (var enemy in FindSectorsNearEntity(overmapEntity, predictedInfluenceRadius + 1))
        {
            var theirPosition = _transformSystem.GetWorldPosition(enemy);
            var betweenUs = theirPosition - ourPosition;
            var mergeDist = sectorComponent.influenceRange + enemy.Comp.influenceRange;
            if (betweenUs.Length() > mergeDist)
            {
                continue;
            }

            mergeable.Add(enemy.Owner);
        }

        return mergeable;
    }

    /// <summary>
    /// Finds potential sectors near an entity. Internal use only.
    /// It can be assumed returned entities have TransformComponents.
    /// </summary>
    /// <param name="ent"></param>
    /// <param name="meters">Distance away from the entity's center that a sector must touch to be included.</param>
    /// <returns></returns>
    private IEnumerable<Entity<OvermapSectorComponent>> FindSectorsNearEntity(EntityUid ent, float meters)
    {
        // query from center
        var useCoords = new EntityCoordinates(ent, new Vector2(0, 0));
        // TODO: this is kinda shitty but it's the max range we expect another sector's center to be, basically.
        //       this is because sectors do not have influence ranges that are necessarily the same as their
        //       physics hitboxes.
        const float lazyExpander = 50.0f;
        return _entityLookupSystem.GetEntitiesInRange<OvermapSectorComponent>(useCoords,
            meters + lazyExpander,
            LookupFlags.Uncontained);
    }
}
