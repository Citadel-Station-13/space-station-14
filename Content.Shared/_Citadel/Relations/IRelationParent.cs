namespace Content.Shared._Citadel.Relations;

/// <summary>
///     Indicates a component manages a parent-child relationship, as the parent.
/// </summary>
public interface IRelationParent
{
    public HashSet<EntityUid> Children { get; }
}

