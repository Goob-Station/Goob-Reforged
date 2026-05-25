using Content.ModuleManager;

namespace Content.Packaging;

public static class ModuleResourceCopier
{
    public static IEnumerable<(ModuleManifest Manifest, string ResourcePath)> EnumerateResourcePaths(string contentDir)
    {
        var modulesPath = Path.Combine(contentDir, "Modules");
        if (!Directory.Exists(modulesPath))
            yield break;

        var manifests = new List<ModuleManifest>();
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

            manifests.Add(manifest);
        }

        manifests.Sort((a, b) => string.CompareOrdinal(a.Id, b.Id));

        foreach (var manifest in manifests)
        {
            var declared = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var rel in manifest.Resources)
            {
                var resourcePath = Path.GetFullPath(Path.Combine(manifest.ModuleDirectory, rel));
                if (!Directory.Exists(resourcePath))
                {
                    Console.WriteLine(
                        $"Warning: Module '{manifest.Id}' declares resource path '{rel}' which does not exist: {resourcePath}");
                    continue;
                }

                declared.Add(resourcePath);
                yield return (manifest, resourcePath);
            }

            var conventional = Path.GetFullPath(Path.Combine(manifest.ModuleDirectory, "Resources"));
            if (Directory.Exists(conventional) && !declared.Contains(conventional))
            {
                Console.WriteLine(
                    $"Warning: Module '{manifest.Id}' has a 'Resources/' directory on disk but it is not declared in module.yml. " +
                    $"Add it under 'resources:' to package it. Path: {conventional}");
            }
        }
    }
}
