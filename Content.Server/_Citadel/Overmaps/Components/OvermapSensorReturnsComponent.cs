namespace Content.Server._Citadel.Overmaps.Components;

/// <summary>
/// Holds data on sensor returns obtained by illuminating the entity
/// with active sensors.
/// </summary>
[RegisterComponent]
public sealed partial class OvermapSensorReturnsComponent : Component
{
    [ViewVariables]
    public List<OvermapSensorReturn> Returns { get; set; } = new List<OvermapSensorReturn>();
}
