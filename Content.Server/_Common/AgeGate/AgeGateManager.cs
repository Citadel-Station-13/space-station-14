// SPDX-FileCopyrightText: Copyright (c) 2025 Space Wizards Federation
// SPDX-License-Identifier: MIT

using Content.Server.Administration.Managers;
using Content.Server.Database;
using Content.Server._Common.PreJoin;
using Content.Shared._Common.AgeGate;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System.Threading.Tasks;

namespace Content.Server._Common.AgeGate;

public sealed class AgeGateManager : IPreJoinAction
{
    [Dependency] private readonly IServerDbManager _dbManager = default!;
    [Dependency] private readonly INetManager _netManager = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;
    [Dependency] private readonly IBanManager _banManager = default!;
    [Dependency] private readonly PreJoinManager _preJoinManager = default!;

    private static DateTime LastValidReadTime => DateTime.UtcNow - TimeSpan.FromDays(60);

    public void Initialize()
    {
        _preJoinManager.RegisterPreJoinAction("AgeGate", this);
        _netManager.RegisterNetMessage<ShowAgeGateMessage>();
        _netManager.RegisterNetMessage<AgeGateSubmittedMessage>(OnAgeGateSubmitted);
    }

    public async Task<bool> PreJoinAction(ICommonSession session)
    {
        // Skip the Age Gate if the player has already done it before.
        if (await _dbManager.HasPassedAgeGate(session.Channel.UserId))
        {
            return true;
        }

        session.Channel.SendMessage(new ShowAgeGateMessage());
        return false;
    }

    private async void OnAgeGateSubmitted(AgeGateSubmittedMessage message)
    {
        if (message.IsAboveRequiredAge)
        {
            await _dbManager.SetPassedAgeGate(message.MsgChannel.UserId, hasPassed: true);
            var session = _playerManager.GetSessionByChannel(message.MsgChannel);
            await _preJoinManager.TryJoinGame(session);
        }
        else
        {
            _banManager.CreateServerBan(
                message.MsgChannel.UserData.UserId,
                null,
                null,
                null,
                null,
                null, // Perma ban
                Shared.Database.NoteSeverity.High,
                Loc.GetString("agegate-ban-reason")
            );
        }
    }
}
