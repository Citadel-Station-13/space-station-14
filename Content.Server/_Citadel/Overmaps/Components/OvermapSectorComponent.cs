namespace Content.Server._Citadel.Overmaps.Components;

/// <summary>
/// Denotes an overmap entity as being able to be landed in.
///
/// Overmap sectors automatically capture things that slow down enough
/// to drop out of supercruise.
///
/// Sectors require map bindings to work. If there are no map bindings,
/// an exception will be thrown when ships attempt to land.
/// </summary>
[RegisterComponent]
public sealed partial class OvermapSectorComponent : Component
{

}
