using Content.Client.Pointing.Components;
<<<<<<< HEAD
using Content.Shared.MobState.EntitySystems;
=======
using Content.Client.Gravity;
using Content.Shared.Mobs.Systems;
>>>>>>> 67ed59a50 (Makes mobs visually float when weightless (#13391))
using Content.Shared.Pointing;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using DrawDepth = Content.Shared.DrawDepth.DrawDepth;

namespace Content.Client.Pointing;

public sealed class PointingSystem : SharedPointingSystem
{
<<<<<<< HEAD
    [Dependency] private readonly AnimationPlayerSystem _player = default!;
    [Dependency] private readonly SharedMobStateSystem _mobState = default!;

    private const string AnimationKey = "pointingarrow";

    /// <summary>
    /// How far it goes in any direction.
    /// </summary>
    private const float Offset = 0.25f;

    /// <summary>
    /// How long it takes to go from the bottom of the animation to the top.
    /// </summary>
    private const float UpTime = 0.5f;

    /// <summary>
    /// Starts at the bottom then goes up and comes back down. Seems to look nicer than starting in the middle.
    /// </summary>
    private static readonly Animation PointingAnimation = new Animation()
    {
        Length = TimeSpan.FromSeconds(2 * UpTime),
        AnimationTracks =
        {
            new AnimationTrackComponentProperty()
            {
                ComponentType = typeof(SpriteComponent),
                Property = nameof(SpriteComponent.Offset),
                InterpolationMode = AnimationInterpolationMode.Linear,
                KeyFrames =
                {
                    new AnimationTrackProperty.KeyFrame(Vector2.Zero, 0f),
                    new AnimationTrackProperty.KeyFrame(new Vector2(0f, Offset), UpTime),
                    new AnimationTrackProperty.KeyFrame(Vector2.Zero, UpTime),
                }
            }
        }
    };
=======
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly FloatingVisualizerSystem _floatingSystem = default!;
>>>>>>> 67ed59a50 (Makes mobs visually float when weightless (#13391))

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GetVerbsEvent<Verb>>(AddPointingVerb);
        SubscribeLocalEvent<PointingArrowComponent, ComponentStartup>(OnArrowStartup);
        SubscribeLocalEvent<PointingArrowComponent, AnimationCompletedEvent>(OnArrowAnimation);
        SubscribeLocalEvent<RoguePointingArrowComponent, ComponentStartup>(OnRogueArrowStartup);
    }

    private void OnArrowAnimation(EntityUid uid, PointingArrowComponent component, AnimationCompletedEvent args)
    {
        _floatingSystem.FloatAnimation(uid, component.Offset, component.AnimationKey, component.AnimationTime);
    }

    private void AddPointingVerb(GetVerbsEvent<Verb> args)
    {
        // Really this could probably be a properly predicted event, but that requires reworking pointing. For now
        // I'm just adding this verb exclusively to clients so that the verb-loading pop-in on the verb menu isn't
        // as bad. Important for this verb seeing as its usually an option on just about any entity.

        if (HasComp<PointingArrowComponent>(args.Target))
        {
            // this is a pointing arrow. no pointing here...
            return;
        }

        // Can the user point? Checking mob state directly instead of some action blocker, as many action blockers are blocked for
        // ghosts and there is no obvious choice for pointing (unless ghosts CanEmote?).
        if (_mobState.IsIncapacitated(args.User))
            return;

        // We won't check in range or visibility, as this verb is currently only executable via the context menu,
        // and that should already have checked that, as well as handling the FOV-toggle stuff.

        Verb verb = new()
        {
            Text = Loc.GetString("pointing-verb-get-data-text"),
            IconTexture = "/Textures/Interface/VerbIcons/point.svg.192dpi.png",
            ClientExclusive = true,
            Act = () => RaiseNetworkEvent(new PointingAttemptEvent(args.Target))
        };

        args.Verbs.Add(verb);
    }

    private void OnArrowStartup(EntityUid uid, PointingArrowComponent component, ComponentStartup args)
    {
        if (TryComp<SpriteComponent>(uid, out var sprite))
        {
            sprite.DrawDepth = (int) DrawDepth.Overlays;
        }

        _floatingSystem.FloatAnimation(uid, component.Offset, component.AnimationKey, component.AnimationTime);
    }

    private void OnRogueArrowStartup(EntityUid uid, RoguePointingArrowComponent arrow, ComponentStartup args)
    {
        if (TryComp<SpriteComponent>(uid, out var sprite))
        {
            sprite.DrawDepth = (int) DrawDepth.Overlays;
            sprite.NoRotation = false;
        }
    }
}
