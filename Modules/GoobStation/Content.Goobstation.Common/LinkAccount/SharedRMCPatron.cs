using Robust.Shared.Serialization;

namespace Content.Goobstation.Common.LinkAccount;

[Serializable, NetSerializable]
public sealed class SharedRMCPatron(string name, string tier)
{
    public readonly string Name = name;
    public readonly string Tier = tier;
}
