namespace Content.Server._00Citadel.Worldgen.Components;

/// <summary>
/// This is used for marking a chunk as loaded.
/// </summary>
[RegisterComponent]
public sealed class LoadedChunkComponent : Component
{
    [ViewVariables]
    public List<EntityUid>? Loaders = null;
}
