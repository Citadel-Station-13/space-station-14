using Content.Server._Citadel.Contracts.Systems;
using Content.Shared._Citadel.Contracts;

namespace Content.Server._Citadel.Contracts.Components;

/// <summary>
/// This is used for tracking core information about a contract, namely it's status.
/// </summary>
[RegisterComponent, Access(typeof(ContractManagementSystem))]
public sealed partial class ContractComponent : Component
{
    public Guid Uuid = Guid.NewGuid();
    /// <summary>
    /// The contract's status, indicating whether it's completed or not among other things.
    /// </summary>
    [DataField("status")]
    public ContractStatus Status;
    /// <summary>
    /// The contractor that owns this contract.
    /// </summary>
    [ViewVariables]
    public Mind.Mind? OwningContractor = default;
    /// <summary>
    /// All subcontractors for the contract, who signed on or were invited by the owning contractor.
    /// </summary>
    [ViewVariables]
    public List<Mind.Mind> SubContractors = new();
}
