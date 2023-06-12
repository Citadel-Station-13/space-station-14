using Content.Server._Citadel.Contracts.Prototypes;
using Content.Server._Citadel.Contracts.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Server._Citadel.Contracts.Components;

/// <summary>
/// This is used for tracking a contract's finalizing and breaching criteria.
/// </summary>
[RegisterComponent, Access(typeof(ContractCriteriaSystem))]
public sealed class ContractCriteriaControlComponent : Component
{
    /// <summary>
    /// The criteria groups and their associated criteria.
    /// </summary>
    [DataField("criteria", required: true, customTypeSerializer: typeof(PrototypeIdDictionarySerializer<List<string>, CriteriaGroupPrototype>))]
    public Dictionary<string, List<string>> CriteriaPrototypes = default!;

    /// <summary>
    /// Additional criteria effects, on top of the ones the groups already have.
    /// </summary>
    [DataField("criteriaEffects")]
    public Dictionary<string, List<CriteriaGroupEffectEvent>> CriteriaEffects = default!;

    /// <summary>
    /// The actual criteria entities.
    /// </summary>
    [DataField("criteriaEntities")]
    public Dictionary<string, List<EntityUid>> Criteria = new();

    /// <summary>
    /// The fulfilled criteria groups.
    /// </summary>
    [DataField("fulfilledCriteria")]
    public HashSet<string> FulfilledCriteriaGroups = new();
}
