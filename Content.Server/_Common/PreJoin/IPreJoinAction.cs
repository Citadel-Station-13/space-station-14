// SPDX-FileCopyrightText: Copyright (c) 2025 Space Wizards Federation
// SPDX-License-Identifier: MIT

using Robust.Shared.Player;
using System.Threading.Tasks;

namespace Content.Server._Common.PreJoin;

/// <summary>
/// An interface for managers with a PreJoin action.
/// </summary>
public interface IPreJoinAction
{
    /// <summary>
    /// Called when a player tries to join the game, if the PreJoinAction is registered with a key that is in the
    /// prejoin.order cvar.
    /// </summary
    /// <returns>
    /// A bool that is true if the action has been skipped or already completed (letting the player join the game if
    /// there are no more actions) or false if the action is in progress (for example, waiting for the player to accept
    /// the server rules).
    /// </returns>
    public Task<bool> PreJoinAction(ICommonSession session);
}
