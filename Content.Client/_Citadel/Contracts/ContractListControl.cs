using Content.Shared._Citadel.Contracts.BUI;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Citadel.Contracts;

public sealed class ContractListControl : ScrollContainer
{
    private BoxContainer _list = new();
    private Dictionary<Guid, ContractInfoControl> _controls = new();

    public event Action<Guid>? OnStartContract;
    public event Action<Guid>? OnCancelContract;
    public event Action<Guid>? OnJoinContract;
    public event Action<Guid>? OnLeaveContract;
    public event Action<Guid>? OnInitiateContract;
    public event Action<Guid>? OnHailContract;

    public ContractListControl()
    {
        AddChild(_list);
        _list.Orientation = BoxContainer.LayoutOrientation.Vertical;
        _list.HorizontalExpand = false;
        HScrollEnabled = false;
        HorizontalExpand = false;
    }

    public void Update(IEnumerable<KeyValuePair<Guid, ContractUiState>> uiState)
    {
        _list.RemoveAllChildren();
        foreach (var (uuid, state) in uiState)
        {
            if (_controls.TryGetValue(uuid, out var control))
            {
                control.Update(state);
                _list.AddChild(control);
            }
            else
            {
                var newControl = new ContractInfoControl(state, uuid);
                newControl.OnStartContract += guid => OnStartContract?.Invoke(guid);
                newControl.OnCancelContract += guid => OnCancelContract?.Invoke(guid);
                newControl.OnJoinContract += guid => OnJoinContract?.Invoke(guid);
                newControl.OnLeaveContract += guid => OnLeaveContract?.Invoke(guid);
                newControl.OnStartContract += guid => OnStartContract?.Invoke(guid);
                newControl.OnInitiateContract += guid => OnInitiateContract?.Invoke(guid);
                newControl.OnHailContract += guid => OnHailContract?.Invoke(guid);
                newControl.Name = uuid.ToString();
                _list.AddChild(newControl);
                _controls.Add(uuid, newControl);
            }
        }
    }
}
