// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Content.Shared.CCVar;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;

namespace Content.Goobstation.Server.Database;

/// <summary>
/// Your shitty database-related ideas, now in goobmod!
/// </summary>
public interface IGoobstationDbManager
{
    void Init();
    void Shutdown();
    Task<List<NetspeakWord>> GetNetspeakWordsAsync();
    Task AddNetspeakWordAsync(string keyword, string username);
    Task RemoveNetspeakWordAsync(string keyword);
}

public sealed partial class GoobstationDbManager : IGoobstationDbManager
{
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private IResourceManager _res = default!;
    [Dependency] private ILogManager _logMgr = default!;

    private ISawmill _sawmill = default!;
    private DbContextOptions? _options;
    private bool _isPostgres;

    public void Init()
    {
        _sawmill = _logMgr.GetSawmill("goob.db");
        var _ = _cfg.GetCVar(CCVars.DatabaseEngine).ToLower() switch
            {
                "sqlite" => SetupSqlite(), "postgres" => SetupPostgres(),
                var engine => throw new InvalidDataException($"Unknown database engine: {engine}")
            };
        using var ctx = CreateContext();
        ctx.Database.Migrate();
    }

    public void Shutdown() { }

    private bool SetupSqlite()
    {
        _isPostgres = false;
        var path = _cfg.GetCVar(CCVars.DatabaseSqliteDbPath);
        var finalPath = _res.UserData.RootDir is { } root
                ? Path.Combine(root, path)
                : ":memory:";
        _sawmill.Debug($"Goobstation DB running on {finalPath}");
        var builder = new DbContextOptionsBuilder<GoobstationSqliteServerDbContext>();
        builder.UseSqlite($"Data Source={finalPath}",
                sqliteOptions =>
                    sqliteOptions.MigrationsHistoryTable("__GoobEFMigrationsHistory"));
        _options = builder.Options;
        return true;
    }

    private bool SetupPostgres()
    {
        _isPostgres = true;
        var (host, port, db, user, pass) =
            (_cfg.GetCVar(CCVars.DatabasePgHost)
                , _cfg.GetCVar(CCVars.DatabasePgPort)
                , _cfg.GetCVar(CCVars.DatabasePgDatabase)
                , _cfg.GetCVar(CCVars.DatabasePgUsername)
                , _cfg.GetCVar(CCVars.DatabasePgPassword)
            );

        var connString = new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Port = port,
                Database = db,
                Username = user,
                Password = pass,
            }.ConnectionString;

