using System.Linq;
using Content.Server._Citadel.Contracts.Components;
using Content.Server._Citadel.Contracts.Prototypes;
using Content.Server.Administration;
using Content.Server.Mind.Components;
using Content.Shared._Citadel.Contracts;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server._Citadel.Contracts.Systems;

/// <summary>
/// This handles managing contracts and processing their completion state.
/// </summary>
public sealed partial class ContractManagementSystem : EntitySystem
{
    [Dependency] private readonly IConsoleHost _consoleHost = default!;
    [Dependency] private readonly IComponentFactory _componentFactory = default!;
    [Dependency] private readonly ILogManager _log = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    [Dependency] private readonly ContractCriteriaSystem _contractCriteria = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {

        SubscribeLocalEvent<CriteriaGroupFinalizeContract>(OnCriteriaGroupFinalizeContract);
        SubscribeLocalEvent<CriteriaGroupBreachContract>(OnCriteriaGroupBreachContract);
        SubscribeLocalEvent<ContractComponent, ComponentShutdown>(OnContractshutdown);
    }

    private void OnContractshutdown(EntityUid uid, ContractComponent component, ComponentShutdown args)
    {
        if (component.OwningContractor is { } owner)
            owner.Contracts.Remove(uid);

        foreach (var contractor in component.SubContractors)
        {
            contractor.Contracts.Remove(uid);
        }
    }

    private void OnCriteriaGroupBreachContract(CriteriaGroupBreachContract ev)
    {
        TryBreachContract(ev.Contract);
    }

    private void OnCriteriaGroupFinalizeContract(CriteriaGroupFinalizeContract ev)
    {
        TryFinalizeContract(ev.Contract);
    }

    public EntityUid CreateUnboundContract(string contractProto)
    {
        var contractEnt = Spawn(contractProto, MapCoordinates.Nullspace);
        DebugTools.Assert(HasComp<ContractComponent>(contractEnt), $"The contract {contractProto} is missing required components!");

        var contract = Comp<ContractComponent>(contractEnt);
        contract.Status = ContractStatus.Initiating;

        RaiseLocalEvent(contractEnt, new ContractStatusChangedEvent(ContractStatus.Uninitialized, ContractStatus.Initiating));

        return contractEnt;
    }

    public void BindContract(EntityUid contractEnt, Mind.Mind contractor)
    {
        var contract = Comp<ContractComponent>(contractEnt);
        if (contract.OwningContractor is null)
        {
            contract.OwningContractor = contractor;
        }
        else
        {
            contract.SubContractors.Add(contractor);
        }

        contractor.Contracts.Add(contractEnt);
    }

    public EntityUid CreateBoundContract(string contractProto, Mind.Mind owner)
    {
        var contractEnt = CreateUnboundContract(contractProto);
        BindContract(contractEnt, owner);

        return contractEnt;
    }

    private bool TryChangeContractState(EntityUid contractUid, ContractComponent contractComponent,
        ContractStatus newStatus)
    {
        var oldStatus = contractComponent.Status;
        var ev = new ContractTryStatusChange(oldStatus, newStatus);
        RaiseLocalEvent(contractUid, ref ev);

        if (ev.Cancelled)
            return false;

        contractComponent.Status = newStatus;
        RaiseLocalEvent(contractUid, new ContractStatusChangedEvent(oldStatus, newStatus));

        return true;
    }

    public bool TryActivateContract(EntityUid contractUid, ContractComponent? contractComponent = null)
    {
        if (!Resolve(contractUid, ref contractComponent))
            return false;

        if (contractComponent.Status != ContractStatus.Initiating)
            return false;

        return TryChangeContractState(contractUid, contractComponent, ContractStatus.Active);
    }

    public bool TryFinalizeContract(EntityUid contractUid, ContractComponent? contractComponent = null)
    {
        if (!Resolve(contractUid, ref contractComponent))
            return false;

        if (contractComponent.Status != ContractStatus.Active)
            return false;

        return TryChangeContractState(contractUid, contractComponent, ContractStatus.Finalized);
    }

    public bool TryBreachContract(EntityUid contractUid, ContractComponent? contractComponent = null)
    {
        if (!Resolve(contractUid, ref contractComponent))
            return false;

        if (contractComponent.Status != ContractStatus.Active)
            return false;

        return TryChangeContractState(contractUid, contractComponent, ContractStatus.Breached);
    }

    public bool TryCancelContract(EntityUid contractUid, ContractComponent? contractComponent = null)
    {
        if (!Resolve(contractUid, ref contractComponent))
            return false;

        if (contractComponent.Status is not ContractStatus.Initiating and not ContractStatus.Active)
            return false;

        return TryChangeContractState(contractUid, contractComponent, ContractStatus.Cancelled);
    }
}
