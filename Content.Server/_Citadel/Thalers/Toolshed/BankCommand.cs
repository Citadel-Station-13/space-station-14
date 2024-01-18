using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.FixedPoint;
using Content.Shared.Mind;
using Robust.Shared.Toolshed;
using Robust.Shared.Toolshed.Syntax;

namespace Content.Server._Citadel.Thalers.Toolshed;

[ToolshedCommand, AdminCommand(AdminFlags.Debug)]
public sealed class BankCommand : ToolshedCommand
{
    private PersonalBankSystem? _bank;

    [CommandImplementation("getthalers")]
    public FixedPoint2 GetThalers([PipedArgument] MindComponent mind)
    {
        _bank ??= GetSys<PersonalBankSystem>();

        if (_bank.TryGetBalance(mind, out var balance))
            return balance.Value;

        return FixedPoint2.Zero;
    }

    [CommandImplementation("adjust")]
    public MindComponent Adjust(
            [CommandInvocationContext] IInvocationContext ctx,
            [PipedArgument] MindComponent mind,
            [CommandArgument] ValueRef<float> adjustment
        )
    {
        _bank ??= GetSys<PersonalBankSystem>();

        var by = adjustment.Evaluate(ctx);
        _bank.TryAdjustBalance(mind, by);

        return mind;
    }
}
