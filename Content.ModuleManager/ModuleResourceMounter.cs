using Robust.Shared.ContentPack;
using Robust.Shared.Log;
using Robust.Shared.Utility;

namespace Content.ModuleManager;

/// <summary>
/// Discovers module manifests at runtime and mounts their resource directories into the VFS.
/// </summary>
public static class ModuleResourceMounter
{
    public static void MountAll(IResourceManager resourceManager, ISawmill sawmill)
    {
        var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../"));
        var modulesPath = Path.Combine(basePath, "Modules");
        if (!Directory.Exists(modulesPath))
            return;

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
                sawmill.Error($"Failed to load module manifest {manifestPath}: {ex.Message}");
                continue;
            }

            if (manifest.Disabled)
                continue;

            manifests.Add(manifest);
        }

        manifests.Sort((a, b) => string.CompareOrdinal(a.Id, b.Id));

        foreach (var manifest in manifests)
        {
            MountManifest(resourceManager, sawmill, manifest);
        }
    }

    private static void MountManifest(IResourceManager resourceManager, ISawmill sawmill, ModuleManifest manifest)
    {
        var declared = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var relativeResourcePath in manifest.Resources)
        {
            var resourcePath = Path.GetFullPath(Path.Combine(manifest.ModuleDirectory, relativeResourcePath));
            if (!Directory.Exists(resourcePath))
            {
                sawmill.Warning(
                    $"Module '{manifest.Id}' declares resource path '{relativeResourcePath}' which does not exist on disk: {resourcePath}");
                continue;
            }

            declared.Add(Path.GetFullPath(resourcePath));
            resourceManager.AddRoot(ResPath.Root, new DiskContentRoot(resourcePath));
            sawmill.Debug($"Mounted module resources: {manifest.Id} -> {resourcePath}");
        }

        var conventionalResources = Path.GetFullPath(Path.Combine(manifest.ModuleDirectory, "Resources"));
        if (Directory.Exists(conventionalResources) && !declared.Contains(conventionalResources))
        {
            sawmill.Warning(
                $"Module '{manifest.Id}' has a 'Resources/' directory on disk but it is not declared in module.yml. " +
                $"Add it under 'resources:' to mount it, or remove it. Path: {conventionalResources}");
        }
    }
}
