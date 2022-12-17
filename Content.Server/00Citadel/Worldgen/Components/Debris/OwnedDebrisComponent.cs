using Content.Server._00Citadel.Worldgen.Systems.Debris;

namespace Content.Server._00Citadel.Worldgen.Components.Debris;

/// <summary>
/// This is used for attaching a piece of debris to it's owning controller.
/// Mostly just syncs deletion.
/// </summary>
[RegisterComponent, Access(typeof(DebrisFeaturePlacerSystem))]
public sealed class OwnedDebrisComponent : Component
{
    [DataField("owningController")]
    public EntityUid OwningController;

    [DataField("lastKey")]
    public Vector2 LastKey;
}
