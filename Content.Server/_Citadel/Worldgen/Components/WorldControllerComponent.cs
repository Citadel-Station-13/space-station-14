using Content.Server._Citadel.Worldgen.Systems;

namespace Content.Server._Citadel.Worldgen.Components;

/// <summary>
/// This is used for controlling overall world loading, containing an index of all chunks in the map.
/// </summary>
[RegisterComponent, Access(typeof(WorldControllerSystem))]
public sealed class WorldControllerComponent : Component
{
    /// <summary>
    /// An index of chunks owned by the controller.
    /// </summary>
    [DataField("chunks")]
    public Dictionary<Vector2i, EntityUid> Chunks = new();
}
