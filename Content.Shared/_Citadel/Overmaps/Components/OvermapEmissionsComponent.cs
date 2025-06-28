using Robust.Shared.GameStates;

namespace Content.Shared._Citadel.Overmaps.Components {
    [RegisterComponent, NetworkedComponent]
    public sealed partial class OvermapEmissionsComponent : Component
    {
        public List<OvermapSignal> Signals { get; set; }
    }
}
