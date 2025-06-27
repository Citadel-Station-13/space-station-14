using Content.Server._Citadel.Overmaps;
using Robust.Shared.GameStates;
using Robust.Shared.Map;

namespace Content.Shared._Citadel.Overmaps
{
    [RegisterComponent, NetworkedComponent]
    public sealed partial class OvermapComponent : Component
    {
        [ViewVariables(VVAccess.ReadOnly), Access(typeof(SharedOvermapSystem), Other = AccessPermissions.ReadExecute)]
        public MapId MapId { get; internal set; } = MapId.Nullspace;
    }
}
