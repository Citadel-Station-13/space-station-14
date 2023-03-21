using Content.Server._Citadel.Contracts.Components;
using Robust.Shared.Console;

namespace Content.Server._Citadel.Contracts;

/// <summary>
/// A directed event fired upon change in contract status.
/// </summary>
public readonly record struct ContractStatusChangedEvent(ContractStatus Old, ContractStatus New);

public readonly record struct CriteriaSetupEvent;
public readonly record struct CriteriaStartTickingEvent;
public readonly record struct CriteriaUpdatedEvent;

[ByRefEvent]
public record struct CriteriaGetDisplayInfo(CriteriaDisplayData? Info = null);

public record struct CriteriaDisplayData(string Description);

[ImplicitDataDefinitionForInheritors]
public abstract record CriteriaGroupEffectEvent
{
    public EntityUid Contract;

    protected CriteriaGroupEffectEvent(EntityUid contract)
    {
        Contract = contract;
    }

    protected CriteriaGroupEffectEvent()
    {
        Contract = EntityUid.Invalid;
    }
}

public sealed record CriteriaGroupBreachContract : CriteriaGroupEffectEvent;

public sealed record CriteriaGroupFinalizeContract : CriteriaGroupEffectEvent;

public readonly record struct ListContractsConsoleCommandEvent(IConsoleShell Shell);
