namespace Content.Server._Citadel.Overmaps.Components;

/// <summary>
/// Tracks interdiction state.
///
/// Will not automatically force a supercruise drop / interdiction effects, whatever is causing this to
/// tick up has to provide the trigger.
///
/// This ensures that we don't have a weird situation where one person is interdicted and someone else isn't.
/// </summary>
[RegisterComponent]
public sealed partial class OvermapSupercruiseInterdictionComponent : Component
{

}
