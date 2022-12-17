using Content.Server._00Citadel.Worldgen.Systems;

namespace Content.Server._00Citadel.Worldgen.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, Access(typeof(WorldControllerSystem))]
public sealed class WorldControllerComponent : Component
{
    [DataField("chunks")]
    public Dictionary<Vector2i, EntityUid> Chunks = new();
}
