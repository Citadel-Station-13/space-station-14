using Content.Client.UserInterface.Fragments;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Content.Shared._Citadel.Contracts.BUI;
using Content.Shared.CartridgeLoader;

namespace Content.Client._Citadel.Contracts;

public sealed partial class ContractsCartridgeUi : UIFragment
{
    private ContractsCartridgeControl? _uiFragment;

    public override Control GetUIFragmentRoot()
    {
        return _uiFragment!;
    }

    public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
    {
        _uiFragment = new ContractsCartridgeControl();
        _uiFragment.OnStartContract += guid => SendMessage(ContractsUiMessageEvent.ContractAction.Start, guid, userInterface);
        _uiFragment.OnJoinContract += guid => SendMessage(ContractsUiMessageEvent.ContractAction.Join, guid, userInterface);
        _uiFragment.OnCancelContract += guid => SendMessage(ContractsUiMessageEvent.ContractAction.Cancel, guid, userInterface);
        _uiFragment.OnInitiateContract += guid => SendMessage(ContractsUiMessageEvent.ContractAction.Sign, guid, userInterface);
        _uiFragment.OnHailContract += guid => SendMessage(ContractsUiMessageEvent.ContractAction.Hail, guid, userInterface);
        _uiFragment.OnLeaveContract += guid => SendMessage(ContractsUiMessageEvent.ContractAction.Leave, guid, userInterface);
    }

    public override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is not ContractCartridgeUiState s)
            throw new NotImplementedException($"Got unknown BUI state {state.GetType()}");

        _uiFragment!.Update(s);
    }

    private void SendMessage(ContractsUiMessageEvent.ContractAction action, Guid contract, BoundUserInterface userInterface)
    {
        var notekeeperMessage = new ContractsUiMessageEvent(action, contract);
        var message = new CartridgeUiMessage(notekeeperMessage);
        userInterface.SendMessage(message);
    }
}
