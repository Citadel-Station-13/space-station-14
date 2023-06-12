namespace Content.Server._Citadel.VesselContracts.Components;

/// <summary>
/// This is used for contracts in which the contractor is granted a vessel of their choosing.
/// This is for the voucher given to the contractor.
/// </summary>
[RegisterComponent]
public sealed class VesselVoucherComponent : Component
{
    /// <summary>
    /// Contract that owns this voucher.
    /// </summary>
    [DataField("contract")]
    public EntityUid Contract = EntityUid.Invalid;
    /// <summary>
    /// The vessel to be granted by the voucher.
    /// </summary>
    [DataField("vesselMap")]
    public string VesselMap = "/Maps/Shuttles/mining.yml";
}
