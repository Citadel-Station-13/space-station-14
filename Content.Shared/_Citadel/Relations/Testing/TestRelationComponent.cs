namespace Content.Shared._Citadel.Relations.Testing;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class TestRelationComponent : Component, IRelationParent, IRelationChild
{
    [DataField]
    public HashSet<EntityUid> Children { get; private set; } = new();
    [DataField]
    public EntityUid? Parent { get; set; } = null;
}
