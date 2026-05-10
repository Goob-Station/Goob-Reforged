// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Server.Database;
using Microsoft.EntityFrameworkCore;

namespace Content.Goobstation.Server.Database;

public abstract class GoobstationServerDbContext : DbContext
{
    public DbSet<NetspeakWord> NetspeakWords { get; set; } = null!;

    // RMC14
    public DbSet<RMCDiscordAccount> RMCDiscordAccounts { get; set; } = default!;
    public DbSet<RMCLinkedAccount> RMCLinkedAccounts { get; set; } = default!;
    public DbSet<RMCPatronTier> RMCPatronTiers { get; set; } = default!;
    public DbSet<RMCPatron> RMCPatrons { get; set; } = default!;
    public DbSet<RMCLinkingCodes> RMCLinkingCodes { get; set; } = default!;
    public DbSet<RMCLinkedAccountLogs> RMCLinkedAccountLogs { get; set; } = default!;
    public DbSet<RMCPatronLobbyMessage> RMCPatronLobbyMessages { get; set; } = default!;
    public DbSet<RMCPatronRoundEndNTShoutout> RMCPatronRoundEndNTShoutouts { get; set; } = default!;

    protected GoobstationServerDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // RMC14
        modelBuilder.Entity<RMCLinkedAccount>()
            .HasOne(l => l.Player)
            .WithOne(p => p.LinkedAccount)
            .HasForeignKey<RMCLinkedAccount>(l => l.PlayerId)
            .HasPrincipalKey<Player>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RMCLinkedAccount>()
            .HasOne(l => l.Discord)
            .WithOne(d => d.LinkedAccount)
            .HasForeignKey<RMCLinkedAccount>(l => l.DiscordId)
            .HasPrincipalKey<RMCDiscordAccount>(d => d.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RMCPatron>()
            .HasOne(p => p.Player)
            .WithOne(p => p.Patron)
            .HasForeignKey<RMCPatron>(p => p.PlayerId)
            .HasPrincipalKey<Player>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RMCPatron>()
            .HasOne(p => p.Tier)
            .WithMany(t => t.Patrons)
            .HasForeignKey(p => p.TierId)
            .HasPrincipalKey(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RMCPatronTier>()
            .HasIndex(t => t.DiscordRole)
            .IsUnique();

        modelBuilder.Entity<RMCLinkingCodes>()
            .HasOne(l => l.Player)
            .WithOne(p => p.LinkingCodes)
            .HasForeignKey<RMCLinkingCodes>(l => l.PlayerId)
            .HasPrincipalKey<Player>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RMCLinkedAccountLogs>()
            .HasOne(l => l.Player)
            .WithMany(p => p.LinkedAccountLogs)
            .HasForeignKey(l => l.PlayerId)
            .HasPrincipalKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RMCLinkedAccountLogs>()
            .HasOne(l => l.Discord)
            .WithMany(p => p.LinkedAccountLogs)
            .HasForeignKey(l => l.DiscordId)
            .HasPrincipalKey(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class GoobstationSqliteServerDbContext : GoobstationServerDbContext
{
    public GoobstationSqliteServerDbContext(DbContextOptions<GoobstationSqliteServerDbContext> options)
        : base(options)
    {
    }
}

public sealed class GoobstationPostgresServerDbContext : GoobstationServerDbContext
{
    public GoobstationPostgresServerDbContext(DbContextOptions<GoobstationPostgresServerDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("goobstation");
    }
}
