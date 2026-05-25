using System.Diagnostics;
using System.IO.Compression;
using Content.ModuleManager;
using Robust.Packaging;
using Robust.Packaging.AssetProcessing;
using Robust.Packaging.AssetProcessing.Passes;
using Robust.Packaging.Utility;
using Robust.Shared.Timing;

namespace Content.Packaging;

public static class ServerPackaging
{
    private static readonly List<PlatformReg> Platforms = new()
    {
        new PlatformReg("win-x64", "Windows", true),
        new PlatformReg("win-arm64", "Windows", true),
        new PlatformReg("linux-x64", "Linux", true),
        new PlatformReg("linux-arm64", "Linux", true),
        new PlatformReg("osx-x64", "MacOS", true),
        new PlatformReg("osx-arm64", "MacOS", true),
        // Non-default platforms (i.e. for Watchdog Git)
        new PlatformReg("win-x86", "Windows", false),
        new PlatformReg("linux-x86", "Linux", false),
        new PlatformReg("linux-arm", "Linux", false),
        new PlatformReg("freebsd-x64", "FreeBSD", false),
    };

    private static List<string> PlatformRids => Platforms
        .Select(o => o.Rid)
        .ToList();

    private static List<string> PlatformRidsDefault => Platforms
        .Where(o => o.BuildByDefault)
        .Select(o => o.Rid)
        .ToList();

    private static readonly List<string> ServerExtraAssemblies = new()
    {
        // Python script had Npgsql. though we want Npgsql.dll as well soooo
        "Npgsql",
        "Microsoft",
        "Concentus",
        "NetCord",
    };

    private static readonly List<string> ServerNotExtraAssemblies = new()
    {
        "Microsoft.CodeAnalysis",
    };

    private static readonly HashSet<string> BinSkipFolders = new()
    {
        // Roslyn localization files, screw em.
        "cs",
        "de",
        "es",
        "fr",
        "it",
        "ja",
        "ko",
        "pl",
        "pt-BR",
        "ru",
        "tr",
        "zh-Hans",
        "zh-Hant",
    };

    public static async Task PackageServer(bool skipBuild, bool hybridAcz, bool logBuild, IPackageLogger logger, string configuration, List<string>? platforms = null)
    {
        if (platforms == null)
        {
            platforms ??= PlatformRidsDefault;
        }

        if (hybridAcz)
        {
            // Hybrid ACZ involves a file "Content.Client.zip" in the server executable directory.
            // Rather than hosting the client ZIP on the watchdog or on a separate server,
            //  Hybrid ACZ uses the ACZ hosting functionality to host it as part of the status host,
            //  which means that features such as automatic UPnP forwarding still work properly.
            await ClientPackaging.PackageClient(skipBuild, logBuild, configuration, logger);
        }

        // Good variable naming right here.
        foreach (var platform in Platforms)
        {
            if (!platforms.Contains(platform.Rid))
                continue;

            await BuildPlatform(platform, skipBuild, hybridAcz, logBuild, configuration, logger);
        }
    }

    private static async Task BuildPlatform(
        PlatformReg platform,
        bool skipBuild,
        bool hybridAcz,
        bool logBuild,
        string configuration,
        IPackageLogger logger)
    {
        logger.Info($"Building project for {platform.TargetOs}...");

        if (!skipBuild)
        {
            var serverModules = FindServerModules();

            foreach (var module in serverModules)
            {
                var projectName = Path.GetFileName(module);
                var startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    ArgumentList =
                    {
                        "build",
                        Path.Combine(module, $"{projectName}.csproj"),
                        "-c", configuration,
                        "--nologo",
                        "/v:m",
                        $"/p:TargetOs={platform.TargetOs}",
                        "/t:Rebuild",
                        "/p:FullRelease=true",
                        "/m",
                    },
                };

                if (logBuild)
                {
                    startInfo.ArgumentList.Add($"/bl:{Path.Combine("release", $"server-{platform.Rid}.binlog")}");
                    startInfo.ArgumentList.Add("/p:ReportAnalyzer=true");
                }

                await ProcessHelpers.RunCheck(startInfo);
            }

            await PublishClientServer(platform.Rid, platform.TargetOs, configuration);
        }

        logger.Info($"Packaging {platform.Rid} server...");

        var sw = RStopwatch.StartNew();
        {
            await using var zipFile =
                File.Open(Path.Combine("release", $"SS14.Server_{platform.Rid}.zip"), FileMode.Create, FileAccess.ReadWrite);
            using var zip = new ZipArchive(zipFile, ZipArchiveMode.Update);
            var writer = new AssetPassZipWriter(zip);

            await WriteServerResources(platform, "", writer, logger, hybridAcz, default, configuration);
            await writer.FinishedTask;
        }

