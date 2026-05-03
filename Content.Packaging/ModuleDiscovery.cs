using Content.ModuleManager;

namespace Content.Packaging;

public static class ModuleDiscovery
{
    public record ModuleInfo(string Name, string ProjectPath, ModuleRole Type);

    public static string GetModuleOutputDir(string projectPath)
    {
        var projectDir = Path.GetDirectoryName(projectPath)!;
        var withTfm = Path.Combine(projectDir, "bin", "Debug", "net10.0");
        if (Directory.Exists(withTfm))
            return withTfm;
        return Path.Combine(projectDir, "bin", "Debug");
    }

    /// <summary>
    /// Discovers all modules by scanning for module.yml files in the Modules/ directory
    /// </summary>
    public static IEnumerable<ModuleInfo> DiscoverModules(string basePath = ".")
    {
        var modulesPath = Path.Combine(basePath, "Modules");

        if (!Directory.Exists(modulesPath))
        {
            yield break;
        }

        foreach (var manifestPath in Directory.GetFiles(modulesPath, "module.yml", SearchOption.AllDirectories))
        {
            ModuleManifest manifest;
            try
            {
                manifest = ModuleManifestLoader.LoadFromFile(manifestPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to load module manifest {manifestPath}: {ex.Message}");
                continue;
            }

            if (manifest.Disabled)
                continue;

            foreach (var project in manifest.Projects)
            {
                var projectPath = ModuleManifestLoader.GetProjectPath(project, manifest.ModuleDirectory);

                if (!File.Exists(projectPath))
                {
                    Console.WriteLine($"Warning: Project file not found: {projectPath}");
                    continue;
                }

                yield return new ModuleInfo(
                    ModuleManifestLoader.GetProjectName(project),
                    projectPath,
                    project.Role
                );
            }
        }
    }
}
