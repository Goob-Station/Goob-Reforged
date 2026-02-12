// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: MIT-WIZARDS

using System.Diagnostics;
using Goobstation.Bootstrap;

await BootstrapBuilder.BuildAll();

var command = args.Length > 0 ? args[0].ToLowerInvariant() : null;

switch (command)
{
    case null:
        var server = StartProject("Content.Server/Content.Server.csproj");
        var client = StartProject("Content.Client/Content.Client.csproj");
        if (server == null || client == null)
            return 1;
        server.WaitForExit();
        client.WaitForExit();
        return 0;

    case "run-client":
        return RunProject("Content.Client/Content.Client.csproj");

    case "run-server":
        return RunProject("Content.Server/Content.Server.csproj");

    default:
        PrintUsage();
        return 1;
}

static void PrintUsage()
{
    Console.WriteLine("Usage: Goobstation.Bootstrap [command]");
    Console.WriteLine();
    Console.WriteLine("  (no args)    - Build and run client + server");
    Console.WriteLine("  run-client   - Build and run the client only");
    Console.WriteLine("  run-server   - Build and run the server only");
}

static int RunProject(string projectPath)
{
    using var process = StartProject(projectPath);
    if (process == null)
        return 1;
    process.WaitForExit();
    return process.ExitCode;
}

static Process? StartProject(string projectPath)
{
    var process = Process.Start(new ProcessStartInfo
    {
        FileName = BootstrapBuilder.DotnetPath,
        Arguments = $"run --project {projectPath}",
        UseShellExecute = false
    });

    if (process == null)
        Console.Error.WriteLine($"Failed to start process for {projectPath}");

    return process;
}
