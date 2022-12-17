using Content.Server._00Citadel.Worldgen.Systems;

namespace Content.Server._00Citadel.Worldgen.Components;

/// <summary>
/// This is used for sending a signal to the entity it's on to load contents whenever a loader gets close enough.
/// Does not support unloading.
/// </summary>
[RegisterComponent, Access(typeof(LocalityLoaderSystem))]
public sealed class LocalityLoaderComponent : Component
{
    [DataField("loadingDistance")]
    public int LoadingDistance = 32;
}
