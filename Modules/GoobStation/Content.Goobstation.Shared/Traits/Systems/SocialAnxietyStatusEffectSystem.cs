// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Shared.IdentityManagement;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Content.Shared.StatusEffectNew;
using Content.Shared.Stunnable;

namespace Content.Goobstation.Shared.Traits.Components;

public sealed partial class SocialAnxietyStatusEffectSystem : EntitySystem
{
    [Dependency] private StandingStateSystem _standing = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedStunSystem _stun = default!;

    [SubscribeLocalEvent]
    private void OnHug(Entity<SocialAnxietyStatusEffectComponent> ent,
        ref StatusEffectRelayedEvent<InteractionSuccessEvent> args)
    {
        _standing.Down(args.AppliedTo);
        _stun.TryUpdateStunDuration(args.AppliedTo, TimeSpan.FromSeconds(ent.Comp.DownedTime));

        var targetName = Identity.Name(args.AppliedTo, EntityManager);
        var performerName = Identity.Name(args.Args.User, EntityManager);

        if (ent.Comp.DownedPopup is null)
            return;

        var msg = Loc.GetString(
            ent.Comp.DownedPopup,
            ("target", targetName),
            ("performer", performerName)
        );

        _popup.PopupEntity(msg, args.AppliedTo, PopupType.MediumCaution);
    }
}
