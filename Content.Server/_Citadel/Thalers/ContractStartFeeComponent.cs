using Content.Shared.FixedPoint;

namespace Content.Server._Citadel.Thalers;

/// <summary>
/// This is used for contracts that cost money to start.
/// </summary>
[RegisterComponent]
public sealed class ContractStartFeeComponent : Component
{
    [DataField("cost", required: true)]
    public FixedPoint2 Cost = FixedPoint2.Zero;
}
