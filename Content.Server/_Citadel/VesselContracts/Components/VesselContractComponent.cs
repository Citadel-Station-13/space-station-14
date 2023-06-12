namespace Content.Server._Citadel.VesselContracts.Components;

/// <summary>
/// This is used for contracts that provide a vessel.
/// </summary>
[RegisterComponent]
public sealed class VesselContractComponent : Component
{
    [DataField("vessel")]
    public EntityUid? Vessel;
}
