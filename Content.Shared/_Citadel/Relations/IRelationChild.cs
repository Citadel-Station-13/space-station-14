namespace Content.Shared._Citadel.Relations;

/// <summary>
///     Indicates a component manages a parent-child relationship, as the child.
/// </summary>
public interface IRelationChild
{
    public EntityUid? Parent { get; set; }
}