        logger.Info($"Finished packaging server in {sw.Elapsed}");
    }

    private static List<string> FindServerModules(string path = ".")
    {
        var serverModules = new List<string> { "Content.Server" };

        // Modules - Add modules from Modules/ directory
        var discoveredModules = ModuleDiscovery.DiscoverModules(path)
            .Where(m => m.Type == ModuleRole.Server)
            .Select(m => Path.GetDirectoryName(m.ProjectPath))
            .Where(dir => dir != null);

        serverModules.AddRange(discoveredModules!);

        return serverModules;
    }

    private static List<string> FindAllServerModules(string path = ".", string configuration = "Debug")
    {
        if (string.IsNullOrEmpty(path))
            path = ".";

        var modules = new List<string> { "Content.Server.Database", "Content.Server", "Content.Shared", "Content.Shared.Database", "ContentModuleManager" };

        var coreDepsPath = Path.Combine(path, "bin", "Content.Server", "Content.Server.deps.json");

        foreach (var mod in ModuleDiscovery.DiscoverModules(path).Where(m => m.Type != ModuleRole.Client).DistinctBy(m => m.Name))
        {
            modules.Add(mod.Name);

            var moduleOutputDir = ModuleDiscovery.GetModuleOutputDir(mod.ProjectPath, configuration);
            var moduleDepsPath = Path.Combine(moduleOutputDir, $"{mod.Name}.deps.json");

            if (File.Exists(coreDepsPath) && File.Exists(moduleDepsPath))
            {
                modules.AddRange(DepsHandler.GetModuleUniqueAssemblies(coreDepsPath, moduleDepsPath));
            }
        }

        return modules.Distinct().ToList();
    }

    private static async Task PublishClientServer(string runtime, string targetOs, string configuration)
    {
        await ProcessHelpers.RunCheck(new ProcessStartInfo
        {
            FileName = "dotnet",
            ArgumentList =
            {
                "publish",
                "--runtime", runtime,
                "--no-self-contained",
                "-c", configuration,
                $"/p:TargetOs={targetOs}",
                "/p:FullRelease=True",
                "/m",
                "RobustToolbox/Robust.Server/Robust.Server.csproj"
            }
        });
    }

    private static async Task WriteServerResources(
        PlatformReg platform,
        string contentDir,
        AssetPass pass,
        IPackageLogger logger,
        bool hybridAcz,
        CancellationToken cancel,
        string configuration)
    {
        var graph = new RobustServerAssetGraph();
        var passes = graph.AllPasses.ToList();

        pass.Dependencies.Add(new AssetPassDependency(graph.Output.Name));
        passes.Add(pass);

        // Front the resources side of the graph with a global "last write wins" dedup pass.
        // Loaders feed dedup; dedup re-emits unique paths into InputResources. This lets
        // modules override main-content resource files without later passes choking on
        // duplicate VFS paths.
        var dedupPass = new AssetPassLastWriteWins { Name = "ContentLastWriteWinsResources" };
        graph.InputResources.AddDependency(dedupPass);
        passes.Add(dedupPass);

        AssetGraph.CalculateGraph(passes, logger);

        var inputPassCore = graph.InputCore;
        var inputPassResources = dedupPass;

        var contentAssemblies = FindAllServerModules(configuration: configuration);

        // Additional assemblies that need to be copied such as EFCore.
        var sourcePath = Path.Combine(contentDir, "bin", "Content.Server");

        // Should this be an asset pass?
        // For future archaeologists I just want audio rework to work and need the audio pass so
        // just porting this as is from python.
        foreach (var fullPath in Directory.EnumerateFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            var fileName = Path.GetFileNameWithoutExtension(fullPath);

            if (!ServerNotExtraAssemblies.Any(o => fileName.StartsWith(o)) && ServerExtraAssemblies.Any(o => fileName.StartsWith(o)))
            {
                contentAssemblies.Add(fileName);
            }
        }

        await RobustSharedPackaging.DoResourceCopy(
            Path.Combine("RobustToolbox", "bin", "Server",
                platform.Rid,
                "publish"),
            inputPassCore,
            BinSkipFolders,
            cancel: cancel);

        await WriteServerContentAssemblies(
            inputPassResources,
            contentDir,
            contentAssemblies,
            cancel,
            configuration);

        await RobustServerPackaging.WriteServerResources(contentDir, inputPassResources, cancel);
        await DoModularResourceCopy(contentDir, inputPassResources, cancel);

        if (hybridAcz)
        {
            inputPassCore.InjectFileFromDisk("Content.Client.zip", Path.Combine("release", "SS14.Client.zip"));
        }

        inputPassCore.InjectFinished();
        inputPassResources.InjectFinished();
    }

    private static async Task DoModularResourceCopy(
        string contentDir,
        AssetPass pass,
        CancellationToken cancel)
    {
        var ignore = RobustSharedPackaging.SharedIgnoredResources.ToHashSet();
        foreach (var (manifest, resourcePath) in ModuleResourceCopier.EnumerateResourcePaths(contentDir))
        {
            await RobustSharedPackaging.DoResourceCopy(resourcePath, pass, ignore, "", cancel);
        }
    }

    private static Task WriteServerContentAssemblies(
        AssetPass pass,
        string contentDir,
        IEnumerable<string> contentAssemblies,
        CancellationToken cancel = default,
        string configuration = "Debug")
    {
        var mainBinDir = Path.Combine(contentDir, "bin", "Content.Server");

        var moduleAssemblyPaths = ModuleDiscovery.DiscoverModules(contentDir)
            .Where(m => m.Type != ModuleRole.Client)
            .ToDictionary(
                m => m.Name,
                m => ModuleDiscovery.GetModuleOutputDir(m.ProjectPath, configuration)
            );

        foreach (var asm in contentAssemblies)
        {
            cancel.ThrowIfCancellationRequested();

            var sourceDir = moduleAssemblyPaths.GetValueOrDefault(asm) ?? mainBinDir;

            var dllPath = Path.Combine(sourceDir, $"{asm}.dll");
            if (File.Exists(dllPath))
                pass.InjectFileFromDisk($"Assemblies/{asm}.dll", dllPath);

            var pdbPath = Path.Combine(sourceDir, $"{asm}.pdb");
            if (File.Exists(pdbPath))
                pass.InjectFileFromDisk($"Assemblies/{asm}.pdb", pdbPath);
        }

        return Task.CompletedTask;
    }

    private readonly record struct PlatformReg(string Rid, string TargetOs, bool BuildByDefault);
}
