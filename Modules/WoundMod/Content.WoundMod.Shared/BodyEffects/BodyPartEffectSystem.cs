// SPDX-FileCopyrightText: 2024 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Body.Part;
using Content.WoundMod.Shared.Body.Components;
using Content.WoundMod.Shared.Body.Events;
using Content.WoundMod.Shared.Body.Part;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;

namespace Content.WoundMod.Shared.BodyEffects;
public partial class BodyPartEffectSystem : EntitySystem
{
    [Dependency] private readonly IComponentFactory _compFactory = default!;
    [Dependency] private readonly ISerializationManager _serManager = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BodyPartComponent, BodyPartComponentsModifyEvent>(OnPartComponentsModify);
    }

    // While I would love to kill this function, problem is that if we happen to have two parts that add the same
    // effect, removing one will remove both of them, since we cant tell what the source of a Component is.
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<BodyPartEffectComponent, BodyPartComponent>();
        var now = _gameTiming.CurTime;
        while (query.MoveNext(out var uid, out var comp, out var part))
        {
            if (now < comp.NextUpdate || !comp.Active.Any() || part.Body is not { } body)
                continue;

            comp.NextUpdate = now + comp.Delay;
            AddComponents(body, uid, comp.Active);
        }
    }

    private void OnPartComponentsModify(Entity<BodyPartComponent> partEnt, ref BodyPartComponentsModifyEvent ev)
    {
        if (!TryComp<WMBodyPartComponent>(partEnt, out var part))
            return;

        if (part.OnAdd != null)
        {
            if (ev.Add)
                AddComponents(ev.Body, partEnt, part.OnAdd);
            else
                RemoveComponents(ev.Body, partEnt, part.OnAdd);
        }

        if (part.OnRemove != null)
        {
            if (ev.Add)
                AddComponents(ev.Body, partEnt, part.OnRemove);
            else
                RemoveComponents(ev.Body, partEnt, part.OnRemove);
        }

        Dirty(partEnt, part);
    }

    private void AddComponents(EntityUid body,
        EntityUid part,
        ComponentRegistry reg,
        BodyPartEffectComponent? effectComp = null)
    {
        if (!Resolve(part, ref effectComp, logMissing: false))
            return;

        foreach (var (key, comp) in reg)
        {
            var compType = comp.Component.GetType();
            if (HasComp(body, compType))
                continue;

            var newComp = (Component) _serManager.CreateCopy(comp.Component, notNullableOverride: true);
            EntityManager.AddComponent(body, newComp, true);

            effectComp.Active[key] = comp;
        }
    }

    private void RemoveComponents(EntityUid body,
        EntityUid part,
        ComponentRegistry reg,
        BodyPartEffectComponent? effectComp = null)
    {
        if (!Resolve(part, ref effectComp, logMissing: false))
            return;

        foreach (var (key, comp) in reg)
        {
            RemComp(body, comp.Component.GetType());
            effectComp.Active.Remove(key);
        }
    }
}
