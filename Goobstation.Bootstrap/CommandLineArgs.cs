using System.Diagnostics.CodeAnalysis;

namespace Goobstation.Bootstrap;

public sealed class CommandLineArgs
{
    /// <summary>
    /// Generate client.
    /// </summary>
    public bool Client { get; set; }

    /// <summary>
    /// Generate server.
    /// </summary>
    public bool Server { get; set; }

    /// <summary>
    /// Should we also build the relevant project.
    /// </summary>
    public bool SkipBuild { get; set; }

    /// <summary>
    /// Should we open a new shell for the run project.
    /// </summary>
    public bool ShellExecute { get; set; }

    // CommandLineArgs, 3rd of her name.
    public static bool TryParse(IReadOnlyList<string> args, [NotNullWhen(true)] out CommandLineArgs? parsed)
    {
        parsed = null;
        var client = true;
        var server = true;
        var skipBuild = false;
        var shellExecute = true;

        using var enumerator = args.GetEnumerator();
        var i = -1;

        while (enumerator.MoveNext())
        {
            i++;
            var arg = enumerator.Current;
            if (i == 0)
            {
                switch (arg)
                {
                    case "client":
                        server = false;
                        break;
                    case "server":
                        client = false;
                        break;
                }
            }

            switch (arg)
            {
                case "--skip-build":
                    skipBuild = true;
                    break;
                case "--no-shell-execute":
                    shellExecute = false;
                    break;
                case "--help":
                    PrintHelp();
                    return false;
                default:
                    Console.WriteLine("Unknown argument: {0}", arg);
                    break;
            }
        }

        parsed = new CommandLineArgs(client, server, skipBuild, shellExecute);
        return true;
    }

    private static void PrintHelp()
    {
        Console.WriteLine(@"
Usage: Goobstation.Bootstrap [client/server/(both)] [options]

Options:
  --skip-build          Skips building the project and uses what's already there.
  --no-shell-execute    Doesn't open a new shell and instead uses the bootstrap shell for logs.
");
    }

    private CommandLineArgs(
        bool client,
        bool server,
        bool skipBuild,
        bool shellExecute)
    {
        Client = client;
        Server = server;
        SkipBuild = skipBuild;
        ShellExecute = shellExecute;
    }
}
