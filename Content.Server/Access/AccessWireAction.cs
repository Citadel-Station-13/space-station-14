using Content.Server.Wires;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Wires;

namespace Content.Server.Access;

public sealed class AccessWireAction : ComponentWireAction<AccessReaderComponent>
{
<<<<<<< HEAD
    [DataField("color")]
    private Color _statusColor = Color.Green;

    [DataField("name")]
    private string _text = "ACC";
=======
    public override Color Color { get; set; } = Color.Green;
    public override string Name { get; set; } = "wire-name-access";
>>>>>>> b20b4b11c (Wire action cleanup (#13496))

    [DataField("pulseTimeout")]
    private int _pulseTimeout = 30;

    public override StatusLightState? GetLightState(Wire wire, AccessReaderComponent comp)
        => comp.Enabled ? StatusLightState.On : StatusLightState.Off;

    public override object StatusKey { get; } = AccessWireActionKey.Status;

    public override bool Cut(EntityUid user, Wire wire, AccessReaderComponent comp)
    {
<<<<<<< HEAD
        if (EntityManager.TryGetComponent<AccessReaderComponent>(wire.Owner, out var access))
        {
            WiresSystem.TryCancelWireAction(wire.Owner, PulseTimeoutKey.Key);
            access.Enabled = false;
        }

=======
        WiresSystem.TryCancelWireAction(wire.Owner, PulseTimeoutKey.Key);
        comp.Enabled = false;
>>>>>>> b20b4b11c (Wire action cleanup (#13496))
        return true;
    }

    public override bool Mend(EntityUid user, Wire wire, AccessReaderComponent comp)
    {
<<<<<<< HEAD
        if (EntityManager.TryGetComponent<AccessReaderComponent>(wire.Owner, out var access))
        {
            access.Enabled = true;
        }

=======
        comp.Enabled = true;
>>>>>>> b20b4b11c (Wire action cleanup (#13496))
        return true;
    }

    public override void Pulse(EntityUid user, Wire wire, AccessReaderComponent comp)
    {
<<<<<<< HEAD
        if (EntityManager.TryGetComponent<AccessReaderComponent>(wire.Owner, out var access))
        {
            access.Enabled = false;
            WiresSystem.StartWireAction(wire.Owner, _pulseTimeout, PulseTimeoutKey.Key, new TimedWireEvent(AwaitPulseCancel, wire));
        }

        return true;
=======
        comp.Enabled = false;
        WiresSystem.StartWireAction(wire.Owner, _pulseTimeout, PulseTimeoutKey.Key, new TimedWireEvent(AwaitPulseCancel, wire));
>>>>>>> b20b4b11c (Wire action cleanup (#13496))
    }

    public override void Update(Wire wire)
    {
        if (!IsPowered(wire.Owner))
        {
            WiresSystem.TryCancelWireAction(wire.Owner, PulseTimeoutKey.Key);
        }
    }

    private void AwaitPulseCancel(Wire wire)
    {
        if (!wire.IsCut)
        {
            if (EntityManager.TryGetComponent<AccessReaderComponent>(wire.Owner, out var access))
            {
                access.Enabled = true;
            }
        }
    }

    private enum PulseTimeoutKey : byte
    {
        Key
    }
}
