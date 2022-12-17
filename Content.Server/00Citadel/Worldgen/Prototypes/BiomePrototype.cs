using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Server._00OuterRim.Worldgen2.Prototypes;

/// <summary>
/// This is a prototype for...
/// </summary>
[Prototype("biome2")]
public sealed class BiomePrototype : IPrototype, IInheritingPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    /// <inheritdoc/>
    [ParentDataField(typeof(AbstractPrototypeIdArraySerializer<EntityPrototype>))]
    public string[]? Parents { get; }

    /// <inheritdoc/>
    [NeverPushInheritance]
    [AbstractDataField]
    public bool Abstract { get; }

    /// <summary>
    /// Higher priority biomes get picked before lower priority ones.
    /// </summary>
    [DataField("priority", required: true)]
    public int Priority { get; }

    /// <summary>
    /// The valid ranges of noise values under which this biome can be picked.
    /// </summary>
    [DataField("noiseRanges", required: true)]
    public Dictionary<string, List<Vector2>> NoiseRanges = default!;

    /// <summary>
    ///     The components that get added to the target map.
    /// </summary>
    [DataField("chunkComponents")]
    [AlwaysPushInheritance]
    public EntityPrototype.ComponentRegistry ChunkComponents { get; } = new();

    //TODO: Get someone to make this a method on componentregistry that does it Correctly.
    /// <summary>
    /// Applies the worldgen config to the given target (presumably a map.)
    /// </summary>
    public void Apply(EntityUid target, ISerializationManager serialization, IEntityManager entityManager, IComponentFactory componentFactory)
    {
        // Add all components required by the prototype. Engine update for this whenst.
        foreach (var (name, data) in ChunkComponents)
        {
            if (!componentFactory.TryGetRegistration(name, out var registration))
                continue;

            if (componentFactory.GetComponent(registration.Type) is not Component component)
                continue;

            component.Owner = target;

            var temp = (object) component;
            serialization.Copy(data.Component, ref temp);
            entityManager.AddComponent(target, (Component)temp!);
        }
    }
}
