using System.Diagnostics;
using System.Runtime.InteropServices;
using Content.Packaging;
using Content.ModuleManager;

namespace Goobstation.Bootstrap;

public static class BootstrapBuilder
{
    public static readonly string DotnetPath = FindDotnet();

    private static string FindDotnet()
    {
        // Runtime dir is like /usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.x/
        var runtimeDir = RuntimeEnvironment.GetRuntimeDirectory();
        var dotnetRoot = Path.GetFullPath(Path.Combine(runtimeDir, "..", "..", ".."));
        var exe = Path.Combine(dotnetRoot, "dotnet");
        if (File.Exists(exe))
            return exe;
        return "dotnet";
    }

    public static string FindRepoRoot()
    {
        var dir = AppContext.BaseDirectory;
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir, "SpaceStation14.slnx")))
                return dir;
            dir = Path.GetDirectoryName(dir);
        }
        throw new Exception("Could not find repo root (SpaceStation14.slnx)");
    }

    public static Task BuildAll()
    {
        var modules = ModuleDiscovery.DiscoverModules().ToList();

        Console.WriteLine("Building core projects...");
        RunDotnetBuild("Content.Client/Content.Client.csproj");
        RunDotnetBuild("Content.Server/Content.Server.csproj");

        foreach (var module in modules)
        {
            Console.WriteLine($"Building module {module.Name}...");
            RunDotnetBuild(module.ProjectPath);
        }

        foreach (var module in modules)
        {
            Console.WriteLine($"Copying {module.Name} outputs...");

            switch (module.Type)
            {
                case ModuleRole.Client:
                    CopyModuleOutputs(module, "bin/Content.Client");
                    break;
                case ModuleRole.Server:
                    CopyModuleOutputs(module, "bin/Content.Server");
                    break;
                case ModuleRole.Shared:
                case ModuleRole.Common:
                    CopyModuleOutputs(module, "bin/Content.Client", "bin/Content.Server");
                    break;
            }
        }

        Console.WriteLine("Build complete.");
        return Task.CompletedTask;
    }

    public static Task BuildClient()
    {
        var modules = ModuleDiscovery.DiscoverModules().Where(module => module.Type is not ModuleRole.Server).ToList();

        Console.WriteLine("Building core projects...");
        RunDotnetBuild("Content.Client/Content.Client.csproj");

        foreach (var module in modules)
        {
            Console.WriteLine($"Building module {module.Name}...");
            RunDotnetBuild(module.ProjectPath);
        }

        foreach (var module in modules)
        {
            Console.WriteLine($"Copying {module.Name} outputs...");
            CopyModuleOutputs(module, "bin/Content.Client");
        }

        Console.WriteLine("Build complete.");
        return Task.CompletedTask;
    }

    public static Task BuildServer()
    {
        var modules = ModuleDiscovery.DiscoverModules().Where(module => module.Type is not ModuleRole.Client).ToList();

        Console.WriteLine("Building core projects...");
        RunDotnetBuild("Content.Server/Content.Server.csproj");

        foreach (var module in modules)
        {
            Console.WriteLine($"Building module {module.Name}...");
            RunDotnetBuild(module.ProjectPath);
        }

        foreach (var module in modules)
        {
            Console.WriteLine($"Copying {module.Name} outputs...");
            CopyModuleOutputs(module, "bin/Content.Server");
        }

        Console.WriteLine("Build complete.");
        return Task.CompletedTask;
    }

    private static void RunDotnetBuild(string projectPath)
    {
        var psi = new ProcessStartInfo
        {
            FileName = DotnetPath,
            Arguments = $"build {projectPath} -c Debug --nologo /v:m /m",
            UseShellExecute = false
        };

        using var process = Process.Start(psi);
        if (process == null)
            throw new Exception($"Failed to start dotnet build for {projectPath}");

        process.WaitForExit();

        if (process.ExitCode != 0)
            throw new Exception($"Build failed for {projectPath} with exit code {process.ExitCode}");
    }

    private static void CopyModuleOutputs(ModuleDiscovery.ModuleInfo module, params string[] targetBinDirs)
    {
        var moduleOutputDir = ModuleDiscovery.GetModuleOutputDir(module.ProjectPath);

        foreach (var targetBinDir in targetBinDirs)
        {
            var coreName = Path.GetFileName(targetBinDir);
            var coreDepsPath = Path.Combine(targetBinDir, $"{coreName}.deps.json");
            var moduleDepsPath = Path.Combine(moduleOutputDir, $"{module.Name}.deps.json");

            var uniqueDlls = DepsHandler.GetModuleUniqueDlls(coreDepsPath, moduleDepsPath);

            foreach (var dll in uniqueDlls)
            {
                var sourceDll = Path.Combine(moduleOutputDir, dll);
                var targetDll = Path.Combine(targetBinDir, dll);

                if (File.Exists(sourceDll))
                    File.Copy(sourceDll, targetDll, overwrite: true);

                var sourcePdb = Path.ChangeExtension(sourceDll, ".pdb");
                var targetPdb = Path.ChangeExtension(targetDll, ".pdb");

                if (File.Exists(sourcePdb))
                    File.Copy(sourcePdb, targetPdb, overwrite: true);
            }

            var moduleAssembly = $"{module.Name}.dll";
            var moduleAssemblyPdb = $"{module.Name}.pdb";

            var sourceModuleDll = Path.Combine(moduleOutputDir, moduleAssembly);
            var targetModuleDll = Path.Combine(targetBinDir, moduleAssembly);

            if (File.Exists(sourceModuleDll))
                File.Copy(sourceModuleDll, targetModuleDll, overwrite: true);

            var sourceModulePdb = Path.Combine(moduleOutputDir, moduleAssemblyPdb);
            var targetModulePdb = Path.Combine(targetBinDir, moduleAssemblyPdb);

            if (File.Exists(sourceModulePdb))
                File.Copy(sourceModulePdb, targetModulePdb, overwrite: true);
        }
    }
}
