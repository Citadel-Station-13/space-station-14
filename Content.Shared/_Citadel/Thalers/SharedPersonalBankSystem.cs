using Content.Shared.FixedPoint;

namespace Content.Shared._Citadel.Thalers;

[Access(typeof(PersonalBankSystem), typeof(Mind.Mind))]
public sealed class BankAccount : IToolshedPrettyPrint
{
    // TODO(Lunar): FixedPoint2 can't represent more than ~20mil or so. Maybe need a new thing if we expect people to get stupid rich.
    // but having someone's balance overflow into the negatives is FUNNY!!!
    [ViewVariables(VVAccess.ReadWrite)]
    public FixedPoint2 Thalers = 2500;

    public string PrettyPrint(ToolshedManager toolshed, out IEnumerable? more, bool moreUsed = false, int? maxOutput = null)
    {
        more = null;
        return $"BankAccount {{ Thalers: {Thalers} }}";
    }
}
