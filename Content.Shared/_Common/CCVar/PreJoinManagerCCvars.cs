// SPDX-FileCopyrightText: Copyright (c) 2025 Space Wizards Federation
// SPDX-License-Identifier: MIT

using Robust.Shared.Configuration;

namespace Content.Shared._Common.CCVar;

[CVarDefs]
public sealed class PreJoinCCVars
{
    /// <summary>
    /// Comma-separated list of PreJoin actions to perform before the player is let into the game.
    /// </summary>
    public static readonly CVarDef<string> PreJoinOrder =
        CVarDef.Create("prejoin.order", "Rules", CVar.SERVERONLY);
}
