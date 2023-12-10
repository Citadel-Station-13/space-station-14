using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.FixedPoint;
using Robust.Shared.Toolshed;
using Robust.Shared.Toolshed.Syntax;

namespace Content.Server._Citadel.Thalers.Toolshed;

[ToolshedCommand, AdminCommand(AdminFlags.Debug)]
public sealed class BankCommand : ToolshedCommand
{
    private PersonalBankSystem? _bank;

    [CommandImplementation("getthalers")]
    public FixedPoint2 GetThalers([PipedArgument] EntityUid user)
    {
        _bank ??= GetSys<PersonalBankSystem>();

        if (_bank.TryGetBalance(user, out var balance))
            return balance.Value;

        return FixedPoint2.Zero;
    }

    [CommandImplementation("adjust")]
    public EntityUid Adjust(
            [CommandInvocationContext] IInvocationContext ctx,
            [PipedArgument] EntityUid user,
            [CommandArgument] ValueRef<float> adjustment
        )
    {
        _bank ??= GetSys<PersonalBankSystem>();

        var by = adjustment.Evaluate(ctx);
        _bank.TryAdjustBalance(user, by);

        return user;
    }
}
