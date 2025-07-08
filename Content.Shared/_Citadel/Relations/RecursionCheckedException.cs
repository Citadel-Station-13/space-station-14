namespace Content.Shared._Citadel.Relations;

/// <summary>
///     An exception thrown when a <see cref="FamilyEntitySystem{TChild,TParent}"/> runs into potentially recursive operations.
/// </summary>
/// <param name="recursingParent"></param>
public sealed class RecursionCheckedException(EntityUid recursingParent) : Exception
{
    public override string Message =>
        $"Recursion check tripped by ${recursingParent}.";
}
