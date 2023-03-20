using Content.Server._Citadel.Contracts.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Server._Citadel.Contracts.Components;

/// <summary>
/// This is used for tracking a contract's finalizing and breaching criteria.
/// </summary>
[RegisterComponent, Access(typeof(ContractCriteriaSystem))]
public sealed class ContractCriteriaControlComponent : Component
{
    [DataField("finalizingCriteria", required: true, customTypeSerializer: typeof(PrototypeIdListSerializer<EntityPrototype>))]
    [Access(typeof(ContractCriteriaSystem), Other = AccessPermissions.None)]
    public List<string> ContractFinalizingCriteriaPrototypes = default!;
    [DataField("breachingCriteria", required: true, customTypeSerializer: typeof(PrototypeIdListSerializer<EntityPrototype>))]
    [Access(typeof(ContractCriteriaSystem), Other = AccessPermissions.None)]
    public List<string> ContractBreachingCriteriaPrototypes = default!;

    [DataField("finalizingCriteriaEntities")]
    public List<EntityUid> FinalizingCriteria = new();
    [DataField("breachingCriteriaEntities")]
    public List<EntityUid> BreachingCriteria = new();
}
