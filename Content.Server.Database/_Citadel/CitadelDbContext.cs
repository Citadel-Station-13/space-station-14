// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

using System;
using Content.Server.Database._Citadel.Model.Dto;
using Content.Server.Database._Citadel.Model.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Content.Server.Database._Citadel;

public abstract class CitadelDbContext(DbContextOptions options) : DbContext(options)
{
    public const string DEFAULT_SCHEMA = "citadel";

    public DbSet<PersistedWorldObject> PersistedWorldObjects { get; set; }
    public DbSet<PersistedCharacter> PersistedCharacters { get; set; }

    /// <summary>
    /// View of the main ServerDbContext players table.
    /// </summary>
    public DbSet<PlayerDto> Players { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DEFAULT_SCHEMA);

        // Our views into the main schema.
        modelBuilder.Entity<PlayerDto>()
            .ToTable(t => t.ExcludeFromMigrations());

    }
}


