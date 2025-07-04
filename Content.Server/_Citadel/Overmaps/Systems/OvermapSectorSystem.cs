using Content.Server._Citadel.Overmaps.Components;
using Content.Shared._Citadel.Overmaps.Systems;

namespace Content.Server._Citadel.Overmaps.Systems;

/// <inheritdoc/>
public sealed class OvermapSectorSystem : SharedOvermapSectorSystem
{
    private EntityQuery<OvermapSectorComponent> _sectorQuery;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        _sectorQuery = GetEntityQuery<OvermapSectorComponent>();
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
        var mergeable = new List<EntityUid>();
        foreach(var entity in FindSectorsNearEntity(overmapEntity, predictedInfluenceRadius + 1))
        {
            // TODO: fill in
        }
        return mergeable;
    }

    /// <summary>
    /// Finds potential sectors near an entity. Internal use only.
    /// </summary>
    /// <param name="ent"></param>
    /// <param name="meters">Distance away from the entity's center that a sector must touch to be included.</param>
    /// <returns></returns>
    private IEnumerable<Entity<OvermapSectorComponent>> FindSectorsNearEntity(EntityUid ent, float meters)
    {


    }
}
