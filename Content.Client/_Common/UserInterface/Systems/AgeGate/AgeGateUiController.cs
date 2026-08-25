// SPDX-FileCopyrightText: Copyright (c) 2025 Space Wizards Federation
// SPDX-License-Identifier: MIT

using Content.Client._Common.AgeGate;
using Content.Shared._Common.AgeGate;
using Robust.Client.Console;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Client._Common.UserInterface.Systems.AgeGate;

/// <summary>
/// This controller is responsible for showing the Age Gate UI when the client recieves the ShowAgeGateMessage,
/// and for sending the AgeGateSubmittedMessage when the player clicks the Submit button.
/// </summary>
public sealed partial class AgeGateUiController : UIController
{
    [Dependency] private IClientConsoleHost _consoleHost = default!;
    [Dependency] private INetManager _netManager = default!;

    private AgeGatePopup? _ageGatePopup;

    protected override string SawmillName => "age_gate";

    public override void Initialize()
    {
        base.Initialize();

        _netManager.RegisterNetMessage<ShowAgeGateMessage>(OnShowAgeGateMessage);
    }

    private void OnShowAgeGateMessage(ShowAgeGateMessage message)
    {
        _ageGatePopup = new AgeGatePopup();

        _ageGatePopup.OnAgeGatePassed += OnAgeGatePassed;
        _ageGatePopup.OnAgeGateGated += OnAgeGateGated;
        _ageGatePopup.OnQuitPressed += OnQuitPressed;

        UIManager.WindowRoot.AddChild(_ageGatePopup);
        LayoutContainer.SetAnchorPreset(_ageGatePopup, LayoutContainer.LayoutPreset.Wide);
    }

    private void OnQuitPressed()
    {
        _consoleHost.ExecuteCommand("quit");
    }

    private void OnAgeGatePassed()
    {
        _netManager.ClientSendMessage(new AgeGateSubmittedMessage { IsAboveRequiredAge = true });
        _ageGatePopup?.Orphan();
        _ageGatePopup = null;
    }

    private void OnAgeGateGated()
    {
        _netManager.ClientSendMessage(new AgeGateSubmittedMessage { IsAboveRequiredAge = false });
        _ageGatePopup?.Orphan();
        _ageGatePopup = null;

        // Wait for the server to ban us...
    }
}
