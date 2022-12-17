using Content.Server._00Citadel.Worldgen.Tools;
using Content.Shared.Storage;

namespace Content.Server._00Citadel.Worldgen.Components.Debris;

/// <summary>
/// This is used for a very simple debris selection for simple biomes. Just uses a spawn table.
/// </summary>
[RegisterComponent]
public sealed class SimpleDebrisSelectorComponent : Component
{
    [DataField("debrisTable", required: true)]
    private List<EntitySpawnEntry> _entries = default!;

    private EntitySpawnCollectionCache? _cache;

    public EntitySpawnCollectionCache CachedDebrisTable
    {
        get
        {
            _cache ??= new EntitySpawnCollectionCache(_entries);
            return _cache;
        }
    }
}
