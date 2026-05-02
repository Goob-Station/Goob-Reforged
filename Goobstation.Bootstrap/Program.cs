using System.Diagnostics;
using Goobstation.Bootstrap;

var repoRoot = BootstrapBuilder.FindRepoRoot();
Environment.CurrentDirectory = repoRoot;

if (!CommandLineArgs.TryParse(args, out var parsed))
    return 0;

// TODO if server/client argument is specified, we should build only server/client modules here
if (!parsed.SkipBuild)
    await BootstrapBuilder.BuildAll();

if (parsed.Client && parsed.Server)
{
    var server = StartProject("Content.Server/Content.Server.csproj");
    var client = StartProject("Content.Client/Content.Client.csproj");
    if (server == null || client == null)
        return 1;
    server.WaitForExit();
    client.WaitForExit();
    return 0;
}

if (parsed.Client)
    return RunProject("Content.Client/Content.Client.csproj");

if (parsed.Server)
    return RunProject("Content.Server/Content.Server.csproj");

return 1;

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
