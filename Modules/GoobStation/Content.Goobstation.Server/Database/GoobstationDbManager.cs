// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.IO;
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
        _ = _cfg.GetCVar(CCVars.DatabaseEngine).ToLower() switch
        {
            "sqlite" => SetupSqlite(),
            "postgres" => SetupPostgres(),
            var engine => throw new InvalidDataException($"Unknown database engine: {engine}"),
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
            (
                _cfg.GetCVar(CCVars.DatabasePgHost),
                _cfg.GetCVar(CCVars.DatabasePgPort),
                _cfg.GetCVar(CCVars.DatabasePgDatabase),
                _cfg.GetCVar(CCVars.DatabasePgUsername),
                _cfg.GetCVar(CCVars.DatabasePgPassword)
            );
        var connString = new NpgsqlConnectionStringBuilder
            {
                Host = host, Port = port, Database = db, Username = user, Password = pass
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
}
