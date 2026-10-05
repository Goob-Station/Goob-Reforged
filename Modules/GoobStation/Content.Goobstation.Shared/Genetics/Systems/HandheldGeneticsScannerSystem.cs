// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Genetics.Components;
using Content.Goobstation.Shared.Genetics.Prototypes;
using Content.Shared.Chat;
using Content.Shared.DoAfter;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.MedicalScanner;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Goobstation.Shared.Genetics.Systems;

public sealed partial class HandheldGeneticsScannerSystem : EntitySystem
{
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private SharedDoAfterSystem _doAfter = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedChatSystem _chat = default!;
    [Dependency] private MutationSequenceSystem _mutationSequence = default!;
    [Dependency] private IPrototypeManager _proto = default!;

    [SubscribeLocalEvent]
    private void OnHandheldInteract(Entity<HandheldGeneticsScannerComponent> ent,
        ref AfterInteractEvent args)
    {
        if (args.Target is not { } target)
            return;

        _audio.PlayPredicted(ent.Comp.ScanningBeginSound, ent, args.User);

        if (!HasComp<MutableComponent>(target))
        {
            _popup.PopupEntity(
                Loc.GetString(ent.Comp.LocNotMutable),
                args.Target.Value,
                args.Target.Value,
                PopupType.Medium
            );
            return;
            // TODO: different error message for targets that arent even organic, maybe call them stupid??
        }

        var doAfterArgs = new DoAfterArgs(
            EntityManager,
            args.User,
            ent.Comp.ScanDelay,
            new HealthAnalyzerDoAfterEvent(),
            ent,
            target
        );

        if (!_doAfter.TryStartDoAfter(doAfterArgs))
            return;

        var msg = Loc.GetString(
            "handheld-genetics-scanner-popup-scan-target",
            ("user", Identity.Entity(args.User, EntityManager))
        );

        _popup.PopupEntity(msg, args.Target.Value, args.Target.Value, PopupType.Medium);
    }

    [SubscribeLocalEvent]
    private void OnDoAfter(Entity<HandheldGeneticsScannerComponent> ent,
        ref HealthAnalyzerDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled || args.Target is null)
            return;

        if (!TryComp<MutableComponent>(args.Target, out var mutable))
        {
            throw new InvalidOperationException("No MutableComponent on target!"); // good enough
        }

        args.Handled = true;

        _audio.PlayPredicted(ent.Comp.ScanningEndSound, ent, args.User);

        FormattedMessage message = [];

        message.AddMarkupOrThrow($"[color=DarkGray][font size=10]{Loc.GetString(ent.Comp.LocReportTitle)}[/font][/color]");

        // TODO: cleanup
        // TODO: mutation names requiring discovery
        // todo: I WANT THIS TO BE !! ChatMessageToOne WITH MARKUP !! BUT YOU CANT FUCKING DO IT FROM SHARED BRO
        // fucking fix this todo but this is wip code anyways so i dont care rn
        List<ProtoId<MutationPrototype>> addedMutations = [];
        foreach (var mutation in mutable.InitialMutations)
        {
            addedMutations.Add(mutation);
            var isActive = mutable.ActiveMutations.Contains(mutation);

            if (isActive)
                message.AddMarkupOrThrow("[bold]");

            var data = _proto.Index(mutation);

            string? knownName;

            // todo check server too and rewrite and bullshit
            if (data.Name is not null)
                knownName = data.Name;
            else
                knownName = $"Mutation {_mutationSequence.GetSequence(data).NumDisplayedID}";

            message.AddMarkupOrThrow(knownName);

            if (isActive)
                message.AddMarkupOrThrow("[/bold]");

            message.PushNewline();
        }

        message.Pop();
        Log.Info(message.ToString());
        _chat.TrySendInGameICMessage(ent, message.ToString(), InGameICChatType.Speak, false, true);
    }
}

[Serializable, NetSerializable]
public sealed partial class HandheldGeneticsScannerDoAfterEvent : SimpleDoAfterEvent;
