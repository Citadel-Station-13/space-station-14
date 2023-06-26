using Content.Shared.Materials;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._Citadel.MiningContracts.Criteria.Components;

/// <summary>
/// This is used for the materials criteria, i.e. "get 10000 units of steel"
/// </summary>
[RegisterComponent]
public sealed class CriteriaMaterialStorageHasMaterialsComponent : Component
{
    [DataField("material", required: true, customTypeSerializer: typeof(PrototypeIdSerializer<MaterialPrototype>))]
    public string Material = default!;

    [DataField("amountRange", required: true)]
    public Vector2i AmountRange = default!;

    [DataField("amount")]
    public int Amount = 0;

    [DataField("ticking")]
    public bool Ticking = false;

    [DataField("satisfied")]
    public bool Satisfied = false;
}
