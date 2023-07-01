namespace Content.Server._Citadel.Contracts.Components;

/// <summary>
/// This is used for contracts that may have subcontracts, including controlling the selection of new contracts.
/// </summary>
[RegisterComponent]
public sealed class SubcontractingComponent : Component
{
    [DataField("categories", required: true)]
    public List<string> Categories = default!;

    public List<EntityUid> ActiveSubcontracts = new();
}
