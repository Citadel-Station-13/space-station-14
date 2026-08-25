// SPDX-FileCopyrightText: Copyright (c) 2025 Space Wizards Federation
// SPDX-License-Identifier: MIT

using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Player;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Server._Common.PreJoin;

/// <summary>
/// This manager is responsible for letting the player join the game (sending them to the lobby), but only after each
/// IPreJoinAction has been sucessfully performed (for example, showing the rules popup and waiting for the player to
/// accept).
/// </summary>
public sealed partial class PreJoinManager
{
    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private IConfigurationManager _configurationManager = default!;
    [Dependency] private ILogManager _logManager = default!;

    private ISawmill _sawmill = default!;

    private Dictionary<string, IPreJoinAction> PreJoinActions = new ();

    public void Initialize()
    {
        _sawmill = _logManager.GetSawmill("prejoin");
    }

    /// <summary>
    /// Register a prejoin action. Managers that implement IPreJoinAction should call this in their Initialize().
    /// </summary>
    /// <param name="key">A string key used in the prejoin.order cvar. It should be the name of the manager without the -Manager part.</param>
    public void RegisterPreJoinAction(string key, IPreJoinAction manager)
    {
        PreJoinActions.Add(key, manager);
    }

    /// <summary>
    /// Either perform the next prejoin action, or join the game if there are no actions left to perform.
    /// </summary>
    public async Task<bool> TryJoinGame(ICommonSession session)
    {
        if (session.Status != SessionStatus.Connected)
        {
            return false;
        }

        var actionOrder = _configurationManager.GetCVar<string>("prejoin.order")
            .Split(',')
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();

        foreach (var actionKey in actionOrder) {
            _sawmill.Debug($"action: {actionKey}");
            if (!PreJoinActions.TryGetValue(actionKey, out var manager))
            {
                _sawmill.Error($"Tried to access prejoin action \"{actionKey}\" which hasn't been registered.");
                throw new Exception("PreJoin action not registered: " + actionKey);
            }

            if (!await manager.PreJoinAction(session))
            {
                return false;
            }
        }

        _sawmill.Debug($"All prejoin actions completed for player {session.UserId}.");
        _playerManager.JoinGame(session);
        return true;
    }
}
