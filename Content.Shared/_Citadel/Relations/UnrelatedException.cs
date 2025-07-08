namespace Content.Shared._Citadel.Relations;

/// <summary>
///     An exception thrown when <see cref="FamilyEntitySystem{TChild,TParent}"/> expects two entities to be related and they are not.
/// </summary>
public sealed class UnrelatedException(EntityUid left, EntityUid right) : Exception
{
    public override string Message => $"{left} and {right} are unrelated when they were expected to be related.";
}
