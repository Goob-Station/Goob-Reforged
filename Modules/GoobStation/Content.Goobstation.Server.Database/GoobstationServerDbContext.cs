// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using Content.Server.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NpgsqlTypes;

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
            .WithOne()
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
            .WithOne()
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
            .WithOne()
            .HasForeignKey<RMCLinkingCodes>(l => l.PlayerId)
            .HasPrincipalKey<Player>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RMCLinkedAccountLogs>()
            .HasOne(l => l.Player)
            .WithMany()
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var ipConverter = new ValueConverter<IPAddress, string>(
            v => v.ToString(),
            v => IPAddress.Parse(v));

        modelBuilder.Entity<Player>()
            .Property(p => p.LastSeenAddress)
            .HasConversion(ipConverter);

        var ipMaskConverter = new ValueConverter<NpgsqlInet, string>(
            v => InetToString(v.Address, v.Netmask),
            v => StringToInet(v)
        );

        modelBuilder
            .Entity<BanAddress>()
            .Property(e => e.Address)
            .HasColumnType("TEXT")
            .HasConversion(ipMaskConverter);

        var jsonStringConverter = new ValueConverter<JsonDocument, string>(
            v => JsonDocumentToString(v),
            v => StringToJsonDocument(v));

        var jsonByteArrayConverter = new ValueConverter<JsonDocument?, byte[]>(
            v => JsonDocumentToByteArray(v),
            v => ByteArrayToJsonDocument(v));

        modelBuilder.Entity<AdminLog>()
            .Property(log => log.Json)
            .HasConversion(jsonStringConverter);

        modelBuilder.Entity<Profile>()
            .HasKey(k => k.Id);

        modelBuilder.Entity<Profile>()
            .Property(log => log.Markings)
            .HasConversion(jsonByteArrayConverter);

        modelBuilder.Entity<Profile>()
            .Property(log => log.OrganMarkings)
            .HasConversion(jsonByteArrayConverter);

        // EF core can make this automatically unique on sqlite but not psql.
        modelBuilder.Entity<IPIntelCache>()
            .HasIndex(p => p.Address)
            .IsUnique();
    }

    private static string InetToString(IPAddress address, int mask) {
        if (address.IsIPv4MappedToIPv6)
        {
            // Fix IPv6-mapped IPv4 addresses
            // So that IPv4 addresses are consistent between separate-socket and dual-stack socket modes.
            address = address.MapToIPv4();
            mask -= 96;
        }
        return $"{address}/{mask}";
    }

    private static NpgsqlInet StringToInet(string inet) {
        var idx = inet.IndexOf('/', StringComparison.Ordinal);
        return new NpgsqlInet(
            IPAddress.Parse(inet.AsSpan(0, idx)),
            byte.Parse(inet.AsSpan(idx + 1), provider: CultureInfo.InvariantCulture)
        );
    }

    private static string JsonDocumentToString(JsonDocument document)
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions {Indented = false});

        document.WriteTo(writer);
        writer.Flush();

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static JsonDocument StringToJsonDocument(string str)
    {
        return JsonDocument.Parse(str);
    }

    private static byte[] JsonDocumentToByteArray(JsonDocument? document)
    {
        if (document == null)
        {
            return Array.Empty<byte>();
        }

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions {Indented = false});

        document.WriteTo(writer);
        writer.Flush();

        return stream.ToArray();
    }

    private static JsonDocument ByteArrayToJsonDocument(byte[] str)
    {
        return JsonDocument.Parse(str);
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
