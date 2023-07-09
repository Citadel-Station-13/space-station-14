using Content.Server._Citadel.Contracts.Components;
using Content.Server._Citadel.Contracts.Prototypes;
using Content.Shared._Citadel.Contracts.BUI;
using JetBrains.Annotations;
using Robust.Shared.Console;
using Robust.Shared.Utility;

namespace Content.Server._Citadel.Contracts;

[ByRefEvent]
public record struct ContractTryStatusChange(ContractStatus Old, ContractStatus New)
{
    public readonly ContractStatus Old = Old; // C# syntax moment, this is assigning the constructor values to the fields.
    public readonly ContractStatus New = New;
    public bool Cancelled = false;
    public readonly FormattedMessage FailMessage = new(); //TODO(lunar): This should use IError in the future.
}

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

[ByRefEvent]
public record struct ContractGetDisplayInfo(ContractDisplayData? Info = null);

/// <summary>
/// An event fired when a criteria group being fulfilled breaches the contract.
/// </summary>
///
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
