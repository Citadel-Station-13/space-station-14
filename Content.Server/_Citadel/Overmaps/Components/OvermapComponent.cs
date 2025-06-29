using Content.Shared._Citadel.Overmaps.Systems;
using Robust.Shared.GameStates;

namespace Content.Server._Citadel.Overmaps.Components
{
    [RegisterComponent, NetworkedComponent]
    public sealed class OvermapComponent : Component
    {
        [ViewVariables(VVAccess.ReadOnly), Access(typeof(SharedOvermapSystem), Other = AccessPermissions.ReadExecute)]
        public EntityUid? MapEntity { get; internal set; };

        public OvermapConfig Config { get; set; }

    }
}
