using System.Text.Json;
using System.Text.Json.Serialization;

namespace Content.Packaging;

/// <summary>
/// Helper class for working with <c>.deps.json</c> files.
/// </summary>
public sealed class DepsHandler
{
    public readonly Dictionary<string, LibraryInfo> Libraries = new();

    public DepsHandler(DepsData data)
    {
        if (data.Targets.Count != 1)
            throw new Exception("Expected exactly one target");

        var target = data.Targets.Single().Value;

        foreach (var (libNameAndVersion, libInfo) in target)
        {
            var split = libNameAndVersion.Split('/', 2);

            Libraries.Add(split[0], libInfo);
        }
    }

    public static DepsHandler Load(string depsFile)
    {
        using var f = File.OpenRead(depsFile);
        var depsData = JsonSerializer.Deserialize<DepsData>(f) ?? throw new InvalidOperationException("Deps are null!");

        return new DepsHandler(depsData);
    }

    public HashSet<string> RecursiveGetLibrariesFrom(string start)
    {
        var found = new HashSet<string>();

        RecursiveAddLibraries(start, found);

        return found;
    }

    private void RecursiveAddLibraries(string start, HashSet<string> set)
    {
        if (!set.Add(start))
            return;

        var lib = Libraries[start];
        if (lib.Dependencies == null)
            return;

        foreach (var dep in lib.Dependencies.Keys)
        {
            RecursiveAddLibraries(dep, set);
        }
    }

    public static HashSet<string> GetModuleUniqueAssemblies(string coreDepsPath, string moduleDepsPath)
    {
        var core = Load(coreDepsPath);
        var module = Load(moduleDepsPath);

        var unique = new HashSet<string>(module.Libraries.Keys);
        unique.ExceptWith(core.Libraries.Keys);

        return unique;
    }

    public static IEnumerable<string> GetModuleUniqueDlls(string coreDepsPath, string moduleDepsPath)
    {
        var core = Load(coreDepsPath);
        var module = Load(moduleDepsPath);

        var uniqueLibs = new HashSet<string>(module.Libraries.Keys);
        uniqueLibs.ExceptWith(core.Libraries.Keys);

        return uniqueLibs
            .Where(lib => module.Libraries.ContainsKey(lib))
            .SelectMany(lib => module.Libraries[lib].GetDllNames());
    }

    public sealed class DepsData
    {
        [JsonInclude, JsonPropertyName("targets")]
        public required Dictionary<string, Dictionary<string, LibraryInfo>> Targets;
    }

    public sealed class LibraryInfo
    {
        [JsonInclude, JsonPropertyName("dependencies")]
        public Dictionary<string, string>? Dependencies;

        [JsonInclude, JsonPropertyName("runtime")]
        public Dictionary<string, object>? Runtime;

        // Paths are like lib/netstandard2.0/JetBrains.Annotations.dll
        public IEnumerable<string> GetDllNames()
        {
            return Runtime == null ? [] : Runtime.Keys.Select(p => p.Split('/')[^1]);
        }
    }
}
