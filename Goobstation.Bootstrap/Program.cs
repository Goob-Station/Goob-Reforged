using System.Diagnostics;
using Goobstation.Bootstrap;

var repoRoot = BootstrapBuilder.FindRepoRoot();
Environment.CurrentDirectory = repoRoot;

if (!CommandLineArgs.TryParse(args, out var parsed))
    return 0;

if (parsed.Client && parsed.Server)
{
    await BootstrapBuilder.BuildAll();
    var server = StartProject("Content.Server/Content.Server.csproj", parsed.ShellExecute);
    var client = StartProject("Content.Client/Content.Client.csproj", parsed.ShellExecute);
    if (server == null || client == null)
        return 1;
    server.WaitForExit();
    client.WaitForExit();
    return 0;
}

if (parsed.Client)
{
    await BootstrapBuilder.BuildClient();
    return RunProject("Content.Client/Content.Client.csproj", parsed.ShellExecute);
}

if (parsed.Server)
{
    await BootstrapBuilder.BuildServer();
    return RunProject("Content.Server/Content.Server.csproj", parsed.ShellExecute);
}

return 1;

static int RunProject(string projectPath, bool useShellExecute)
{
    using var process = StartProject(projectPath, useShellExecute);
    if (process == null)
        return 1;
    process.WaitForExit();
    return process.ExitCode;
}

static Process? StartProject(string projectPath, bool useShellExecute)
{
    var process = Process.Start(new ProcessStartInfo
    {
        FileName = BootstrapBuilder.DotnetPath,
        Arguments = $"run --project {projectPath}",
        UseShellExecute = useShellExecute,
    });

    if (process == null)
        Console.Error.WriteLine($"Failed to start process for {projectPath}");

    return process;
}
