namespace Content.Server._00OuterRim.Worldgen2.Components;

/// <summary>
/// This is used for sending a signal to the entity it's on to load contents whenever a loader gets close enough.
/// Does not support unloading.
/// </summary>
[RegisterComponent]
public sealed class LocalityLoaderComponent : Component
{
    [DataField("loadingDistance")] public int LoadingDistance = 32;
}
