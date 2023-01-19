using Content.Shared.Doors.Components;
using Robust.Shared.GameStates;

namespace Content.Shared.Doors.Systems;

public abstract class SharedAirlockSystem : EntitySystem
{
    [Dependency] protected readonly SharedAppearanceSystem Appearance = default!;
    [Dependency] protected readonly SharedDoorSystem DoorSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SharedAirlockComponent, ComponentGetState>(OnGetState);
        SubscribeLocalEvent<SharedAirlockComponent, ComponentHandleState>(OnHandleState);
        SubscribeLocalEvent<SharedAirlockComponent, BeforeDoorClosedEvent>(OnBeforeDoorClosed);
    }

    private void OnGetState(EntityUid uid, SharedAirlockComponent airlock, ref ComponentGetState args)
    {
        // Need to network airlock safety state to avoid mis-predicts when a door auto-closes as the client walks through the door.
        args.State = new AirlockComponentState(airlock.Safety);
    }

    private void OnHandleState(EntityUid uid, SharedAirlockComponent airlock, ref ComponentHandleState args)
    {
        if (args.Current is not AirlockComponentState state)
            return;

        airlock.Safety = state.Safety;
    }

    protected virtual void OnBeforeDoorClosed(EntityUid uid, SharedAirlockComponent airlock, BeforeDoorClosedEvent args)
    {
        if (!airlock.Safety)
            args.PerformCollisionCheck = false;
    }


    public void UpdateEmergencyLightStatus(SharedAirlockComponent component)
    {
        Appearance.SetData(component.Owner, DoorVisuals.EmergencyLights, component.EmergencyAccess);
    }

    public void ToggleEmergencyAccess(SharedAirlockComponent component)
    {
        component.EmergencyAccess = !component.EmergencyAccess;
<<<<<<< HEAD
        UpdateEmergencyLightStatus(component);
=======
        UpdateEmergencyLightStatus(uid, component);
    }

    public void SetAutoCloseDelayModifier(AirlockComponent component, float value)
    {
        if (component.AutoCloseDelayModifier.Equals(value))
            return;

        component.AutoCloseDelayModifier = value;
    }

    public void SetSafety(AirlockComponent component, bool value)
    {
        component.Safety = value;
    }

    public void SetBoltWireCut(AirlockComponent component, bool value)
    {
        component.BoltWireCut = value;
>>>>>>> c6d3e4f3b (Fix warnings and code cleanup/fixes (#13570))
    }
}
