namespace Content.Server._Citadel.Overmaps.Components;

/// <summary>
/// Default supercruise component.
///
/// Provides default functionality for automatically handling supercruise drops,
/// and marks the entity as a supercruise entity.
/// </summary>
[RegisterComponent]
public sealed partial class OvermapSupercruiseComponent : Component
{
    /// <summary>
    /// Drop us out of supercruise if we're moving laterally.
    ///
    /// Usually, supercruise entities should not move laterally, as supercruise is exclusively
    /// forward/backward.
    /// </summary>
    [ViewVariables]
    public bool DropOnLateralMotion { get; set; } = true;
}
