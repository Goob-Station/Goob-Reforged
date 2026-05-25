using System.Collections.Concurrent;
using Robust.Packaging.AssetProcessing;

namespace Content.Packaging;

public sealed class AssetPassLastWriteWins : AssetPass
{
    private readonly ConcurrentDictionary<string, AssetFile> _files = new(StringComparer.Ordinal);

    protected override AssetFileAcceptResult AcceptFile(AssetFile file)
    {
        _files[file.Path] = file;
        return AssetFileAcceptResult.Consumed;
    }

    protected override void AcceptFinished()
    {
        foreach (var file in _files.Values)
        {
            SendFile(file);
        }
    }
}
