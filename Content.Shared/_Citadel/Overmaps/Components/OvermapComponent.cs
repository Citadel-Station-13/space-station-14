using Content.Shared._Citadel.Overmaps.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Map;

namespace Content.Shared._Citadel.Overmaps.Components
{
    [RegisterComponent, NetworkedComponent]
    public sealed partial class OvermapComponent : Component
    {
        [ViewVariables(VVAccess.ReadOnly), Access(typeof(SharedOvermapSystem), Other = AccessPermissions.ReadExecute)]
        public MapId MapId { get; internal set; } = MapId.Nullspace;

        [ViewVariables(VVAccess.ReadOnly), Access(typeof(SharedOvermapSystem), Other = AccessPermissions.ReadExecute)]
        public OvermapId? OvermapId { get; internal set; } = null;

        public OvermapConfig Config { get; set; }

    }
}
