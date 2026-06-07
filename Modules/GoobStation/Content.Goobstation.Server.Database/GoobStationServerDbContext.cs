// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Microsoft.EntityFrameworkCore;

namespace Content.GoobStation.Server.Database;

public abstract class GoobStationServerDbContext : DbContext
{
    public DbSet<NetspeakWord> NetspeakWords { get; set; } = null!;

    protected GoobStationServerDbContext(DbContextOptions options) : base(options)
    {
    }
}

public sealed class GoobStationSqliteServerDbContext : GoobStationServerDbContext
{
    public GoobStationSqliteServerDbContext(DbContextOptions<GoobStationSqliteServerDbContext> options)
        : base(options)
    {
    }
}

public sealed class GoobStationPostgresServerDbContext : GoobStationServerDbContext
{
    public GoobStationPostgresServerDbContext(DbContextOptions<GoobStationPostgresServerDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("goobstation");
    }
}
