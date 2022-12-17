namespace Content.Server._00OuterRim.Worldgen2.Components;

/// <summary>
/// This is used for marking a chunk as loaded.
/// </summary>
[RegisterComponent]
public sealed class LoadedChunkComponent : Component
{
    public List<EntityUid>? Loaders = null;
}
