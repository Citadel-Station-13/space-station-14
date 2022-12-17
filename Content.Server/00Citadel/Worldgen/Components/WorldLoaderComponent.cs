namespace Content.Server._00OuterRim.Worldgen2.Components;

/// <summary>
/// This is used for allowing some objects to load the game world.
/// </summary>
[RegisterComponent]
public sealed class WorldLoaderComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("radius")]
    public int Radius = 128;
}
