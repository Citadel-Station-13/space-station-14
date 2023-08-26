namespace Content.Server._Citadel.Contracts.Components;

/// <summary>
/// This is used for contracts that are advanced by finalizing other contracts.
/// </summary>
[RegisterComponent]
public sealed partial class FinalizeContractsCriteriaComponent : Component
{
    [DataField("toClear")]
    public int ContractsToClear;

    public int ContractsCleared;
}
