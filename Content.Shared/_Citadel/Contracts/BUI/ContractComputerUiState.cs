using Robust.Shared.Serialization;

namespace Content.Shared._Citadel.Contracts.BUI;

/// <summary>
///
/// </summary>
[Serializable, NetSerializable]
public sealed class ContractListUiState
{

}

public sealed class ContractUiState
{
    public required Dictionary<string, List<CriteriaDisplayData>> Criteria;

    public required string Description;
}

public record struct CriteriaDisplayData(string Description);
