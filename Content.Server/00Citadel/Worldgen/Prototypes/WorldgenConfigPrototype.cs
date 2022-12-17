using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

namespace Content.Server._00OuterRim.Worldgen2.Prototypes;

/// <summary>
/// This is a prototype for...
/// </summary>
[Prototype("worldgenConfig")]
public sealed class WorldgenConfigPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    ///     The components that get added to the target map.
    /// </summary>
    [DataField("components", required: true)]
    public EntityPrototype.ComponentRegistry Components { get; } = default!;

    //TODO: Get someone to make this a method on componentregistry that does it Correctly.
    /// <summary>
    /// Applies the worldgen config to the given target (presumably a map.)
    /// </summary>
    public void Apply(EntityUid target, ISerializationManager serialization, IEntityManager entityManager, IComponentFactory componentFactory)
    {
        // Add all components required by the prototype. Engine update for this whenst.
        foreach (var (name, data) in Components)
        {
            if (!componentFactory.TryGetRegistration(name, out var registration))
                continue;

            if (entityManager.HasComponent(target, registration.Type))
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
