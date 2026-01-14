// SPDX-FileCopyrightText: 2025 Space Station 14 Contributors
//
// SPDX-License-Identifier: MIT-WIZARDS

using System.Diagnostics.CodeAnalysis;

namespace Content.Packaging;

public sealed class CommandLineArgs
{
    // PJB forgib me

    /// <summary>
    /// Generate client or server.
    /// </summary>
    public bool Client { get; set; }

    /// <summary>
    /// Should we also build the relevant project.
    /// </summary>
    public bool SkipBuild { get; set; }

    /// <summary>
    /// Should we wipe the release folder or ignore it.
    /// </summary>
    public bool WipeRelease { get; set; }

    /// <summary>
    /// Platforms for server packaging.
    /// </summary>
    public List<string>? Platforms { get; set; }

    /// <summary>
    /// Use HybridACZ for server packaging.
    /// </summary>
    public bool HybridAcz { get; set; }

    /// <summary>
    /// Configuration used for when packaging the server. (Release, Debug, Tools)
    /// </summary>
    public string Configuration { get; set; }

    // CommandLineArgs, 3rd of her name.
    public static bool TryParse(IReadOnlyList<string> args, [NotNullWhen(true)] out CommandLineArgs? parsed)
    {
        parsed = null;
        bool? client = null;
        var skipBuild = false;
        var wipeRelease = true;
        var hybridAcz = false;
        var configuration = "Release";
        List<string>? platforms = null;

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
                    case "client": client = true; break;
                    case "server": client = false; break;
                    default: return false;
                }
                continue;
            }

            switch (arg)
            {
                case "--skip-build": skipBuild = true; break;
                case "--no-wipe-release": wipeRelease = false; break;
                case "--hybrid-acz": hybridAcz = true; break;
                case "--platform" when !enumerator.MoveNext():
                    Console.WriteLine("No platform provided");
                    return false;
                case "--platform":
                    platforms ??= [];
                    platforms.Add(enumerator.Current);
                    break;
                case "--configuration" when !enumerator.MoveNext():
                    Console.WriteLine("No configuration provided");
                    return false;
                case "--configuration": configuration = enumerator.Current; break;
                case "--help":
                    PrintHelp();
                    return false;
                default: Console.WriteLine("Unknown argument: {0}", arg); break;
            }
        }

        if (client == null)
        {
            Console.WriteLine("Client / server packaging unspecified.");
            return false;
        }

        parsed = new CommandLineArgs(client.Value, skipBuild, wipeRelease, hybridAcz, platforms, configuration);
        return true;
    }

    private static void PrintHelp()
    {
        Console.WriteLine(@"
Usage: Content.Packaging [client/server] [options]

Options:
  --skip-build          Should we skip building the project and use what's already there.
  --no-wipe-release     Don't wipe the release folder before creating files.
  --hybrid-acz          Use HybridACZ for server builds.
  --platform            Platform for server builds. Default will output several x64 targets.
  --configuration       Configuration to use for building the server (Release, Debug, Tools). Default is Release.
");
    }

    private CommandLineArgs(
        bool client,
        bool skipBuild,
        bool wipeRelease,
        bool hybridAcz,
        List<string>? platforms,
        string configuration)
    {
        Client = client;
        SkipBuild = skipBuild;
        WipeRelease = wipeRelease;
        HybridAcz = hybridAcz;
        Platforms = platforms;
        Configuration = configuration;
    }
}
