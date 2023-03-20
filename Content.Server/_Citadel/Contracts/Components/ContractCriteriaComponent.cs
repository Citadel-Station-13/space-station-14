using Content.Server._Citadel.Contracts.Systems;

namespace Content.Server._Citadel.Contracts.Components;

/// <summary>
/// This is used for a contract criteria.
/// </summary>
[RegisterComponent, Access(typeof(ContractCriteriaSystem))]
public sealed class ContractCriteriaComponent : Component
{
    [DataField("owningContract")]
    public EntityUid OwningContract;

    /// <summary>
    /// Whether or not the criteria has been satisfied.
    /// </summary>
    /// <remarks>This should be updated by other components on the contract that do the heavy lifting.</remarks>
    [DataField("satisfied")]
    public bool Satisfied = false;
}

public readonly record struct CriteriaSetupEvent;
public readonly record struct CriteriaStartTickingEvent;
public readonly record struct CriteriaUpdatedEvent;
