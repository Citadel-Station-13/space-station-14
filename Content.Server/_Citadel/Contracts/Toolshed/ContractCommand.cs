using System.Linq;
using Content.Server._Citadel.Contracts.Components;
using Content.Server._Citadel.Contracts.Systems;
using Content.Server.Administration;
using Content.Server.Players;
using Content.Shared._Citadel.Contracts;
using Content.Shared.Administration;
using Content.Shared.Mind;
using Content.Shared.Players;
using Robust.Server.Player;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed;
using Robust.Shared.Toolshed.TypeParsers;
using Robust.Shared.Utility;

namespace Content.Server._Citadel.Contracts.Toolshed;

[ToolshedCommand, AdminCommand(AdminFlags.Admin)]
public sealed class ContractCommand : ToolshedCommand
{
    private ContractManagementSystem? _contractManagement;
    private ContractCriteriaSystem? _contractCriteria;

    [CommandImplementation("new")]
    public EntityUid New([CommandArgument] Prototype<EntityPrototype> contract)
    {
        _contractManagement ??= GetSys<ContractManagementSystem>();

        return _contractManagement.CreateUnboundContract(contract.AsType());
    }

    [CommandImplementation("bindto")]
    public EntityUid New([PipedArgument] EntityUid contract, [CommandArgument] ICommonSession session)
    {
        _contractManagement ??= GetSys<ContractManagementSystem>();
        _contractManagement.BindContract(contract, session.GetMind());
        return contract;
    }


    [CommandImplementation("owningnew")]
    public EntityUid New([PipedArgument] ICommonSession session, [CommandArgument] Prototype<EntityPrototype> contract)
    {
        _contractManagement ??= GetSys<ContractManagementSystem>();

        return _contractManagement.CreateBoundContract(contract.AsType(), session.GetMind());
    }

    [CommandImplementation("list")]
    public IEnumerable<EntityUid> List()
    {
        var q = EntityManager.EntityQueryEnumerator<ContractComponent>();

        while (q.MoveNext(out var uid, out _))
            yield return uid;
    }

    [CommandImplementation("detail")]
    public IEnumerable<FormattedMessage> Detail([PipedArgument] IEnumerable<EntityUid> ents)
        => ents.Select(Detail);

    [CommandImplementation("detail")]
    public FormattedMessage Detail([PipedArgument] EntityUid ent)
    {
        _contractCriteria ??= GetSys<ContractCriteriaSystem>();

        if (!TryComp<ContractComponent>(ent, out var contract))
            return FormattedMessage.FromUnformatted("[not a contract]");

        var msg = new FormattedMessage();
        msg.AddMarkup($"== {EntName(ent)} ==");
        msg.PushNewline();
        switch (contract.Status)
        {
            case ContractStatus.Uninitialized:
                msg.AddMarkup("[color=red]Uninitialized[/color]");
                break;
            case ContractStatus.Initiating:
                msg.AddMarkup("[color=yellow]Initiating[/color]");
                break;
            case ContractStatus.Active:
                msg.AddMarkup("[color=green]Active[/color]");
                break;
            case ContractStatus.Finalized:
                msg.AddMarkup("[color=magenta]Finalized[/color]");
                break;
            case ContractStatus.Breached:
                msg.AddMarkup("[color=red]Breached[/color]");
                break;
            case ContractStatus.Cancelled:
                msg.AddMarkup("[color=red]Cancelled[/color]");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        msg.PushNewline();

        if (contract.OwningContractor is {} owner)
            msg.AddMarkup($"Owner: {owner.OwnedEntity}, {owner.Session}");
        else
            msg.AddMarkup("No owner.");

        msg.PushNewline();

        if (contract.SubContractors.Count != 0)
        {
            msg.AddMarkup("Subcontractors:");
            msg.PushNewline();
            foreach (var contractor in contract.SubContractors)
            {
                msg.AddMarkup($"- {contractor.OwnedEntity}, {contractor.Session}");
                msg.PushNewline();
            }
        }

        if (!TryComp<ContractCriteriaControlComponent>(ent, out var criteria))
        {
            msg.AddMarkup("No criteria.");
            return msg;
        }

        var groups = criteria.Criteria.Keys;
        foreach (var group in groups)
        {
            msg.AddMarkup($"Group [color=yellow]{group}[/color]");
            msg.PushNewline();
            msg.AddMarkup($"  Criteria:");
            msg.PushNewline();
            foreach (var criterion in criteria.Criteria[group])
            {
                msg.AddMarkup($"  - Criterion [color=cyan]{EntityManager.ToPrettyString(criterion)}[/color]:");
                msg.PushNewline();
                if (!_contractCriteria.TryGetCriteriaDisplayData(criterion, out var data))
                {
                    msg.AddMarkup("[Criteria has no data, may be bugged?]");
                    msg.PushNewline();
                    continue;
                }
                msg.AddMarkup("      ");
                msg.AddMessage(data.Value.Description);
                msg.PushNewline();
            }

            var effects = criteria.CriteriaEffects;

            if (!effects.ContainsKey(group))
                break;

            msg.AddMarkup($"   Effects:");
            msg.PushNewline();
            foreach (var effect in effects[group])
            {
                msg.AddMarkup($"  - ");
                msg.AddMarkup(effect.Describe() ?? $"[effect has no desc] {effect.GetType()}");
                msg.PushNewline();
            }
        }


        return msg;
    }

    [CommandImplementation("activate")]
    public bool TryActivate([PipedArgument] EntityUid contract)
    {
        _contractManagement ??= GetSys<ContractManagementSystem>();

        return _contractManagement.TryActivateContract(contract);
    }

    [CommandImplementation("finalize")]
    public bool TryFinalize([PipedArgument] EntityUid contract)
    {
        _contractManagement ??= GetSys<ContractManagementSystem>();

        return _contractManagement.TryFinalizeContract(contract);
    }

    [CommandImplementation("breach")]
    public bool TryBreach([PipedArgument] EntityUid contract)
    {
        _contractManagement ??= GetSys<ContractManagementSystem>();

        return _contractManagement.TryBreachContract(contract);
    }

    [CommandImplementation("cancel")]
    public bool TryCancel([PipedArgument] EntityUid contract)
    {
        _contractManagement ??= GetSys<ContractManagementSystem>();

        return _contractManagement.TryCancelContract(contract);
    }
}
