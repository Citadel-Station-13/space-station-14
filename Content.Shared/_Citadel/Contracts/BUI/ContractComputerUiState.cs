using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._Citadel.Contracts.BUI;

/// <summary>
///
/// </summary>
[Serializable, NetSerializable]
public sealed class ContractListUiState
{
    public List<ContractUiState> Contracts = new();


}

[NetSerializable, Serializable]
public sealed class ContractUiState
{
    public Dictionary<string, List<CriteriaDisplayData>> Criteria = new();

    public ContractDisplayData Data;
}

[NetSerializable, Serializable]
public record struct CriteriaDisplayData(string Description);

[NetSerializable, Serializable]
public record struct ContractDisplayData(FormattedMessage Description);
