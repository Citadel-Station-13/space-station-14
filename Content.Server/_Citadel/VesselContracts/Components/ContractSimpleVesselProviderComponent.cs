namespace Content.Server._Citadel.VesselContracts.Components;

/// <summary>
/// This is used for creating and FTLing in a contract vessel, without any of the complexities one would normally want.
/// This is to speed up testing.
/// </summary>
[RegisterComponent]
public sealed class ContractSimpleVesselProviderComponent : Component
{
    [DataField("vesselMap")]
    public string VesselMap = "/Maps/Shuttles/mining.yml";
}
