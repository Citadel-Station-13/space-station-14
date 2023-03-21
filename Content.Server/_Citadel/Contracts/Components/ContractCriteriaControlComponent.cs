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
    [DataField("criteria", required: true, customTypeSerializer: typeof(PrototypeIdDictionarySerializer<List<string>, CriteriaGroupPrototype>))]
    public Dictionary<string, List<string>> CriteriaPrototypes = default!;

    [DataField("criteriaEntities")]
    public Dictionary<string, List<EntityUid>> Criteria = new();

    [DataField("fulfilledCriteria")]
    public HashSet<string> FulfilledCriteriaGroups = new();
}
