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

    // CommandLineArgs, 3rd of her name.
    public static bool TryParse(IReadOnlyList<string> args, [NotNullWhen(true)] out CommandLineArgs? parsed)
    {
        parsed = null;
        bool client = true;
        bool server = true;
        var skipBuild = false;

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
                case "--help":
                    PrintHelp();
                    return false;
                default:
                    Console.WriteLine("Unknown argument: {0}", arg);
                    break;
            }
        }

        parsed = new CommandLineArgs(client, server, skipBuild);
        return true;
    }

    private static void PrintHelp()
    {
        Console.WriteLine(@"
Usage: Goobstation.Bootstrap [client/server/(both)] [options]

Options:
  --skip-build          Should we skip building the project and use what's already there.
");
    }

    private CommandLineArgs(
        bool client,
        bool server,
        bool skipBuild)
    {
        Client = client;
        Server = server;
        SkipBuild = skipBuild;
    }
}
