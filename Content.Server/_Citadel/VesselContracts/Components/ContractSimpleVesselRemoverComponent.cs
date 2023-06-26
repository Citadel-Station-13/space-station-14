namespace Content.Server._Citadel.VesselContracts.Components;

/// <summary>
/// This is used for simple vessel removal.
/// </summary>
[RegisterComponent]
public sealed class ContractSimpleVesselRemoverComponent : Component
{
    [DataField("active")]
    public bool Active = false;
}