        _sawmill.Debug($"Using Goobstation Postgres schema at {host}:{port}/{db}");
        var builder = new DbContextOptionsBuilder<GoobstationPostgresServerDbContext>();
        builder.UseNpgsql(connString,
                npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable("__GoobEFMigrationsHistory", "goobstation"));
        _options = builder.Options;
        return true;
    }

    private GoobstationServerDbContext CreateContext() => _isPostgres switch
    {
        true => new GoobstationPostgresServerDbContext((DbContextOptions<GoobstationPostgresServerDbContext>)_options!),
        false => new GoobstationSqliteServerDbContext((DbContextOptions<GoobstationSqliteServerDbContext>)_options!)
    };

    public async Task<List<NetspeakWord>> GetNetspeakWordsAsync()
    {
        await using var ctx = CreateContext();
        return await ctx.NetspeakWords.ToListAsync();
    }

    public async Task AddNetspeakWordAsync(string keyword, string username)
    {
        await using var ctx = CreateContext();
        ctx.NetspeakWords.Add(new NetspeakWord { Keyword = keyword, Username = username });
        await ctx.SaveChangesAsync();
    }

    public async Task RemoveNetspeakWordAsync(string keyword)
    {
        await using var ctx = CreateContext();
        if (await ctx.NetspeakWords.FirstOrDefaultAsync(w => w.Keyword == keyword) is { } word)
        {
            ctx.NetspeakWords.Remove(word);
            await ctx.SaveChangesAsync();
        }
    }

    #region RMC14

    public async Task<Guid?> GetLinkingCode(Guid player)
    {
        await using var ctx = CreateContext();
        var linking = await ctx.RMCLinkingCodes.FirstOrDefaultAsync(l => l.PlayerId == player);
        return linking?.Code;
    }

    public async Task SetLinkingCode(Guid player, Guid code)
    {
        await using var ctx = CreateContext();
        var linking = await ctx.RMCLinkingCodes.FirstOrDefaultAsync(l => l.PlayerId == player);
        if (linking == null)
        {
            linking = new RMCLinkingCodes { PlayerId = player };
            ctx.RMCLinkingCodes.Add(linking);
        }

        linking.Code = code;
        linking.CreationTime = DateTime.UtcNow;
        await ctx.SaveChangesAsync();
    }

    public async Task<bool> HasLinkedAccount(Guid player, CancellationToken cancel)
    {
        await using var ctx = CreateContext();
        return await ctx.RMCLinkedAccounts.AnyAsync(l => l.PlayerId == player, cancel);
    }

    public async Task<RMCPatron?> GetPatron(Guid player, CancellationToken cancel)
    {
        await using var ctx = CreateContext();
        var patron = await ctx.RMCPatrons
            .Include(p => p.Tier)
            .Include(p => p.LobbyMessage)
            .Include(p => p.RoundEndNTShoutout)
            .FirstOrDefaultAsync(p => p.PlayerId == player, cancellationToken: cancel);
        return patron;
    }

    public async Task<List<RMCPatron>> GetAllPatrons()
    {
        await using var ctx = CreateContext();
        return await ctx.RMCPatrons
            .Include(p => p.Player)
            .Include(p => p.Tier)
            .ToListAsync();
    }

    public async Task SetGhostColor(Guid player, System.Drawing.Color? color)
    {
        await using var ctx = CreateContext();
        var patron = await ctx.RMCPatrons.FirstOrDefaultAsync(p => p.PlayerId == player);
        if (patron == null)
            return;

        patron.GhostColor = color?.ToArgb();
        await ctx.SaveChangesAsync();
    }

    public async Task SetLobbyMessage(Guid player, string message)
    {
        await using var ctx = CreateContext();
        var msg = await ctx.RMCPatronLobbyMessages
            .Include(l => l.Patron)
            .FirstOrDefaultAsync(p => p.PatronId == player);
        msg ??= ctx.RMCPatronLobbyMessages
            .Add(new RMCPatronLobbyMessage
            {
                PatronId = player,
                Message = message,
            })
            .Entity;
        msg.Message = message;

        await ctx.SaveChangesAsync();
    }

    public async Task SetNTShoutout(Guid player, string name)
    {
        await using var ctx = CreateContext();
        var msg = await ctx.RMCPatronRoundEndNTShoutouts
            .Include(s => s.Patron)
            .FirstOrDefaultAsync(p => p.PatronId == player);
        msg ??= ctx.RMCPatronRoundEndNTShoutouts
            .Add(new RMCPatronRoundEndNTShoutout()
            {
                PatronId = player,
                Name = name,
            })
            .Entity;
        msg.Name = name;

        await ctx.SaveChangesAsync();
    }

    public async Task<List<(string Message, string User)>> GetLobbyMessages()
    {
        await using var ctx = CreateContext();
        var messages = await ctx.RMCPatronLobbyMessages
            .Include(p => p.Patron)
            .ThenInclude(p => p.Player)
            .Where(p => p.Patron.Tier.LobbyMessage)
            .Where(p => !string.IsNullOrWhiteSpace(p.Message))
            .Select(p => new { p.Message, p.Patron.Player.LastSeenUserName })
            .Select(p => new ValueTuple<string, string>(p.Message, p.LastSeenUserName))
            .ToListAsync();

        return messages;
    }

    public async Task<List<string>> GetShoutouts()
    {
        await using var ctx = CreateContext();
        var ntNames = await ctx.RMCPatronRoundEndNTShoutouts
            .Include(p => p.Patron)
            .Where(p => p.Patron.Tier.RoundEndShoutout)
            .Where(p => !string.IsNullOrWhiteSpace(p.Name))
            .Select(p => p.Name)
            .ToListAsync();

        return ntNames;
    }

    #endregion
}
