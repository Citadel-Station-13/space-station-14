namespace Content.Server._Citadel.Contracts.Criteria.Components;

/// <summary>
/// This is used for the "don't die, dumbass" criteria.
/// </summary>
[RegisterComponent]
public sealed class CriteriaNoDeathsComponent : Component
{
    [DataField("description")]
    public string Description = "Fulfilled by any member of the contracting team dying.";

    public bool Ticking = false;
}
