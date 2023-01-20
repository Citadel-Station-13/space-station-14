<<<<<<< HEAD
using Content.Server.Doors.Components;
=======
using Content.Server.Doors.Systems;
>>>>>>> b20b4b11c (Wire action cleanup (#13496))
using Content.Server.Wires;
using Content.Shared.Doors;
using Content.Shared.Wires;

namespace Content.Server.Doors;

public sealed class DoorBoltLightWireAction : ComponentWireAction<AirlockComponent>
{
    public override Color Color { get; set; } = Color.Lime;
    public override string Name { get; set; } = "wire-name-bolt-light";

<<<<<<< HEAD
    [DataField("name")]
    private string _text = "BLIT";

    public override StatusLightData? GetStatusLightData(Wire wire)
    {
        StatusLightState lightState = StatusLightState.Off;
        if (IsPowered(wire.Owner) && EntityManager.TryGetComponent<AirlockComponent>(wire.Owner, out var door))
        {
            lightState = door.BoltLightsEnabled
                ? StatusLightState.On
                : StatusLightState.Off;
        }

        return new StatusLightData(
            _statusColor,
            lightState,
            _text);
    }
=======
    public override StatusLightState? GetLightState(Wire wire, AirlockComponent comp)
        => comp.BoltLightsEnabled ? StatusLightState.On : StatusLightState.Off;
>>>>>>> b20b4b11c (Wire action cleanup (#13496))

    public override object StatusKey { get; } = AirlockWireStatus.BoltLightIndicator;

    public override bool Cut(EntityUid user, Wire wire, AirlockComponent door)
    {
<<<<<<< HEAD
        if (EntityManager.TryGetComponent<AirlockComponent>(wire.Owner, out var door))
        {
            door.BoltLightsVisible = false;
        }

=======
        EntityManager.System<AirlockSystem>().SetBoltLightsEnabled(wire.Owner, door, false);
>>>>>>> b20b4b11c (Wire action cleanup (#13496))
        return true;
    }

    public override bool Mend(EntityUid user, Wire wire, AirlockComponent door)
    {
<<<<<<< HEAD
        if (EntityManager.TryGetComponent<AirlockComponent>(wire.Owner, out var door))
        {
            door.BoltLightsVisible = true;
        }
=======
>>>>>>> b20b4b11c (Wire action cleanup (#13496))

        EntityManager.System<AirlockSystem>().SetBoltLightsEnabled(wire.Owner, door, true);
        return true;
    }

    public override void Pulse(EntityUid user, Wire wire, AirlockComponent door)
    {
<<<<<<< HEAD
        if (EntityManager.TryGetComponent<AirlockComponent>(wire.Owner, out var door))
        {
            door.BoltLightsVisible = !door.BoltLightsEnabled;
        }

        return true;
=======
        EntityManager.System<AirlockSystem>().SetBoltLightsEnabled(wire.Owner, door, !door.BoltLightsEnabled);
>>>>>>> b20b4b11c (Wire action cleanup (#13496))
    }
}
