using System.Linq;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Reactions;
using Content.Shared.Atmos.Components;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Server.Atmos;

public sealed partial class GasReactionChamberSystem : EntitySystem
{
    [Dependency] private AtmosphereSystem _atmosphereSystem = default!;
    [Dependency] private IPrototypeManager _prototypeMan = default!;
    [Dependency] private EntityQuery<GasMixtureHolderComponent> _gasHolderQuery = default!;

    private GasReactionPrototype[] _gasReactions = [];

    /// <summary>
    ///     List of gas reactions ordered by priority.
    /// </summary>
    public IEnumerable<GasReactionPrototype> GasReactions => _gasReactions;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GasReactionChamberComponent, AtmosDeviceUpdateEvent>(OnAtmosUpdate);

        _gasReactions = _prototypeMan.EnumeratePrototypes<GasReactionPrototype>().ToArray();
        Array.Sort(_gasReactions, (a, b) => b.Priority.CompareTo(a.Priority));
    }

    private void OnAtmosUpdate(Entity<GasReactionChamberComponent> chamber, ref AtmosDeviceUpdateEvent args)
    {
        if (chamber.Comp.Reactions is null || !_gasHolderQuery.TryComp(chamber, out var gasHolderComp))
            return;

        var reactGases = GasReactions.Where(x => chamber.Comp.Reactions.Contains(x.ID));

        _atmosphereSystem.React(gasHolderComp.Air, gasHolderComp, reactGases);
    }
}