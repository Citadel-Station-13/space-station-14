using Content.Server._Citadel.Contracts.Components;
using JetBrains.Annotations;
using Robust.Shared.Console;

namespace Content.Server._Citadel.Contracts;

/// <summary>
/// An event fired upon change in contract status.
/// </summary>
/// <target>Contract</target>
/// <broadcast>false</broadcast>
public readonly record struct ContractStatusChangedEvent(ContractStatus Old, ContractStatus New);

/// <summary>
/// An event fired when a contract is ready for setup.
/// There may or may not be an owner assigned at that point.
/// </summary>
/// <target>Criteria</target>
/// <broadcast>false</broadcast>
public readonly record struct CriteriaSetupEvent;
/// <summary>
/// An event fired when a contract's criteria should start evaluating themselves.
/// </summary>
/// <target>Criteria</target>
/// <broadcast>false</broadcast>
public readonly record struct CriteriaStartTickingEvent;

/// <summary>
/// An event fired when any criteria on a contract has been updated.
/// </summary>
/// <target>Contract</target>
/// <broadcast>false</broadcast>
public readonly record struct CriteriaUpdatedEvent;

/// <summary>
/// An event fired when attempting to get information to use to present a criteria to the user.
/// </summary>
/// <target>Criteria</target>
/// <broadcast>false</broadcast>
[ByRefEvent]
public record struct CriteriaGetDisplayInfo(CriteriaDisplayData? Info = null);

public record struct CriteriaDisplayData(string Description);

/// <summary>
/// An event fired when a criteria group has been fulfilled.
/// </summary>
[ImplicitDataDefinitionForInheritors]
public abstract record CriteriaGroupEffectEvent
{
    public EntityUid Contract = EntityUid.Invalid;

    public virtual string? Describe() { return null; }
}

/// <summary>
/// An event fired when a criteria group being fulfilled breaches the contract.
/// </summary>
[UsedImplicitly]
public sealed record CriteriaGroupBreachContract : CriteriaGroupEffectEvent;

/// <summary>
/// An event fired when a criteria group being fulfilled finalizes the contract.
/// </summary>
[UsedImplicitly]
public sealed record CriteriaGroupFinalizeContract : CriteriaGroupEffectEvent;

/// <summary>
/// An event fired when listing a contract's details with the lscontracts command.
/// </summary>
public readonly record struct ListContractsConsoleCommandEvent(IConsoleShell Shell);
