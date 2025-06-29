using Content.Shared._Citadel.Overmaps.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Map;

namespace Content.Server._Citadel.Overmaps.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class OvermapBindingsComponent : Component
{
    /// <summary>
    /// Bound map entities
    /// </summary>
    [Access(typeof(SharedOvermapBindingsSystem), Other = AccessPermissions.ReadExecute)]
    public List<EntityUid> BoundMapEntities { get; internal set; } = new List<EntityUid>();
}
