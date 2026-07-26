// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
// This Source Code Form is "Incompatible With Secondary Licenses", as
// defined by the Mozilla Public License, v. 2.0.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Content.Server.Database._Citadel;

public sealed class CitadelPostgresContext(DbContextOptions<CitadelPostgresContext> options) : CitadelDbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        ((IDbContextOptionsBuilderInfrastructure) options).AddOrUpdateExtension(new SnakeCaseExtension());

        options.ConfigureWarnings(x =>
        {
            x.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
#if DEBUG
                // for tests
                x.Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning);
#endif
        });

#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach(var property in entity.GetProperties())
            {
                if (property.FieldInfo?.FieldType == typeof(DateTime) ||
                    property.FieldInfo?.FieldType == typeof(DateTime?))
                {
                    property.SetColumnType("timestamp with time zone");

                    if (property.FieldInfo!.GetCustomAttribute<DatabaseGeneratedAttribute>() is {} options)
                    {
                        if (options.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                        {
                            property.SetDefaultValueSql("current_timestamp()");
                        }
                    }
                }
            }
        }
    }
}
