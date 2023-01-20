using Content.Server.Doors.Components;
using Content.Server.Wires;
using Content.Shared.Doors;
using Content.Shared.Wires;

namespace Content.Server.Doors;

public sealed class DoorSafetyWireAction : ComponentWireAction<AirlockComponent>
{
<<<<<<< HEAD
    [DataField("color")]
    private Color _statusColor = Color.Red;

    [DataField("name")]
    private string _text = "SAFE";
=======
    public override Color Color { get; set; } = Color.Red;
    public override string Name { get; set; } = "wire-name-door-safety";
    
>>>>>>> b20b4b11c (Wire action cleanup (#13496))

    [DataField("timeout")]
    private int _timeout = 30;

    public override StatusLightState? GetLightState(Wire wire, AirlockComponent comp)
        => comp.Safety ? StatusLightState.On : StatusLightState.Off;

    public override object StatusKey { get; } = AirlockWireStatus.SafetyIndicator;

    public override bool Cut(EntityUid user, Wire wire, AirlockComponent door)
    {
<<<<<<< HEAD
        if (EntityManager.TryGetComponent<AirlockComponent>(wire.Owner, out var door))
        {
            WiresSystem.TryCancelWireAction(wire.Owner, PulseTimeoutKey.Key);
            door.Safety = false;
        }

=======
        WiresSystem.TryCancelWireAction(wire.Owner, PulseTimeoutKey.Key);
        EntityManager.System<SharedAirlockSystem>().SetSafety(door, false);
>>>>>>> b20b4b11c (Wire action cleanup (#13496))
        return true;
    }

    public override bool Mend(EntityUid user, Wire wire, AirlockComponent door)
    {
<<<<<<< HEAD
        if (EntityManager.TryGetComponent<AirlockComponent>(wire.Owner, out var door))
        {
            door.Safety = true;
        }

=======
        EntityManager.System<SharedAirlockSystem>().SetSafety(door, true);
>>>>>>> b20b4b11c (Wire action cleanup (#13496))
        return true;
    }

    public override void Pulse(EntityUid user, Wire wire, AirlockComponent door)
    {
<<<<<<< HEAD
        if (EntityManager.TryGetComponent<AirlockComponent>(wire.Owner, out var door))
        {
            door.Safety = false;
            WiresSystem.StartWireAction(wire.Owner, _timeout, PulseTimeoutKey.Key, new TimedWireEvent(AwaitSafetyTimerFinish, wire));
        }

        return true;
=======
        EntityManager.System<SharedAirlockSystem>().SetSafety(door, false);
        WiresSystem.StartWireAction(wire.Owner, _timeout, PulseTimeoutKey.Key, new TimedWireEvent(AwaitSafetyTimerFinish, wire));
>>>>>>> b20b4b11c (Wire action cleanup (#13496))
    }

    public override void Update(Wire wire)
    {
        if (!IsPowered(wire.Owner))
        {
            WiresSystem.TryCancelWireAction(wire.Owner, PulseTimeoutKey.Key);
        }
    }

    private void AwaitSafetyTimerFinish(Wire wire)
    {
        if (!wire.IsCut)
        {
            if (EntityManager.TryGetComponent<AirlockComponent>(wire.Owner, out var door))
            {
                door.Safety = true;
            }
        }
    }

    private enum PulseTimeoutKey : byte
    {
        Key
    }
}
