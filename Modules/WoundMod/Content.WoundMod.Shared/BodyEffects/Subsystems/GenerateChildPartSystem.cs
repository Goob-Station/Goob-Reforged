// SPDX-FileCopyrightText: 2024 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Content.Shared.Body.Part;
using Content.Shared.Body.Systems;
using Content.WoundMod.Shared.Body.Events;
using Content.WoundMod.Shared.Body.Systems;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.WoundMod.Shared.BodyEffects.Subsystems;

public sealed class GenerateChildPartSystem : EntitySystem
{
    [Dependency] private readonly SharedBodySystem _bodySystem = default!;
    [Dependency] private readonly SharedWMBodySystem _wmBodySystem = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly INetManager _net = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GenerateChildPartComponent, BodyPartComponentsModifyEvent>(OnPartComponentsModify);
    }

    private void OnPartComponentsModify(EntityUid uid, GenerateChildPartComponent component, ref BodyPartComponentsModifyEvent args)
    {
        if (args.Add)
            CreatePart(uid, component);
        //else
            //DeletePart(uid, component);
    }

    private void CreatePart(EntityUid uid, GenerateChildPartComponent component)
    {
        if (!TryComp(uid, out BodyPartComponent? partComp)
            || partComp.Body is null
            || component.Active)
            return;

        // I pinky swear to also move this to the server side properly next update :)
        if (!_net.IsServer)
            return;
        var childPart = Spawn(component.Id, new EntityCoordinates(partComp.Body.Value, Vector2.Zero));

        if (!TryComp(childPart, out BodyPartComponent? childPartComp))
            return;

        var slotName = _wmBodySystem.GetSlotFromBodyPart(uid);
        _bodySystem.TryCreatePartSlot(uid, slotName, childPartComp.PartType, out var _);
        _bodySystem.AttachPart(uid, slotName, childPart, partComp, childPartComp);
        component.ChildPart = childPart;
        component.Active = true;
        Dirty(childPart, childPartComp);
    }

    // Still unusued, gotta figure out what I want to do with this function outside of fuckery with mantis blades.
    private void DeletePart(EntityUid uid, GenerateChildPartComponent component)
    {
        if (!TryComp(uid, out BodyPartComponent? partComp))
            return;

        _wmBodySystem.DropSlotContents((uid, partComp));
        var ev = new BodyPartDroppedEvent((uid, partComp));
        RaiseLocalEvent(uid, ref ev);
        QueueDel(uid);
    }
}
