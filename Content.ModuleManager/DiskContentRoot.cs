using System.Diagnostics.CodeAnalysis;
using Robust.Shared.ContentPack;
using Robust.Shared.Utility;

namespace Content.ModuleManager;

/// <summary>
/// Mounts a directory on disk as a VFS content root.
/// </summary>
public sealed class DiskContentRoot : IContentRoot
{
    private readonly DirectoryInfo _directory;

    public DiskContentRoot(string directoryPath)
    {
        _directory = new DirectoryInfo(directoryPath);
    }

    public void Mount()
    {
    }

    public bool TryGetFile(ResPath relPath, [NotNullWhen(true)] out Stream? stream)
    {
        var path = GetPath(relPath);
        if (!File.Exists(path))
        {
            stream = null;
            return false;
        }

        try
        {
            stream = File.OpenRead(path);
            return true;
        }
        catch (FileNotFoundException)
        {
            stream = null;
            return false;
        }
        catch (DirectoryNotFoundException)
        {
            stream = null;
            return false;
        }
    }

    public bool FileExists(ResPath relPath)
    {
        return File.Exists(GetPath(relPath));
    }

    public IEnumerable<ResPath> FindFiles(ResPath path)
    {
        var fullPath = GetPath(path);
        if (!Directory.Exists(fullPath))
            yield break;

        foreach (var filePath in Directory.EnumerateFiles(fullPath, "*", SearchOption.AllDirectories))
        {
            var relPath = filePath.Substring(_directory.FullName.Length);
            yield return ResPath.FromRelativeSystemPath(relPath);
        }
    }

    public IEnumerable<string> GetRelativeFilePaths()
    {
        return EnumerateRelativeFilePaths(_directory);
    }

    private IEnumerable<string> EnumerateRelativeFilePaths(DirectoryInfo dir)
    {
        foreach (var file in dir.EnumerateFiles())
        {
            if ((file.Attributes & FileAttributes.Hidden) != 0 || file.Name[0] == '.')
                continue;

            var relPath = file.FullName.Substring(_directory.FullName.Length);
            yield return ResPath.FromRelativeSystemPath(relPath).ToRootedPath().ToString();
        }

        foreach (var subDir in dir.EnumerateDirectories())
        {
            if ((subDir.Attributes & FileAttributes.Hidden) != 0 || subDir.Name[0] == '.')
                continue;

            foreach (var relPath in EnumerateRelativeFilePaths(subDir))
                yield return relPath;
        }
    }

    private string GetPath(ResPath relPath)
    {
        var relSysPath = relPath.ToRelativeSystemPath();

        if (relSysPath.Contains("\\..") || relSysPath.Contains("/..") || relSysPath.StartsWith(".."))
            throw new InvalidOperationException($"Invalid path traversal: {relPath}");

        var fullPath = Path.GetFullPath(Path.Join(_directory.FullName, relSysPath));
        if (!fullPath.StartsWith(_directory.FullName)
            && fullPath != _directory.FullName.TrimEnd(Path.DirectorySeparatorChar))
        {
            throw new InvalidOperationException($"Invalid path traversal: {relPath}");
        }

        return fullPath;
    }
}
