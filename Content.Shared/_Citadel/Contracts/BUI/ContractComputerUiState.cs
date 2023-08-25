using Content.Shared.CartridgeLoader;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._Citadel.Contracts.BUI;

/// <summary>
///
/// </summary>
[Serializable, NetSerializable]
public sealed class ContractListUiState
{
    public Dictionary<Guid, ContractUiState> Contracts;
    public FixedPoint2 BankAccount;

    public ContractListUiState(Dictionary<Guid, ContractUiState> contracts, FixedPoint2 bankAccount)
    {
        Contracts = contracts;
        BankAccount = bankAccount;
    }
}

[NetSerializable, Serializable]
public sealed record ContractUiState(ContractUiState.ContractUserStatus UserStatus, string Name, string Owner, List<string> Subcontractors, ContractDisplayData Data, ContractStatus Status )
{
    public ContractUserStatus UserStatus = UserStatus;
    public ContractStatus Status = Status;
    public string Name = Name;
    public string Owner = Owner;
    public List<string> Subcontractors = Subcontractors;
    public Dictionary<string, List<CriteriaDisplayData>> Criteria = new();
    public Dictionary<string, List<FormattedMessage>> Effects = new();
    public bool Startable = true;
    public FormattedMessage? NoStartReason = null;

    public ContractDisplayData Data = Data;

    public enum ContractUserStatus
    {
        Owner,
        Subcontractor,
        OpenToJoin,
        OpenToOwn,
    }
}

[NetSerializable, Serializable]
public record struct CriteriaDisplayData(FormattedMessage Description);

[NetSerializable, Serializable]
public record struct ContractDisplayData(FormattedMessage Description);

[NetSerializable, Serializable]
public sealed class ContractCartridgeUiState : BoundUserInterfaceState
{
    public ContractListUiState Contracts;

    public ContractCartridgeUiState(ContractListUiState contracts)
    {
        Contracts = contracts;
    }
}

[Serializable, NetSerializable]
public sealed class ContractsUiMessageEvent : CartridgeMessageEvent
{
    public ContractAction Action;
    public Guid Contract;

    public ContractsUiMessageEvent(ContractAction action, Guid contract)
    {
        Action = action;
        Contract = contract;
    }

    public enum ContractAction
    {
        Sign,
        Join,
        Cancel,
        Leave,
        Start,
        Hail
    }
}
