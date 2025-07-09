using Content.Shared._Citadel.Relations;
using Robust.Shared.Prototypes;

namespace Content.Shared._Citadel.Worldgen.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class CitadelWorldChunkManagerComponent : Component, IFamilyRelationParent
{
    /// <inheritdoc/>
    [DataField]
    public HashSet<EntityUid> Children { get; set; } = new();

    /// <summary>
    ///     Index of coordinates to children.
    /// </summary>
    [DataField]
    public Dictionary<Vector2i, EntityUid> ChildrenIndex = new();

    [DataField]
    public EntProtoId<CitadelWorldChunkComponent> ChunkType;
}
