using Content.Server.Wires;
using Content.Shared.VendingMachines;
using Content.Shared.Wires;

namespace Content.Server.VendingMachines;

public sealed class VendingMachineEjectItemWireAction : ComponentWireAction<VendingMachineComponent>
{
    private VendingMachineSystem _vendingMachineSystem = default!;

<<<<<<< HEAD
    private Color _color = Color.Red;
    private string _text = "VEND";
=======
    public override Color Color { get; set; } = Color.Red;
    public override string Name { get; set; } = "wire-name-vending-eject";

>>>>>>> b20b4b11c (Wire action cleanup (#13496))
    public override object? StatusKey { get; } = EjectWireKey.StatusKey;

    public override StatusLightState? GetLightState(Wire wire, VendingMachineComponent comp)
        => comp.CanShoot ? StatusLightState.BlinkingFast : StatusLightState.On;

    public override void Initialize()
    {
        base.Initialize();

        _vendingMachineSystem = EntityManager.System<VendingMachineSystem>();
    }

    public override bool Cut(EntityUid user, Wire wire, VendingMachineComponent vending)
    {
<<<<<<< HEAD
        if (EntityManager.TryGetComponent(wire.Owner, out VendingMachineComponent? vending))
        {
            _vendingMachineSystem.SetShooting(wire.Owner, true, vending);
        }

=======
        _vendingMachineSystem.SetShooting(wire.Owner, true, vending);
>>>>>>> b20b4b11c (Wire action cleanup (#13496))
        return true;
    }

    public override bool Mend(EntityUid user, Wire wire, VendingMachineComponent vending)
    {
<<<<<<< HEAD
        if (EntityManager.TryGetComponent(wire.Owner, out VendingMachineComponent? vending))
        {
            _vendingMachineSystem.SetShooting(wire.Owner, false, vending);
        }

=======
        _vendingMachineSystem.SetShooting(wire.Owner, false, vending);
>>>>>>> b20b4b11c (Wire action cleanup (#13496))
        return true;
    }

    public override void Pulse(EntityUid user, Wire wire, VendingMachineComponent vending)
    {
<<<<<<< HEAD
        _vendingMachineSystem.EjectRandom(wire.Owner, true);

        return true;
=======
        _vendingMachineSystem.EjectRandom(wire.Owner, true, vendComponent: vending);
>>>>>>> b20b4b11c (Wire action cleanup (#13496))
    }
}
