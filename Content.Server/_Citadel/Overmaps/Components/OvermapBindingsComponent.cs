using Content.Shared._Citadel.Overmaps.Systems;

namespace Content.Server._Citadel.Overmaps.Components;

public sealed class OvermapBindingsComponent : Component
{
    /// <summary>
    /// Bound map entities
    /// </summary>
    [Access(typeof(SharedOvermapBindingsSystem), Other = AccessPermissions.ReadExecute)]
    public List<EntityUid> BoundMapEntities { get; internal set; } = new List<EntityUid>();
}
