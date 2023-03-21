using Robust.Shared.Prototypes;

namespace Content.Server._Citadel.Contracts.Prototypes;

/// <summary>
/// This is a prototype for a group of criteria.
/// </summary>
[Prototype("citadelCriteriaGroup")]
public sealed class CriteriaGroupPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    [DataField("name", required: true)]
    public string Name { get; } = default!;

    [DataField("mode", required: true)]
    public CriteriaGroupMode Mode { get; } = default!;

    [DataField("effects", required: true)]
    public List<CriteriaGroupEffectEvent> Effects { get; } = default!;
}

public enum CriteriaGroupMode
{
    All,
    Any
}
