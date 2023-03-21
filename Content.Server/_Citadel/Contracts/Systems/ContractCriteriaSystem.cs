using System.Diagnostics.CodeAnalysis;
using Content.Server._Citadel.Contracts.Components;
using Content.Server._Citadel.Contracts.Prototypes;
using Content.Server.Administration;
using Content.Shared.Administration;
using JetBrains.Annotations;
using Robust.Shared.Console;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server._Citadel.Contracts.Systems;

/// <summary>
/// This handles contract criteria, their setup, and querying them.
/// </summary>
[PublicAPI]
public sealed class ContractCriteriaSystem : EntitySystem
{
    [Dependency] private readonly IConsoleHost _consoleHost = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    [Dependency] private readonly ContractManagementSystem _contract = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ContractCriteriaControlComponent, ContractStatusChangedEvent>(OnContractStatusChanged);
        SubscribeLocalEvent<ContractComponent, CriteriaUpdatedEvent>(OnCriteriaUpdated);
        _consoleHost.RegisterCommand("finalizecontract", FinalizeContract);
        _consoleHost.RegisterCommand("breachcontract", BreachContract);
        _consoleHost.RegisterCommand("activatecontract", ActivateContract);
    }

    [AdminCommand(AdminFlags.Admin)]
    private void FinalizeContract(IConsoleShell shell, string argstr, string[] args)
    {
        if (!EntityUid.TryParse(args[0], out var uid))
        {
            shell.WriteError($"The input '{args[0]}' is not a valid entity ID.");
        }

        if (_contract.TryFinalizeContract(uid, null))
        {
            shell.WriteLine("Finalized.");
        }
        else
        {
            shell.WriteError("Failed to finalize!");
        }
    }

    [AdminCommand(AdminFlags.Admin)]
    private void BreachContract(IConsoleShell shell, string argstr, string[] args)
    {
        if (!EntityUid.TryParse(args[0], out var uid))
        {
            shell.WriteError($"The input '{args[0]}' is not a valid entity ID.");
        }

        if (_contract.TryBreachContract(uid, null))
        {
            shell.WriteLine("Breached.");
        }
        else
        {
            shell.WriteError("Failed to breach!");
        }
    }

    [AdminCommand(AdminFlags.Admin)]
    private void ActivateContract(IConsoleShell shell, string argstr, string[] args)
    {
        if (!EntityUid.TryParse(args[0], out var uid))
        {
            shell.WriteError($"The input '{args[0]}' is not a valid entity ID.");
        }

        if (_contract.TryActivateContract(uid, null))
        {
            shell.WriteLine("Activated.");
        }
        else
        {
            shell.WriteError("Failed to activate!");
        }
    }

    #region Event Handling
    private void OnCriteriaUpdated(EntityUid uid, ContractComponent component, CriteriaUpdatedEvent args)
    {
        if (component.Status != ContractStatus.Active)
            return;

        if (!TryComp<ContractCriteriaControlComponent>(uid, out var criteriaControl))
            return;

        foreach (var (groupProto, criteria) in criteriaControl.Criteria)
        {
            var group = _proto.Index<CriteriaGroupPrototype>(groupProto);

            if (criteria.Count == 0)
                continue;

            switch (group.Mode)
            {
                case CriteriaGroupMode.All:
                {
                    var allPass = true;
                    foreach (var criterion in criteria)
                    {
                        var comp = Comp<ContractCriteriaComponent>(criterion);
                        allPass &= comp.Satisfied;
                    }

                    if (allPass)
                    {
                        ActivateCriteriaGroup(uid, group);
                    }

                    break;
                }
                case CriteriaGroupMode.Any:
                {
                    foreach (var criterion in criteria)
                    {
                        var comp = Comp<ContractCriteriaComponent>(criterion);
                        if (comp.Satisfied)
                        {
                            ActivateCriteriaGroup(uid, group);
                            break;
                        }
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void ActivateCriteriaGroup(EntityUid contractUid, CriteriaGroupPrototype group)
    {
        foreach (var effect in group.Effects)
        {
            var ev = effect with { Contract = contractUid };
            RaiseLocalEvent(ev);
        }
    }

    private void OnContractStatusChanged(EntityUid contractUid, ContractCriteriaControlComponent criteriaControlComponent, ContractStatusChangedEvent args)
    {
        Logger.Debug($"{args.New}, {args.Old}");
        switch (args)
        {
            // Contract initiated, set up the criteria.
            case (ContractStatus.Uninitialized, ContractStatus.Initiating):
            {
                OnContractInitiating(contractUid, criteriaControlComponent);
                break;
            }
            // Contract active, set up criteria checking.
            case (ContractStatus.Initiating, ContractStatus.Active):
            {
                OnContractActive(contractUid, criteriaControlComponent);
                break;
            }
        }
    }

    private void OnContractInitiating(EntityUid contractUid, ContractCriteriaControlComponent criteriaControlComponent)
    {
        foreach (var (group, protos) in criteriaControlComponent.CriteriaPrototypes)
        {
            foreach (var proto in protos)
            {
                AddContractCriteria(contractUid, proto, group, criteriaControlComponent);
            }
        }
    }

    private void OnContractActive(EntityUid contractUid, ContractCriteriaControlComponent criteriaControlComponent)
    {
        foreach (var (_, criteria) in criteriaControlComponent.Criteria)
        {
            foreach (var criterion in criteria)
            {
                RaiseLocalEvent(criterion, new CriteriaStartTickingEvent());
            }
        }
    }
    #endregion

    private EntityUid BuildContractCriteria(EntityUid contractUid, string criteriaProto,
        ContractCriteriaControlComponent? criteriaControlComponent = null)
    {
        var criteriaEnt = Spawn(criteriaProto, MapCoordinates.Nullspace);
        DebugTools.Assert(HasComp<ContractCriteriaComponent>(criteriaEnt), $"The contract {criteriaProto} is missing required components!");

        var criteria = Comp<ContractCriteriaComponent>(criteriaEnt);
        criteria.OwningContract = contractUid;

        RaiseLocalEvent(criteriaEnt, new CriteriaSetupEvent());

        return criteriaEnt;
    }

    /// <summary>
    /// Adds a finalizing criteria to a contract.
    /// </summary>
    /// <returns>An initialized criteria.</returns>
    public EntityUid AddContractCriteria(EntityUid contractUid, string criteriaProto, string criteriaGroup,
        ContractCriteriaControlComponent? criteriaControlComponent = null)
    {
        // Deliberate throw-if-not-present, hence not using Resolve.
        criteriaControlComponent ??= Comp<ContractCriteriaControlComponent>(contractUid);

        var criteria = BuildContractCriteria(contractUid, criteriaProto, criteriaControlComponent);
        if (!criteriaControlComponent.Criteria.TryGetValue(criteriaGroup, out var criterion))
        {
            criterion = new List<EntityUid>();
            criteriaControlComponent.Criteria[criteriaGroup] = criterion;
        }

        criterion.Add(criteria);

        return criteria;
    }

    public bool TryGetCriteriaDisplayData(EntityUid criteriaUid, [NotNullWhen(true)] out CriteriaDisplayData? displayData)
    {
        var ev = new CriteriaGetDisplayInfo();
        RaiseLocalEvent(criteriaUid, ref ev);
        displayData = ev.Info;
        return displayData != null;
    }

    public void SetCriteriaStatus(EntityUid criteriaUid, bool status, ContractCriteriaComponent contractCriteria)
    {
        if (status == contractCriteria.Satisfied)
            return;

        contractCriteria.Satisfied = status;
        RaiseLocalEvent(contractCriteria.OwningContract, new CriteriaUpdatedEvent());
    }
}
