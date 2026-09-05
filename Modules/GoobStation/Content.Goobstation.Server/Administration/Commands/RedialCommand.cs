using Content.Goobstation.Server.Redial;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;

namespace Content.Goobstation.Server.Administration.Commands;

[AdminCommand(AdminFlags.Host)]
public sealed partial class RedialCommand : LocalizedCommands
{
    [Dependency] private IPlayerManager _playerMan = default!;
    [Dependency] private RedialManager _redialMan = default!;

    public override string Command => "redial";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 2)
        {
            shell.WriteLine(Help);
            return;
        }

        var address = args[0];

        for (int i = 1; i < args.Length; i++)
        {
            var playerName = args[i];

            if (!_playerMan.TryGetSessionByUsername(playerName, out var player))
            {
                shell.WriteError($"Unable to find player: '{playerName}'.");
                return;
            }

            _redialMan.Redial(player.Channel, address);
        }
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        return args.Length switch
        {
            >1 => CompletionResult.FromHintOptions(CompletionHelper.SessionNames(), Loc.GetString("main-menu-username-text")),
            _ => CompletionResult.Empty,
        };
    }
}
