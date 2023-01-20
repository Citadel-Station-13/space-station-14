<<<<<<< HEAD
using Content.Server.Doors.Components;
=======
using Content.Server.Doors.Systems;
>>>>>>> b20b4b11c (Wire action cleanup (#13496))
using Content.Server.Wires;
using Content.Shared.Doors;
using Content.Shared.Wires;

namespace Content.Server.Doors;

public sealed class DoorBoltWireAction : ComponentWireAction<AirlockComponent>
{
<<<<<<< HEAD
    [DataField("color")]
    private Color _statusColor = Color.Red;

    [DataField("name")]
    private string _text = "BOLT";

    public override StatusLightData? GetStatusLightData(Wire wire)
    {
        StatusLightState lightState = StatusLightState.Off;
        if (IsPowered(wire.Owner)
            && EntityManager.TryGetComponent<AirlockComponent>(wire.Owner, out var door))
        {
            if (door.BoltsDown)
            {
                lightState = StatusLightState.On;
            }
        }

        return new StatusLightData(
            _statusColor,
            lightState,
            _text);
    }
=======
    public override Color Color { get; set; } = Color.Red;
    public override string Name { get; set; } = "wire-name-door-bolt";
    
    public override StatusLightState? GetLightState(Wire wire, AirlockComponent comp)
        => comp.BoltsDown ? StatusLightState.On : StatusLightState.Off;
>>>>>>> b20b4b11c (Wire action cleanup (#13496))

    public override object StatusKey { get; } = AirlockWireStatus.BoltIndicator;

    public override bool Cut(EntityUid user, Wire wire, AirlockComponent airlock)
    {
<<<<<<< HEAD
        if (EntityManager.TryGetComponent<AirlockComponent>(wire.Owner, out var door))
        {
            door.BoltWireCut = true;
            if (!door.BoltsDown && IsPowered(wire.Owner))
                door.SetBoltsWithAudio(true);
        }
=======
        EntityManager.System<SharedAirlockSystem>().SetBoltWireCut(airlock, true);
        if (!airlock.BoltsDown && IsPowered(wire.Owner))
            EntityManager.System<AirlockSystem>().SetBoltsWithAudio(wire.Owner, airlock, true);
>>>>>>> b20b4b11c (Wire action cleanup (#13496))

        return true;
    }

    public override bool Mend(EntityUid user, Wire wire, AirlockComponent door)
    {
<<<<<<< HEAD
        if (EntityManager.TryGetComponent<AirlockComponent>(wire.Owner, out var door))
            door.BoltWireCut = false;

=======
        EntityManager.System<SharedAirlockSystem>().SetBoltWireCut(door, true);
>>>>>>> b20b4b11c (Wire action cleanup (#13496))
        return true;
    }

    public override void Pulse(EntityUid user, Wire wire, AirlockComponent door)
    {
<<<<<<< HEAD
        if (EntityManager.TryGetComponent<AirlockComponent>(wire.Owner, out var door))
        {
            if (IsPowered(wire.Owner))
            {
                door.SetBoltsWithAudio(!door.BoltsDown);
            }
            else if (!door.BoltsDown)
            {
                door.SetBoltsWithAudio(true);
            }

        }

        return true;
=======
        if (IsPowered(wire.Owner))
            EntityManager.System<AirlockSystem>().SetBoltsWithAudio(wire.Owner, door, !door.BoltsDown);
        else if (!door.BoltsDown)
            EntityManager.System<AirlockSystem>().SetBoltsWithAudio(wire.Owner, door, true);
>>>>>>> b20b4b11c (Wire action cleanup (#13496))
    }
}
