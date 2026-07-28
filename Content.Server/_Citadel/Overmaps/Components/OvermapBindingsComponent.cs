using Content.Shared._Citadel.Overmaps.Systems;

namespace Content.Server._Citadel.Overmaps.Components;

/// <summary>
/// Component storing map bindings for an overmap entity.
/// </summary>
public sealed partial class OvermapBindingsComponent : Component
{
    /// <summary>
    /// Bound map entities
    /// </summary>
    [Access(typeof(SharedOvermapBindingsSystem), Other = AccessPermissions.ReadExecute)]
    public List<EntityUid> BoundMapEntities { get; } = new List<EntityUid>();
}
