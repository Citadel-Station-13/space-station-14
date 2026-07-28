using Robust.Shared.GameStates;

namespace Content.Server._Citadel.Overmaps.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class OvermapEmissionsComponent : Component
{
    [ViewVariables]
    public List<OvermapSignal> Signals { get; set; } = new List<OvermapSignal>();
}
