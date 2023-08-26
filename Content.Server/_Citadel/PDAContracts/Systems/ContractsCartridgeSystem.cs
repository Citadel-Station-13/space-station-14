using System.Linq;
using Content.Server._Citadel.Contracts.Components;
using Content.Server._Citadel.Contracts.Systems;
using Content.Server._Citadel.PDAContracts.Components;
using Content.Server._Citadel.VesselContracts.Components;
using Content.Server.CartridgeLoader;
using Content.Server.Players;
using Content.Shared._Citadel.Contracts;
using Content.Shared._Citadel.Contracts.BUI;
using Content.Shared.CartridgeLoader;
using Content.Shared.FixedPoint;
using Robust.Server.Player;
using Robust.Shared.Utility;

namespace Content.Server._Citadel.PDAContracts.Systems;

/// <summary>
/// This handles the contracts cartridge and it's UI.
/// </summary>
public sealed class ContractsCartridgeSystem : EntitySystem
{
    [Dependency] private readonly CartridgeLoaderSystem? _cartridgeLoaderSystem = default!;
    [Dependency] private readonly ContractManagementSystem _contracts = default!;
    [Dependency] private readonly ContractCriteriaSystem _criteria = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ContractsCartridgeComponent, CartridgeUiReadyEvent>(OnUiReady);
        SubscribeLocalEvent<ContractsCartridgeComponent, CartridgeMessageEvent>(OnMessage);
    }

    private void OnMessage(EntityUid uid, ContractsCartridgeComponent component, CartridgeMessageEvent args)
    {
        if (args is not ContractsUiMessageEvent { } ev)
            return;

        var player = (IPlayerSession) args.Session!;
        var contract = EntityQuery<ContractComponent>().First(x => x.Uuid == ev.Contract);
        switch (ev.Action)
        {
            case ContractsUiMessageEvent.ContractAction.Sign:
                if (contract.OwningContractor is not null)
                    return;
                _contracts.BindContract(contract.Owner, player.GetMind()!);
                break;
            case ContractsUiMessageEvent.ContractAction.Join:
                _contracts.BindContract(contract.Owner, player.GetMind()!);
                break;
            case ContractsUiMessageEvent.ContractAction.Cancel:
                if (contract.OwningContractor != player.GetMind()!)
                    return;
                _contracts.TryCancelContract(contract.Owner);
                break;
            case ContractsUiMessageEvent.ContractAction.Leave:
                break;
            case ContractsUiMessageEvent.ContractAction.Start:
                if (contract.OwningContractor != player.GetMind()!)
                    return;
                _contracts.TryActivateContract(contract.Owner);
                break;
            case ContractsUiMessageEvent.ContractAction.Hail:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        UpdateUiState(uid, args.LoaderUid, player, component);
    }

    private void OnUiReady(EntityUid uid, ContractsCartridgeComponent component, CartridgeUiReadyEvent args)
    {
        UpdateUiState(uid, args.Loader, (IPlayerSession)args.Session, component);
    }

    public ContractListUiState GenerateState(EntityUid cart, IPlayerSession user)
    {
        var mind = user.GetMind()!;
        var conQuery = EntityQueryEnumerator<ContractComponent, ContractCriteriaControlComponent, ContractGroupsComponent>();

        var contractStates = new Dictionary<Guid, ContractUiState>();

        var lookingFor = new HashSet<string>();

        var noVessel = false;

        if (mind.Contracts.Any(HasComp<VesselContractComponent>))
        {
            noVessel = true;
            var vesselContract = mind.Contracts.Where(HasComp<VesselContractComponent>).First();
            if (TryComp<ContractGroupsComponent>(vesselContract, out var groups))
            {
                lookingFor.UnionWith(groups.Groups.Where(x => x != "Vessel"));
            }
        }

        while (conQuery.MoveNext(out var contract, out var contractComp, out var criteriaComp, out var contractGroups))
        {
            if (!contractGroups.Groups.IsSupersetOf(lookingFor))
                continue;

            var status = ContractUiState.ContractUserStatus.OpenToJoin;
            var subCons = contractComp.SubContractors;
            if (contractComp.OwningContractor is null)
            {
                status = ContractUiState.ContractUserStatus.OpenToOwn;
            }
            else if (subCons.Contains(mind))
            {
                status = ContractUiState.ContractUserStatus.Subcontractor;
            }
            else if (contractComp.OwningContractor == mind)
            {
                status = ContractUiState.ContractUserStatus.Owner;
            }

            if (noVessel && contractGroups.Groups.Contains("Vessel") &&
                status is not ContractUiState.ContractUserStatus.Owner
                    and not ContractUiState.ContractUserStatus.Subcontractor)
                continue;

            if (contractComp.Status is ContractStatus.Finalized or ContractStatus.Breached && status == ContractUiState.ContractUserStatus.OpenToJoin)
                continue;

            var ownerName = contractComp.OwningContractor?.CharacterName ?? "[INACTIVE]";
            var subContractorNames = contractComp.SubContractors.Select(x => x.CharacterName!).ToList();
            var contractDesc = FormattedMessage.FromUnformatted(Description(contract));
            var ev = new GetContractDescription(new ContractDisplayData(contractDesc));
            RaiseLocalEvent(contract, ref ev);

            var state = new ContractUiState(status, Name(contract), ownerName, subContractorNames,
                ev.Data, contractComp.Status);

            if (!_contracts.CouldChangeStatusTo(contract, ContractStatus.Active, out var failMsg))
            {
                state.Startable = false;
                state.NoStartReason = failMsg;
            }

            foreach (var (group, criteria) in criteriaComp.Criteria)
            {
                state.Criteria[group] = new();
                state.Effects[group] = new();
                var list = state.Criteria[group];
                var effects = state.Effects[group];
                foreach (var criterion in criteria)
                {
                    if (_criteria.TryGetCriteriaDisplayData(criterion, out var data))
                        list.Add(data.Value);
                }

                var criteriaEffects = criteriaComp.CriteriaEffects;
                if (!criteriaEffects.ContainsKey(group))
                    continue;

                foreach (var effect in criteriaComp.CriteriaEffects[group])
                {
                    if (effect.Describe() is {} desc)
                        effects.Add(FormattedMessage.FromMarkup(desc));
                }
            }

            contractStates.Add(contractComp.Uuid, state);
        }

        return new ContractListUiState(contractStates, mind.BankAccount?.Thalers ?? FixedPoint2.Zero);
    }

    private void UpdateUiState(EntityUid uid, EntityUid loaderUid, IPlayerSession session, ContractsCartridgeComponent? component)
    {
        if (!Resolve(uid, ref component))
            return;

        var innerState = GenerateState(uid, session);

        _cartridgeLoaderSystem?.UpdateCartridgeUiState(loaderUid, new ContractCartridgeUiState(innerState));
    }
}

[ByRefEvent]
public record struct GetContractDescription(ContractDisplayData Data);
