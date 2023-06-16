using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Server._Citadel.Contracts;
using Content.Server._Citadel.Contracts.Components;
using Content.Server.Mind.Components;
using Content.Server.Station.Systems;
using Content.Shared.FixedPoint;
using JetBrains.Annotations;

namespace Content.Server._Citadel.Thalers;

/// <summary>
/// This handles personal bank accounts.
/// </summary>
public sealed class PersonalBankSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<CriteriaGroupAwardCash>(OnAwardCash);
    }

    private void OnAwardCash(CriteriaGroupAwardCash ev)
    {
        var contract = Comp<ContractComponent>(ev.Contract);
        foreach (var mind in contract.SubContractors.Append(contract.OwningContractor!))
        {
            if (mind.OwnedEntity is null)
                continue;

            TryAdjustBalance(mind.OwnedEntity.Value, ev.Amount);
        }
    }

    [PublicAPI]
    public bool TryGetBalance(EntityUid user, [NotNullWhen(true)] out FixedPoint2? balance)
    {
        balance = null;

        if (!TryComp<MindComponent>(user, out var mindComp) || mindComp.Mind is not { } mind || mind.BankAccount is not { } account)
            return false;

        balance = account.Thalers;
        return true;
    }

    [PublicAPI]
    public bool CanAdjustBalance(EntityUid user, FixedPoint2 amount)
    {
        if (!TryComp<MindComponent>(user, out var mindComp) || mindComp.Mind is not { } mind || mind.BankAccount is not { } account)
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
    public bool TryAdjustBalance(EntityUid user, FixedPoint2 amount)
    {
        if (!TryComp<MindComponent>(user, out var mindComp) || mindComp.Mind is not { } mind || mind.BankAccount is not { } account)
            return false;

        if (!CanAdjustBalance(user, amount))
            return false;

        account.Thalers += amount;

        return true;
    }
}

[Access(typeof(PersonalBankSystem), typeof(Mind.Mind))]
public sealed class BankAccount
{
    // TODO(Lunar): FixedPoint2 can't represent more than ~20mil or so. Maybe need a new thing if we expect people to get stupid rich.
    // but having someone's balance overflow into the negatives is FUNNY!!!
    public FixedPoint2 Thalers = 2500;
}


public sealed record CriteriaGroupAwardCash(FixedPoint2 Amount) : CriteriaGroupEffectEvent
{
    [DataField("amount")]
    public FixedPoint2 Amount = Amount;

    public override string? Describe()
    {
        return Loc.GetString("criteria-group-award-cash-effect-description", ("amount", Amount));
    }
}
