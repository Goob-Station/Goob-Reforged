// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Xenoarchaeology.Artifact;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.WoundMod.Shared.StatusEffects;

namespace Content.WoundMod.Server.StatusEffects;

public sealed class ActivateArtifactEffectSystem : EntitySystem
{
    [Dependency] private readonly SharedXenoArtifactSystem _artifact = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<ActivateArtifactEffectComponent, ComponentInit>(OnInit);
    }
    private void OnInit(EntityUid uid, ActivateArtifactEffectComponent component, ComponentInit args)
    {
        if (!TryComp<XenoArtifactComponent>(uid, out var artifact))
            return;

        _artifact.TryActivateXenoArtifact((uid,artifact), null, null, Transform(uid).Coordinates, false);
    }


}
