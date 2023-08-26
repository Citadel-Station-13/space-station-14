namespace Content.Server._Citadel.Contracts.Criteria.Components;

/// <summary>
/// This is used for the "don't die, dumbass" criteria.
/// </summary>
[RegisterComponent]
public sealed partial class CriteriaNoDeathsComponent : Component
{
    [DataField("description")]
    public string Description = "Letting any member of the team die.";

    public bool Ticking = false;
}
