using Content.Server._00Citadel.Worldgen.Systems;

namespace Content.Server._00Citadel.Worldgen.Components;

/// <summary>
/// This is used for marking an entity as being a world chunk.
/// </summary>
[RegisterComponent, Access(typeof(WorldControllerSystem))]
public sealed class WorldChunkComponent : Component
{
    [DataField("coordinates")]
    public Vector2i Coordinates;

    [DataField("map")]
    public EntityUid Map;
}
