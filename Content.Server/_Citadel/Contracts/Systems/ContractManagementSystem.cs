﻿using System.Linq;
using Content.Server._Citadel.Contracts.Components;
using Content.Server.Administration;
using Content.Server.Mind;
using Content.Server.Mind.Components;
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
public sealed class ContractManagementSystem : EntitySystem
{
    [Dependency] private readonly IConsoleHost _consoleHost = default!;
    [Dependency] private readonly IComponentFactory _componentFactory = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    [Dependency] private readonly MindSystem _mindSystem = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        _consoleHost.RegisterCommand("makecontract", MakeContractCommand, MakeContractCommandCompletion);
        _consoleHost.RegisterCommand("lscontracts", ListContractsCommand);
    }

    [AdminCommand(AdminFlags.Admin)]
    private void ListContractsCommand(IConsoleShell shell, string argstr, string[] args)
    {
        var query = EntityQueryEnumerator<ContractComponent>();
        while (query.MoveNext(out var uid, out var contract))
        {
            shell.WriteLine($"{ToPrettyString(uid)} | OWNER: {ToPrettyString(contract.OwningContractor.OwnedEntity ?? EntityUid.Invalid)} | SUBCONS: {string.Join(',', contract.SubContractors.Select(x => ToPrettyString(x.OwnedEntity ?? EntityUid.Invalid)))}");
        }
    }

    private CompletionResult MakeContractCommandCompletion(IConsoleShell shell, string[] args)
    {
        switch (args.Length)
        {
            case 1:
            {
                var compName = _componentFactory.GetComponentName(typeof(ContractComponent));
                var options = IoCManager.Resolve<IPrototypeManager>()
                    .EnumeratePrototypes<EntityPrototype>()
                    .Where(p => p.Components.ContainsKey(compName))
                    .OrderBy(p => p.ID)
                    .Select(p => p.ID);

                return CompletionResult.FromHintOptions(options, "<preset>");
            }
            case 2:
            {
                var options = _playerManager.ServerSessions.Select(c => c.Name).OrderBy(c => c);
                return CompletionResult.FromHintOptions(options, Loc.GetString("cmd-ban-hint"));
            }
            default:
                return CompletionResult.Empty;
        }
    }

    [AdminCommand(AdminFlags.Admin)]
    private void MakeContractCommand(IConsoleShell shell, string argstr, string[] args)
    {
        if (args.Length != 2)
        {
            shell.WriteError("Invalid number of arguments.");
            return;
        }

        if (!_playerManager.TryGetSessionByUsername(args[1], out var session))
        {
            shell.WriteError($"Unknown player {args[1]}");
            return;
        }

        if (!TryComp<MindComponent>(session.AttachedEntity, out var mind) || mind.Mind is null)
        {
            shell.WriteError($"{args[1]}'s entity has no mind! Cannot assign to contract.");
            return;
        }

        var contract = CreateBoundContract(args[0], mind.Mind);

        shell.WriteLine($"Bound the contract {ToPrettyString(contract)} to {ToPrettyString(session.AttachedEntity.Value)}");
    }

    public EntityUid CreateBoundContract(string contractProto, Mind.Mind owner)
    {
        var contractEnt = Spawn(contractProto, MapCoordinates.Nullspace);
        DebugTools.Assert(HasComp<ContractComponent>(contractEnt), $"The contract {contractProto} is missing required components!");

        var contract = Comp<ContractComponent>(contractEnt);
        contract.Status = ContractStatus.Initiating;
        contract.OwningContractor = owner;

        RaiseLocalEvent(new ContractStatusChangedEvent(ContractStatus.Uninitialized, ContractStatus.Initiating));

        return contractEnt;
    }

    private void ChangeContractState(EntityUid contractUid, ContractComponent contractComponent,
        ContractStatus newStatus)
    {
        var oldStatus = contractComponent.Status;
        contractComponent.Status = newStatus;
        RaiseLocalEvent(new ContractStatusChangedEvent(oldStatus, newStatus));
    }

    public bool TryActivateContract(EntityUid contractUid, ContractComponent? contractComponent)
    {
        if (!Resolve(contractUid, ref contractComponent))
            return false;

        if (contractComponent.Status != ContractStatus.Initiating)
            return false;

        ChangeContractState(contractUid, contractComponent, ContractStatus.Active);
        return true;
    }

    public bool TryFinalizeContract(EntityUid contractUid, ContractComponent? contractComponent)
    {
        if (!Resolve(contractUid, ref contractComponent))
            return false;

        if (contractComponent.Status != ContractStatus.Active)
            return false;

        ChangeContractState(contractUid, contractComponent, ContractStatus.Finalized);
        return true;
    }

    public bool TryBreachContract(EntityUid contractUid, ContractComponent? contractComponent)
    {
        if (!Resolve(contractUid, ref contractComponent))
            return false;

        if (contractComponent.Status != ContractStatus.Active)
            return false;

        ChangeContractState(contractUid, contractComponent, ContractStatus.Breached);
        return true;
    }

    public bool TryCancelContract(EntityUid contractUid, ContractComponent? contractComponent)
    {
        if (!Resolve(contractUid, ref contractComponent))
            return false;

        if (contractComponent.Status is not ContractStatus.Initiating and not ContractStatus.Active)
            return false;

        ChangeContractState(contractUid, contractComponent, ContractStatus.Cancelled);
        return true;
    }
}
