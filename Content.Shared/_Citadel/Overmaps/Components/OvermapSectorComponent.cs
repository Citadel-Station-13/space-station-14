using Robust.Shared.Containers;
using Robust.Shared.GameStates;

namespace Content.Shared._Citadel.Overmaps.Components;

/// <summary>
/// Denotes an overmap entity as being able to be landed in.
///
/// Overmap sectors automatically capture things that slow down enough
/// to drop out of supercruise.
///
/// Sectors require map bindings to work. If there are no map bindings,
/// an exception will be thrown when ships attempt to land.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class OvermapSectorComponent : Component
{
    /// <summary>
    /// Range where we automatically capture a contacting entity
    /// that's dropping out of overmap, if it's an ephemerally
    /// travelling entity.
    ///
    /// That entity's influence range will also be taken into account
    /// (handled on the entity's side); two sectors may not overlap
    /// by this manner.
    /// </summary>
    [ViewVariables, AutoNetworkedField]
    public float influenceRange { get; set; } = 5.0f;

    #warning how to do this
    /// <summary>
    /// Holds all the overmap entities currently in ourselves.
    ///
    /// This is nestable. A sector should however, not be allowed to join another sector
    /// while things are inside it, as true map-merging is not yet implemented.
    ///
    /// Undefined behavior results if that happens.
    /// This is server-side only.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public Container EntityContainer = default!;
}
