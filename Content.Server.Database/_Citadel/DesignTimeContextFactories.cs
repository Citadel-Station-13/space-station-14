#if TOOLS

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SQLitePCL;

namespace Content.Server.Database._Citadel;

public sealed class DesignTimeContextFactoryPostgres : IDesignTimeDbContextFactory<CitadelPostgresContext>
{
    public CitadelPostgresContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CitadelPostgresContext>();
        optionsBuilder.UseNpgsql("Server=localhost");
        return new CitadelPostgresContext(optionsBuilder.Options);
    }
}

public sealed class DesignTimeContextFactorySqlite : IDesignTimeDbContextFactory<CitadelSqliteContext>
{
    public CitadelSqliteContext CreateDbContext(string[] args)
    {
#if !USE_SYSTEM_SQLITE
        raw.SetProvider(new SQLite3Provider_e_sqlite3());
#endif

        var optionsBuilder = new DbContextOptionsBuilder<CitadelSqliteContext>();
        optionsBuilder.UseSqlite("Data Source=:memory:");
        return new CitadelSqliteContext(optionsBuilder.Options);
    }
}

#endif
