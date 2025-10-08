// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using Content.Server.Database._Citadel.Model.Dto;
using Microsoft.EntityFrameworkCore;

namespace Content.Server.Database._Citadel.Model.Persistence;

/// <summary>
///     A persisted character, potentially owned by a specific player.
/// </summary>
public class PersistedCharacter : PersistedWorldObject
{
    /// <summary>
    ///     The number of times this character has died as far as the game is concerned.
    ///     This means "dead at round end".
    /// </summary>
    public int DeathCount { get; set; } = 0;

    /// <summary>
    ///     The player this character belongs to, if any.
    /// </summary>
    public PlayerDto? Player { get; set; }
}
