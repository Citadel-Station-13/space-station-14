// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Content.Server.Database._Citadel.Model.Persistence;


/// <summary>
///     A persisted world object is the superclass
/// </summary>
[PrimaryKey(nameof(Id))]
public abstract class PersistedWorldObject
{
    /// <summary>
    ///     Primary key and id used in game and in db.
    /// </summary>
    /// <remarks>
    ///     We generate these on the game server as UUIDv7, the chance of conflict basically does not exist.
    /// </remarks>
    public required Guid Id { get; set; }

    /// <summary>
    ///     The "true name" of an object, used for in-game display.
    /// </summary>
    [StringLength(250)]
    public required string TrueName { get; set; }

    /// <summary>
    ///     Notes tied to this PWO, for administrative use.
    /// </summary>
    public List<string> Notes { get; set; } = new();

    /// <summary>
    ///     The time of this PWO being created.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime Created { get; set; }

    /// <summary>
    ///     The time of last modification.
    /// </summary>
    public DateTime LastModified { get; set; }
}
