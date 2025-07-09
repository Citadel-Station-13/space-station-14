using Content.Shared._Citadel.Relations;

namespace Content.Shared._Citadel.Worldgen.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class CitadelWorldChunkComponent : Component, IFamilyRelationChild
{
    /// <inheritdoc/>
    [DataField]
    public EntityUid? Parent { get; set; }

    /// <summary>
    ///     The index of this chunk in the chunk grid.
    /// </summary>
    [DataField]
    public Vector2i Index { get; set; }

    /// <summary>
    ///     Where the chunk is in its lifecycle.
    /// </summary>
    [DataField]
    public ChunkLifeStage ChunkLifeStage { get; set; }
}

public enum ChunkLifeStage : byte
{
    Broken = 0,
    Initialized = 1,
    Loaded,
    Unloaded,
}
