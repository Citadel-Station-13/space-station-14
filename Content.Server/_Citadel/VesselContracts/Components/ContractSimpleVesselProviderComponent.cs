using Content.Server.Station;

namespace Content.Server._Citadel.VesselContracts.Components;

/// <summary>
/// This is used for creating and FTLing in a contract vessel, without any of the complexities one would normally want.
/// This is to speed up testing.
/// </summary>
[RegisterComponent]
public sealed partial class ContractSimpleVesselProviderComponent : Component
{
    [DataField("vesselMap")]
    public string VesselMap = "/Maps/_citadel/mining_vessel_small.yml";

    [DataField("vesselConfig", required: true)]
    public StationConfig VesselConfig = default!;
}
