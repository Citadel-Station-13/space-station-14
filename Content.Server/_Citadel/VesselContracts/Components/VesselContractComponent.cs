namespace Content.Server._Citadel.VesselContracts.Components;

/// <summary>
/// This is used for contracts that provide a vessel.
/// </summary>
[RegisterComponent]
public sealed class VesselContractComponent : Component
{
    /// <summary>
    /// The vessel attached to this contract. This uses StationSystem, so expect a station-like entity here.
    /// </summary>
    [DataField("vessel")]
    public EntityUid? Vessel;
}
