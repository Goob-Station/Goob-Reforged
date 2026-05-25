using System.Diagnostics;
using System.IO.Compression;
using Content.ModuleManager;
using Robust.Packaging;
using Robust.Packaging.AssetProcessing;
using Robust.Packaging.AssetProcessing.Passes;
using Robust.Packaging.Utility;
using Robust.Shared.Timing;

namespace Content.Packaging;

public static class ClientPackaging
{
    /// <summary>
    /// Be advised this can be called from server packaging during a HybridACZ build.
    /// Be also advised this goes against god and nature
    /// </summary>
    public static async Task PackageClient(bool skipBuild, bool logBuild, string configuration, IPackageLogger logger, string path = ".")
    {
        logger.Info("Building client...");

        if (!skipBuild)
        {
            var clientProjects = GetClientModules(path);

            foreach (var project in clientProjects)
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    ArgumentList =
                    {
                        "build",
                        project,
                        "-c", configuration,
                        "--nologo",
                        "/v:m",
                        "/t:Rebuild",
                        "/p:FullRelease=true",
                        "/m",
                    },
                };

                if (logBuild)
                {
                    startInfo.ArgumentList.Add($"/bl:{Path.Combine("release", "client.binlog")}");
                    startInfo.ArgumentList.Add("/p:ReportAnalyzer=true");
                }

                await ProcessHelpers.RunCheck(startInfo);
            }
        }

        logger.Info("Packaging client...");

        var sw = RStopwatch.StartNew();
        {
            await using var zipFile =
                File.Open(Path.Combine("release", "SS14.Client.zip"), FileMode.Create, FileAccess.ReadWrite);
            await using var zip = new ZipArchive(zipFile, ZipArchiveMode.Update);
            var writer = new AssetPassZipWriter(zip);

            await WriteResources("", configuration, writer, logger, default);
            await writer.FinishedTask;
        }

        logger.Info($"Finished packaging client in {sw.Elapsed}");
    }

    private static List<string> GetClientModules(string path)
    {
        var clientProjects = new List<string> { Path.Combine("Content.Client", "Content.Client.csproj") };

        // Modules - Add modules from Modules/ directory
        clientProjects.AddRange(
            ModuleDiscovery.DiscoverModules(path)
                .Where(m => m.Type == ModuleRole.Client)
                .Select(m => m.ProjectPath)
        );

        return clientProjects;
    }

    private static List<string> FindAllModules(string path = ".", string configuration = "Debug")
    {
        if (string.IsNullOrEmpty(path))
            path = ".";

        var modules = new List<string> { "Content.Client", "Content.Shared", "Content.Shared.Database", "ContentModuleManager" };

        var coreDepsPath = Path.Combine(path, "bin", "Content.Client", "Content.Client.deps.json");

        foreach (var mod in ModuleDiscovery.DiscoverModules(path).Where(m => m.Type != ModuleRole.Server).DistinctBy(m => m.Name))
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

    public static async Task WriteResources(
        string contentDir,
        string configuration,
        AssetPass pass,
        IPackageLogger logger,
        CancellationToken cancel)
    {
        var graph = new RobustClientAssetGraph();
        pass.Dependencies.Add(new AssetPassDependency(graph.Output.Name));

        var dedupPass = new AssetPassLastWriteWins { Name = "ContentLastWriteWins" };
        graph.Input.AddDependency(dedupPass);

        var dropSvgPass = new AssetPassFilterDrop(f => f.Path.EndsWith(".svg"))
        {
            Name = "DropSvgPass",
        };
        dropSvgPass.AddDependency(graph.Input).AddBefore(graph.PresetPasses);

        AssetGraph.CalculateGraph([pass, dedupPass, dropSvgPass, ..graph.AllPasses], logger);

        var inputPass = dedupPass;

        var modules = FindAllModules(contentDir, configuration);

        await WriteClientContentAssemblies(
            inputPass,
            contentDir,
            modules,
            cancel,
            configuration);

        await WriteClientResources(contentDir, inputPass, SharedPackaging.AdditionalIgnoredResources, cancel);

        inputPass.InjectFinished();
    }

    private static async Task WriteClientResources(
        string contentDir,
        AssetPass pass,
        IReadOnlySet<string> additionalIgnoredResources,
        CancellationToken cancel = default)
    {
        var ignoreSet = RobustClientPackaging.ClientIgnoredResources
            .Union(RobustSharedPackaging.SharedIgnoredResources)
            .Union(additionalIgnoredResources)
            .ToHashSet();

        await RobustSharedPackaging.DoResourceCopy(Path.Combine(contentDir, "Resources"), pass, ignoreSet, cancel: cancel);
        await DoModularResourceCopy(contentDir, pass, ignoreSet, cancel);
    }

    private static async Task DoModularResourceCopy(
        string contentDir,
        AssetPass pass,
        HashSet<string> ignoreSet,
        CancellationToken cancel = default)
    {
        foreach (var (manifest, resourcePath) in ModuleResourceCopier.EnumerateResourcePaths(contentDir))
        {
            await RobustSharedPackaging.DoResourceCopy(resourcePath, pass, ignoreSet, "", cancel);
        }
    }

    private static Task WriteClientContentAssemblies(
        AssetPass pass,
        string contentDir,
        IEnumerable<string> contentAssemblies,
        CancellationToken cancel = default,
        string configuration = "Debug")
    {
        var mainBinDir = Path.Combine(contentDir, "bin", "Content.Client");

        var moduleOutputDirs = new Dictionary<string, string>();

        // Discover all non-Server modules
        var allModules = ModuleDiscovery.DiscoverModules(contentDir)
            .Where(m => m.Type != ModuleRole.Server)
            .ToList();

        // Map each module to its own build output directory
        foreach (var module in allModules)
        {
            var moduleOutputDir = ModuleDiscovery.GetModuleOutputDir(module.ProjectPath, configuration);
            moduleOutputDirs[module.Name] = moduleOutputDir;
        }

        foreach (var asm in contentAssemblies)
        {
            cancel.ThrowIfCancellationRequested();

            // Check module output dirs first, fall back to mainBinDir
            var sourceDir = moduleOutputDirs.GetValueOrDefault(asm) ?? mainBinDir;

            var dllPath = Path.Combine(sourceDir, $"{asm}.dll");
            if (File.Exists(dllPath))
                pass.InjectFileFromDisk($"Assemblies/{asm}.dll", dllPath);

            var pdbPath = Path.Combine(sourceDir, $"{asm}.pdb");
            if (File.Exists(pdbPath))
                pass.InjectFileFromDisk($"Assemblies/{asm}.pdb", pdbPath);
        }

        return Task.CompletedTask;
    }
}
