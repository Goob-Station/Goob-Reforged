// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Server.Database;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Goobstation.Server.Administration.Commands;

[AdminCommand(AdminFlags.Fun)]
public sealed class AddNetspeakWordCommand : IConsoleCommand
{
    [Dependency] private readonly IGoobstationDbManager _db = default!;

    public string Command => "netspeak_add";
    public string Description => "Adds a netspeak word to the database.";
    public string Help => "Usage: netspeak_add <keyword>";

    public async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 1)
        {
            shell.WriteError("Usage: netspeak_add <keyword>");
            return;
        }

        var keyword = string.Join(" ", args);
        var username = shell.Player?.Name ?? "Console";

        await _db.AddNetspeakWordAsync(keyword, username);
        shell.WriteLine($"Added netspeak word: {keyword}");
    }
}

[AdminCommand(AdminFlags.Fun)]
public sealed class RemoveNetspeakWordCommand : IConsoleCommand
{
    [Dependency] private readonly IGoobstationDbManager _db = default!;

    public string Command => "netspeak_remove";
    public string Description => "Removes a netspeak word from the database.";
    public string Help => "Usage: netspeak_remove <keyword>";

    public async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 1)
        {
            shell.WriteError("Usage: netspeak_remove <keyword>");
            return;
        }

        var keyword = string.Join(" ", args);

        await _db.RemoveNetspeakWordAsync(keyword);
        shell.WriteLine($"Removed netspeak word: {keyword}");
    }
}

[AdminCommand(AdminFlags.Fun)]
public sealed class ListNetspeakWordsCommand : IConsoleCommand
{
    [Dependency] private readonly IGoobstationDbManager _db = default!;

    public string Command => "netspeak_list";
    public string Description => "Lists all netspeak words in the database.";
    public string Help => "Usage: netspeak_list";

    public async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var words = await _db.GetNetspeakWordsAsync();

        if (words.Count == 0)
        {
            shell.WriteLine("No netspeak words in database.");
            return;
        }

        shell.WriteLine($"Netspeak words ({words.Count}):");
        foreach (var word in words)
        {
            shell.WriteLine($"  [{word.Id}] \"{word.Keyword}\" (added by {word.Username})");
        }
    }
}
