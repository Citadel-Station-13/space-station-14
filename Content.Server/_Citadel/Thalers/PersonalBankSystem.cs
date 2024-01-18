﻿using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Server._Citadel.Contracts;
using Content.Server._Citadel.Contracts.Components;
using Content.Server._Citadel.Contracts.Prototypes;
using Content.Server._Citadel.PDAContracts.Systems;
using Content.Server.Chat.Managers;
using Content.Server.Chat.Systems;
using Content.Server.Station.Systems;
using Content.Shared._Citadel.Contracts;
using Content.Shared._Citadel.Thalers;
using Content.Shared.Chat;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.FixedPoint;
using JetBrains.Annotations;

namespace Content.Server._Citadel.Thalers;

/// <summary>
/// This handles personal bank accounts.
/// </summary>
public sealed class PersonalBankSystem : EntitySystem
{
    [Dependency] private readonly IChatManager _chat = default!;
    [Dependency] private readonly IViewVariablesManager _vv = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<CriteriaGroupAwardCash>(OnAwardCash);
        // maybe give this it's own system, it's not relevant to banks.
        SubscribeLocalEvent<ContractStartFeeComponent, ContractTryStatusChange>(OnContractTryStatusChange);
        SubscribeLocalEvent<ContractStartFeeComponent, GetContractDescription>(OnGetDescription);
    }

    private void OnGetDescription(EntityUid uid, ContractStartFeeComponent component, ref GetContractDescription args)
    {
        var msg = args.Data.Description;
        msg.PushNewline();
        msg.AddText($"This contract costs {component.Cost} Thalers for the main signer to start.");
    }

    private void OnContractTryStatusChange(EntityUid uid, ContractStartFeeComponent component, ref ContractTryStatusChange args)
    {
        if (args is not {Old: ContractStatus.Initiating, New: ContractStatus.Active})
            return; // If we're not still in setup, we don't care.

        var contract = Comp<ContractComponent>(uid);

        if (contract.OwningContractor is null)
            return;

        var success = TryAdjustBalance(contract.OwningContractor, -component.Cost);

        if (success)
            return;

        args.Cancelled = true;

        args.FailMessage.AddText($"Not enough money to start the contract, you have {contract.OwningContractor.BankAccount!.Thalers} but need {component.Cost}.");
    }

    private void OnAwardCash(CriteriaGroupAwardCash ev)
    {
        var contract = Comp<ContractComponent>(ev.Contract);
        foreach (var mind in contract.SubContractors.Append(contract.OwningContractor!))
        {
            //if (mind.OwnedEntity is null)
                //continue;

            TryAdjustBalance(mind, ev.Amount);
        }
    }

    [PublicAPI]
    public bool TryGetBalance(MindComponent mind, [NotNullWhen(true)] out FixedPoint2? balance)
    {
        /*balance = null;

        if (!TryComp<MindComponent>(user, out var mindComp) || mindComp is not {BankAccount: { } account})
            return false;*/
        balance = null;

        if(mind.BankAccount is not {} account)
            return false;

        balance = account.Thalers;
        return true;
    }

    [PublicAPI]
    public bool CanAdjustBalance(MindComponent mind, FixedPoint2 amount)
    {
        //if (!TryComp<MindComponent>(user, out var mindComp) || mindComp is not {BankAccount: { } account} mind)
            //return false;
        if (mind.BankAccount is not { } account)
            return false;

        var newSum = account.Thalers + amount;
        if (newSum < 0 && account.Thalers > 0 && amount > 0)
        {
            Logger.Error($"Some poor sod just had their bank account overflow. {mind.UserId}, {ToPrettyString(mind.OwnedEntity ?? EntityUid.Invalid)}. Originally had {account.Thalers}, intended to add {amount}, but bailed.");
            return false; // don't nuke their account.
        }
        if (newSum < 0)
            return false;

        return true;
    }

    [PublicAPI]
    public bool TryAdjustBalance(MindComponent mind, FixedPoint2 amount)
    {
        //if (!TryComp<MindComponent>(user, out var mindComp) || mindComp is not {BankAccount: { } account})
            //return false;
        if (mind.BankAccount is not { } account)
            return false;

        if (!CanAdjustBalance(mind, amount))
            return false;

        account.Thalers += amount;

        return true;
    }
}

[PublicAPI]
public sealed partial record CriteriaGroupAwardCash() : CriteriaGroupEffectEvent
{
    [DataField("amount")]
    public FixedPoint2 Amount;
    public override string? Describe()
    {
        return Loc.GetString("criteria-group-award-cash-effect-description", ("amount", Amount.Float()));
    }
}
