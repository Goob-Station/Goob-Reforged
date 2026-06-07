// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

#if TOOLS

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SQLitePCL;

namespace Content.GoobStation.Server.Database;

public sealed class GoobStationDesignTimeContextFactoryPostgres : IDesignTimeDbContextFactory<GoobStationPostgresServerDbContext>
{
    public GoobStationPostgresServerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GoobStationPostgresServerDbContext>();
        optionsBuilder.UseNpgsql("Server=localhost", npgsqlOptions =>
            npgsqlOptions.MigrationsHistoryTable("__GoobEFMigrationsHistory", "goobstation"));
        return new GoobStationPostgresServerDbContext(optionsBuilder.Options);
    }
}

public sealed class GoobStationDesignTimeContextFactorySqlite : IDesignTimeDbContextFactory<GoobStationSqliteServerDbContext>
{
    public GoobStationSqliteServerDbContext CreateDbContext(string[] args)
    {
#if !USE_SYSTEM_SQLITE
        raw.SetProvider(new SQLite3Provider_e_sqlite3());
#endif

        var optionsBuilder = new DbContextOptionsBuilder<GoobStationSqliteServerDbContext>();
        optionsBuilder.UseSqlite("Data Source=:memory:", sqliteOptions =>
            sqliteOptions.MigrationsHistoryTable("__GoobEFMigrationsHistory"));
        return new GoobStationSqliteServerDbContext(optionsBuilder.Options);
    }
}

#endif
