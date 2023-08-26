namespace Content.Server._Citadel.Contracts.Components;

/// <summary>
/// This is used for contract groups. simpl.
/// </summary>
[RegisterComponent]
public sealed partial class ContractGroupsComponent : Component
{
    [DataField("groups")]
    public HashSet<string> Groups = new();
}
