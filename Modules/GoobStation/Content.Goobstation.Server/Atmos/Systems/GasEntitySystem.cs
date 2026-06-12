using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Server.GameTicking;
using Content.Shared.Atmos.Prototypes;
using Robust.Shared.Prototypes;

/// <summary>
/// System that creates and store entities of specific gases just to allow gases work with components
/// </summary>
public sealed partial class GasEntitySystem : EntitySystem
{
    [Dependency] private IPrototypeManager _protoMan = default!;
    private FrozenDictionary<ProtoId<GasPrototype>, EntityUid> _gasesDict = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<PostGameMapLoad>(AfterMapLoad);
        SubscribeLocalEvent<PrototypesReloadedEventArgs>(OnPrototypesReload);
    }

    private void AfterMapLoad(PostGameMapLoad args)
    {
        if (_gasesDict == null || _gasesDict.Count == 0)
            FillGasesDictionary();
    }

    private void OnPrototypesReload(PrototypesReloadedEventArgs args)
    {
        FillGasesDictionary();
    }

    private void FillGasesDictionary()
    {
        var gases = _protoMan.EnumeratePrototypes<GasPrototype>();
        Dictionary<ProtoId<GasPrototype>, EntityUid> gasesDict = [];

        foreach (var gas in gases)
        {
            if (!_protoMan.TryIndex<EntityPrototype>(gas.ID, out var gasEntProto))
            {
                Log.Error($"GasPrototype «{gas.ID}» does not have correlation entity prototype with same ID.");
                continue;
            }

            // Spawn gas entity with same gas ID and put it in dictionary
            var gasEnt = Spawn(gasEntProto.ID);
            gasesDict.Add(gas.ID, gasEnt);
        }

        _gasesDict = gasesDict.ToFrozenDictionary();
    }

    public bool TryGetGasEntity(string gasId, [NotNullWhen(true)] out EntityUid? gasEnt)
    {
        if (_gasesDict.TryGetValue(gasId, out var outValue))
        {
            gasEnt = outValue;
            return true;
        }
        gasEnt = null;
        return false;
    }
}