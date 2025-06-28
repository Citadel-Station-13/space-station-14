using Content.Shared._Citadel.Overmaps.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Map;

namespace Content.Shared._Citadel.Overmaps.Components
{
    [RegisterComponent, NetworkedComponent]
    public sealed partial class OvermapBindingsComponent : Component
    {
        /// <summary>
        /// Bound map ID(s)
        /// </summary>
        ///
        /// TODO: multiz support
        [Access(typeof(SharedOvermapBindingsSystem), Other = AccessPermissions.ReadExecute)]
        public MapId MapId { get; internal set; } = MapId.Nullspace;

    }
}
