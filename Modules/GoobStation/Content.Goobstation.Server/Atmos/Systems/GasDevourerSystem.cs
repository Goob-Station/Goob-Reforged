using Content.Goobstation.Common.Atmos;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Unary.EntitySystems;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Piping.Unary.Components;
using Robust.Server.GameObjects;

namespace Content.Goobstation.Server.Atmos;

public sealed partial class GasDevourerSystem : EntitySystem
{
    [Dependency] private EntityQuery<GasMixtureHolderComponent> _gasHolderQuery = default!;

    [Dependency] private AtmosphereSystem _atmosphereSystem = default!;
    [Dependency] private GasVentScrubberSystem _scrubberSystem = default!;
    [Dependency] private TransformSystem _transformSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GasDevourerComponent, AtmosDeviceUpdateEvent>(OnAtmosUpdated);
    }

    private void OnAtmosUpdated(Entity<GasDevourerComponent> devourer, ref AtmosDeviceUpdateEvent args)
    {
        // Return if entity cannot hold gas or we are not on grid
        if (!_gasHolderQuery.TryComp(devourer, out var gasHolder) || args.Grid is not { } grid)
            return;

        var position = _transformSystem.GetGridTilePositionOrDefault(devourer.Owner);
        var gasEnumerator = _atmosphereSystem.GetAdjacentTileMixtures(grid, position, false, true);

        while (gasEnumerator.MoveNext(out var tileMixture))
        {
            _scrubberSystem.Scrub(args.dt, devourer.Comp.TransferRate * _atmosphereSystem.PumpSpeedup(),
                ScrubberPumpDirection.Siphoning, [], tileMixture, gasHolder.Air);
        }
    }
}
