namespace Content.Server._Citadel.Contracts.Criteria.Components;

/// <summary>
/// This is used for a criteria an admin must manually satisfy.
/// </summary>
[RegisterComponent]
public sealed class CriteriaManualComponent : Component
{
    [DataField("desc", required: true)]
    public string Description = default!;
}
