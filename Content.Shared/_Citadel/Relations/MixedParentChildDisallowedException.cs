namespace Content.Shared._Citadel.Relations;

public sealed class MixedParentChildDisallowedException(Type child, Type parent) : Exception
{
    public override string Message =>
        $"Mixing Parent and Child components on the same entity is disallowed for {child} and {parent}";
}
