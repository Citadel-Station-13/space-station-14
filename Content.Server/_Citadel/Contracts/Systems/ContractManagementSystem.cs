using Content.Server._Citadel.Contracts.Components;
using Content.Shared._Citadel.Contracts;
using Robust.Shared.Map;
using Robust.Shared.Utility;

namespace Content.Server._Citadel.Contracts.Systems;

/// <summary>
/// This handles managing contracts and processing their completion state.
/// </summary>
public sealed class ContractManagementSystem : EntitySystem
{
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

        RaiseLocalEvent(contractEnt, new ContractStatusChangedEvent(contractEnt, ContractStatus.Uninitialized, ContractStatus.Initiating), broadcast: true);

        return contractEnt;
    }

    public void BindContract(EntityUid contractEnt, Mind.Mind contractor)
    {
        var contract = Comp<ContractComponent>(contractEnt);
        if (contract.OwningContractor is null)
        {
            contract.OwningContractor = contractor;
            if (contract.AutoStart && contract.Status is ContractStatus.Initiating)
            {
                if (!TryChangeContractState(contractEnt, contract, ContractStatus.Active))
                    TryChangeContractState(contractEnt, contract, ContractStatus.Cancelled);
            }
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

    public bool CouldChangeStatusTo(EntityUid contractUid, ContractStatus newStatus, out FormattedMessage? failMsg, ContractComponent? contractComponent = null)
    {
        failMsg = null;
        if (!Resolve(contractUid, ref contractComponent))
            return false;
        var ev = new ContractTryStatusChange(contractComponent.Status, newStatus);
        RaiseLocalEvent(contractUid, ref ev);

        if (ev.Cancelled)
            failMsg = ev.FailMessage;

        return !ev.Cancelled;
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
        RaiseLocalEvent(contractUid, new ContractStatusChangedEvent(contractUid, oldStatus, newStatus), broadcast: true);

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
