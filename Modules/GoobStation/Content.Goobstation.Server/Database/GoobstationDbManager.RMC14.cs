using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Database;
using Microsoft.EntityFrameworkCore;

namespace Content.Goobstation.Server.Database;

public sealed partial class GoobstationDbManager
{
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
}
