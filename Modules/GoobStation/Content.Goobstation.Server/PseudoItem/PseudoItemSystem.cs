// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: LicenseRef-GAG-1.0.txt

using Content.Goobstation.Shared.PseudoItem;
using Content.Shared.Hands.EntitySystems;

namespace Content.Goobstation.Server.PseudoItem;

public sealed class PseudoItemSystem : SharedPseudoItemSystem
{
    //[Dependency] private SharedHandsSystem _hands = default!;
    //[Dependency] private  CarryingSystem _carrying = default!;
    //[Dependency] private SharedTransformSystem _xform = default!;

    /* todo marty carry
    [SubscribeLocalEvent]
    private void OnGettingPickedUpAttempt(EntityUid uid, PseudoItemComponent component, GettingPickedUpAttemptEvent args)
    {
        // Try to pick the entity up instead first
        if (args.User != args.Item && _carrying.TryCarry(args.User, uid))
        {
            args.Cancel();
            return;
        }

        // If could not pick up, just take it out onto the ground as per default
        _xform.AttachToGridOrMap(uid);
        args.Cancel();
    }*/
}
