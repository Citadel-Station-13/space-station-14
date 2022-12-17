using Content.Server._00OuterRim.Worldgen2.Systems.Debris;

namespace Content.Server._00OuterRim.Worldgen2.Components.Debris;

/// <summary>
/// This is used for attaching a piece of debris to it's owning controller.
/// Mostly just syncs deletion.
/// </summary>
[RegisterComponent, Access(typeof(DebrisFeaturePlacerSystem))]
public sealed class OwnedDebrisComponent : Component
{
    public EntityUid OwningController;
    public Vector2 LastKey;
}
