namespace Content.Server._Citadel.VesselContracts.Components;

/// <summary>
/// This is used for vessels that have a contract they're part of.
/// </summary>
[RegisterComponent]
public sealed partial class ContractedVesselComponent : Component
{
    [DataField("contract")]
    public EntityUid Contract = default!;
}
