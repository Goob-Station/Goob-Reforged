using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Content.Goobstation.Server.Database;

public sealed partial class GoobstationDbManager
{
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
