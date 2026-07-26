// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Content.Server.Database._Citadel.Model.Dto;

/// <summary>
///     A subset of the Player table for use in CitadelDbContext.
/// </summary>
[Table("player", Schema = ServerDbContext.DEFAULT_SCHEMA)]
[PrimaryKey(nameof(Id))]
public sealed class PlayerDto
{
    [Key]
    public int Id { get; set; }

    [Key]
    public Guid UserId { get; set; }
}
