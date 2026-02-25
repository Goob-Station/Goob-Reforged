// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Supermatter.Components;
using Content.Goobstation.Shared.Supermatter.Systems;
using Content.Server.AlertLevel;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Audio;
using Content.Server.Chat.Systems;
using Content.Server.DoAfter;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Lightning;
using Content.Server.Station.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos;
using Content.Shared.Chat;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Kitchen.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Projectiles;
using Content.Shared.Radiation.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;
using System.Text;

namespace Content.Goobstation.Server.Supermatter.Systems;

public sealed class SupermatterSystem : SharedSupermatterSystem
{
    [Dependency] private static readonly AtmosphereSystem Atmosphere = default!; // Static since enum uses it to implement IDisposable
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly ExplosionSystem _explosion = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly AmbientSoundSystem _ambient = default!;
    [Dependency] private readonly LightningSystem _lightning = default!;
    [Dependency] private readonly AlertLevelSystem _alert = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly DoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLog = default!;
    [Dependency] private readonly ISharedChatManager _sharedChat = default!;

    private DelamType _delamType = DelamType.Explosion;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SupermatterComponent, ComponentRemove>(OnComponentRemove);
        SubscribeLocalEvent<SupermatterComponent, MapInitEvent>(OnMapInit);

        SubscribeLocalEvent<SupermatterComponent, StartCollideEvent>(OnCollideEvent);
        SubscribeLocalEvent<SupermatterComponent, InteractHandEvent>(OnHandInteract);
        SubscribeLocalEvent<SupermatterComponent, InteractUsingEvent>(OnItemInteract);
        SubscribeLocalEvent<SupermatterComponent, ExaminedEvent>(OnExamine);
        SubscribeLocalEvent<SupermatterComponent, SupermatterDoAfterEvent>(OnGetSliver);
    }

    private void OnComponentRemove(EntityUid uid, SupermatterComponent component, ComponentRemove args)
    {
        // turn off any ambient if component is removed (ex. entity deleted)
        _ambient.SetAmbience(uid, false);
        component.AudioStream = _audio.Stop(component.AudioStream);
    }

    private void OnMapInit(EntityUid uid, SupermatterComponent component, MapInitEvent args)
    {
        // Set the Sound
        _ambient.SetAmbience(uid, true);

        //Add Air to the initialized SM in the Map so it doesnt delam on default
        var mix = Atmosphere.GetContainingMixture(uid, true, true);
        mix?.AdjustMoles(Gas.Oxygen, Atmospherics.OxygenMolesStandard);
        mix?.AdjustMoles(Gas.Nitrogen, Atmospherics.NitrogenMolesStandard);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_gameTiming.IsFirstTimePredicted)
            return;

        var smEqe = EntityManager.EntityQueryEnumerator<SupermatterComponent>();

        while (smEqe.MoveNext(out var entity, out var comp))
        {
            if (!comp.Activated)
                continue;

            comp.UpdateAccumulator += frameTime;

            if (comp.UpdateAccumulator >= comp.UpdateTimer)
            {
                comp.UpdateAccumulator -= comp.UpdateTimer;
                Cycle(entity, comp);
            }
        }
    }

    public void Cycle(EntityUid ent, SupermatterComponent comp)
    {
        comp.ZapAccumulator++;
        comp.YellAccumulator++;

        ProcessAtmos(ent, comp);
        HandleDamage(ent, comp);

        if (comp.Damage >= comp.DelaminationPoint || comp.Delamming)
            HandleDelamination(ent, comp);

        HandleSoundLoop(comp);

        if (comp.ZapAccumulator >= comp.ZapTimer)
        {
            comp.ZapAccumulator -= comp.ZapTimer;
            SupermatterZap(ent, comp);
        }

        if (comp.YellAccumulator >= comp.YellTimer)
        {
            comp.YellAccumulator -= comp.YellTimer;
            HandleAnnouncements(ent, comp);
        }
    }

    #region Processing

    /// <summary>
    ///     Handle power and radiation output depending on atmospheric things.
    /// </summary>
    private void ProcessAtmos(EntityUid uid, SupermatterComponent sm)
    {
        #region Get gas mix

        var mix = Atmosphere.GetContainingMixture(uid, true, true);

        if (mix is not { })
            return;

        using var absorbed = new GasWrapper(mix, sm.GasEfficiency);

        var moles = absorbed.Gas.TotalMoles;

        if (!(moles > 0f))
            return;

        #endregion

        var (radModifier, zapModifier, moleModifier, heatModifier, heatResistModifier) = absorbed.Gas.GetGasModifiers();

        #region Calculate CO2 powerloss inhibition effect

        var co2Ratio = absorbed.Gas.GetGasMolarPercentage(Gas.CarbonDioxide);
        var underThresholdScaler = Math.Min(
            Math.Clamp(co2Ratio / sm.PowerlossInhibitionGasThreshold, 0, 1),
            Math.Clamp(moles / sm.PowerlossInhibitionMoleThreshold, 0, 1)
            );

        // Apply CO2 ratio if thresholds are met, otherwise limit the ratio according to how far away we are from thresholds
        sm.PowerlossDynamicScaling = co2Ratio * underThresholdScaler;

        // 
        var moleBoost = Math.Clamp(moles / sm.PowerlossInhibitionMoleBoostThreshold, 1f, 1.5f);
        var powerlossInhibitor = Math.Clamp(1f - sm.PowerlossDynamicScaling * moleBoost, 0f, 1f);

        #endregion

        #region Add power to crystal

        // Transfer matter power to power
        if (sm.MatterPower != 0)
        {
            // Get how much matter power to transfer
            var removedMatter = Math.Clamp(sm.MatterPower, 0f, 1f * sm.MatterPowerConversion);

            sm.Power = Math.Max(sm.Power + removedMatter, 0);
            sm.MatterPower = Math.Max(sm.MatterPower - removedMatter, 0);
        }

        // Increase power from temperature
        sm.Power = Math.Max(absorbed.Gas.Temperature * heatModifier / Atmospherics.T0C + sm.Power, 0);

        // Yeah, it consumes all ammonia in one tick cuz it's funny af
        sm.Power = Math.Max(absorbed.Gas.GetMoles(Gas.Ammonia) * sm.AmmoniaEnergyPerMole + sm.Power, 0);
        absorbed.Gas.SetMoles(Gas.Ammonia, 0f);

        #endregion

        #region Generate outputs

        //Radiate stuff
        if (TryComp<RadiationSourceComponent>(uid, out var rad))
        {
            rad.Intensity = sm.Power * radModifier * sm.RadiationOutputFactor;
        }

        // Convert power to energy
        var energy = sm.Power * sm.ReactionPowerModifier;

        // Release the waste. Both are scaled by modifier and energy, but o2 also scales with temperatures.
        absorbed.Gas.AdjustMoles(Gas.Oxygen, Math.Max(moleModifier * (energy + absorbed.Gas.Temperature - Atmospherics.T0C) * sm.OxygenReleaseEfficiencyModifier, 0f));
        absorbed.Gas.AdjustMoles(Gas.Plasma, Math.Max(moleModifier * sm.PlasmaReleaseModifier * energy, 0f));

        // Increase temperature
        absorbed.Gas.Temperature += energy * sm.ThermalReleaseModifier;

        #endregion

        #region Scale down power

        // I'd recommend plotting these two if you want to get it
        // but in general this lets it need less input to stay under 10 power than above
        // Below 10 power it substracts very little, and above it substracts 1/10
        // 10f (and 0.9f) hardcoded to discourage yaml majors messing with it since it impacts a lot
        // (And would require massive structural changes, all to minuscule benefit)
        var powerReduction = (float)Math.Pow(sm.Power / 5f, 3f);

        // After this point power is lowered
        // This wraps around to the begining of the function
        sm.Power = Math.Max(sm.Power - Math.Min(powerReduction, sm.Power * 0.8f) * powerlossInhibitor, 0f);

        #endregion
    }

    /// <summary>
    ///     Shoot lightning bolts depensing on accumulated power.
    /// </summary>
    private void SupermatterZap(EntityUid uid, SupermatterComponent sm)
    {

        #region Calc modifiers for zap

        // This isn't DRY but erm whatever. Alternatively I can surface this. And add a few params or some weird struct.
        // (Also I can't cleanly run it on top level anyways since damage is independent

        var mix = Atmosphere.GetContainingMixture(uid, true, true);

        if (mix is not { })
            return;

        var gas = mix.Clone();
        var moles = gas.TotalMoles;

        if (!(moles > 0f))
            return;

        var (_, zapModifier, _, _, _) = gas.GetGasModifiers();

        #endregion

        // Divide power by it's threshold to get a value from 0 to 1, then multiply by the amount of possible lightnings
        // Makes it pretty obvious that if SM is shooting out red lightnings something is wrong.
        // And if it shoots too weak lightnings it means that it's underfed. Feed the SM :godo:
        var zapPower = sm.Power * zapModifier / sm.PowerPenaltyThreshold * sm.LightningPrototypes.Length;
        var zapPowerNorm = (int)Math.Clamp(zapPower, 0, sm.LightningPrototypes.Length - 1);
        _lightning.ShootRandomLightnings(uid, 3.5f, sm.Power > sm.PowerPenaltyThreshold ? 3 : 1, sm.LightningPrototypes[zapPowerNorm]);
    }

    /// <summary>
    ///     Handles environmental damage.
    /// </summary>
    private void HandleDamage(EntityUid uid, SupermatterComponent sm)
    {
        var damageArchived = sm.Damage;

        #region Get gas info

        var mix = Atmosphere.GetContainingMixture(uid, true, true);

        // We're in space or there is no gas to process
        if (mix is not { } || mix.TotalMoles == 0f)
        {
            sm.Damage += Math.Max(sm.Power / 1000 * sm.DamageIncreaseMultiplier, 0.1f);
            return;
        }

        // Absorbed gas from surrounding area
        using var surrounding = new GasWrapper(mix, sm.GasEfficiency);
        var moles = surrounding.Gas.TotalMoles;
        var (_, _, _, _, heatResistModifier) = surrounding.Gas.GetGasModifiers();

        #endregion

        var totalDamage = 0f;

        var tempThreshold = (Atmospherics.T0C + sm.HeatPenaltyThreshold) * heatResistModifier;

        // Temperature start to have a positive effect on damage after 350
        var tempDamage = Math.Max(Math.Clamp(moles / 200f, .5f, 1f) * surrounding.Gas.Temperature - tempThreshold, 0f) * sm.MoleHeatThreshold / 150f * sm.DamageIncreaseMultiplier;
        totalDamage += tempDamage;

        // Power only starts affecting damage when it is above 5000
        var powerDamage = Math.Max(sm.Power - sm.PowerPenaltyThreshold, 0f) / 500f * sm.DamageIncreaseMultiplier;
        totalDamage += powerDamage;

        // Molar count only starts affecting damage when it is above 1800
        var moleDamage = Math.Max(moles - sm.MolePenaltyThreshold, 0) / 80 * sm.DamageIncreaseMultiplier;
        totalDamage += moleDamage;

        // Healing damage
        if (moles < sm.MolePenaltyThreshold)
        {
            // left there a very small float value so that it doesn't eventually divide by 0.
            var healHeatDamage = Math.Min(surrounding.Gas.Temperature - tempThreshold, 0.001f) / 150;
            totalDamage += healHeatDamage;
        }

        sm.Damage = Math.Min(damageArchived + sm.DamageHardcap * sm.DelaminationPoint, totalDamage);
        sm.DamageDelta = sm.Damage - damageArchived;
    }

    /// <summary>
    ///     Handles announcements.
    /// </summary>
    private void HandleAnnouncements(EntityUid uid, SupermatterComponent sm)
    {
        var message = string.Empty;
        var global = false;

        var integrity = GetIntegrity(sm).ToString("0.00");

        // Delam is happening
        if (sm.Delamming && !sm.DelamAnnounced)
        {
            var sb = new StringBuilder();
            var alertLevel = "yellow";

            string? loc;
            switch (_delamType)
            {
                case DelamType.Explosion:
                default:
                    loc = "supermatter-delam-explosion";
                    break;

                case DelamType.Singulo:
                    loc = "supermatter-delam-overmass";
                    alertLevel = "delta";
                    break;

                case DelamType.Tesla:
                    loc = "supermatter-delam-tesla";
                    alertLevel = "delta";
                    break;

                case DelamType.Cascade:
                    loc = "supermatter-delam-cascade";
                    alertLevel = "delta";
                    break;
            }

            var station = _station.GetOwningStation(uid);
            if (station != null)
                _alert.SetLevel((EntityUid)station, alertLevel, true, true, true, false);

            sb.AppendLine(Loc.GetString(loc));
            sb.AppendLine(Loc.GetString("supermatter-seconds-before-delam", ("seconds", sm.DelamTimer)));

            message = sb.ToString();
            global = true;
            sm.DelamAnnounced = true;

            SupermatterAnnouncement(uid, message, global);
            return;
        }

        // Delam stopped, let everyone know.
        if (sm.Damage < sm.DelaminationPoint && sm.Delamming)
        {
            message = Loc.GetString("supermatter-delam-cancel", ("integrity", integrity));
            sm.DelamAnnounced = false;
            global = true;
            SupermatterAnnouncement(uid, message, global);
            return;
        }

        // We are not taking consistent damage. Engis/warn not needed.
        if (sm.DamageDelta >= 0)
            return;

        // Check if we need to warn anyone
        switch (sm.Damage)
        {
            case >= SupermatterComponent.EmergencyPoint:
                message = Loc.GetString("supermatter-emergency", ("integrity", integrity));
                global = true;
                break;
            case >= SupermatterComponent.WarningPoint:
                message = Loc.GetString("supermatter-warning", ("integrity", integrity));
                break;
        }

        SupermatterAnnouncement(uid, message, global);
    }

    #endregion

    #region Helper functions

    /// <summary>
    ///     Help the SM announce something.
    /// </summary>
    /// <param name="global">If true, does the station announcement.</param>
    /// <param name="customSender">If true, sends the announcement from Central Command.</param>
    public void SupermatterAnnouncement(EntityUid uid, string message, bool global = false, string? customSender = null)
    {
        if (global)
        {
            var sender = customSender ?? Loc.GetString("supermatter-announcer");
            _chat.DispatchStationAnnouncement(uid, message, sender, colorOverride: Color.Yellow);
            return;
        }
        _chat.TrySendInGameICMessage(uid, message, InGameICChatType.Speak, hideChat: false, checkRadioPrefix: true);
    }

    /// <summary>
    ///     Returns the integrity rounded to hundreds, e.g. 100.00%
    /// </summary>
    public static float GetIntegrity(SupermatterComponent sm)
    {
        var integrity = sm.Damage / sm.DelaminationPoint;
        integrity = (float)Math.Round(100 - integrity * 100, 2);
        integrity = integrity < 0 ? 0 : integrity;
        return integrity;
    }

    /// <summary>
    ///     Decide on how to delaminate.
    /// </summary>
    public DelamType ChooseDelamType(EntityUid uid, SupermatterComponent sm)
    {
        var mix = Atmosphere.GetContainingMixture(uid, true, true);

        if (mix is { })
        {
            var moles = mix.TotalMoles;

            if (moles >= sm.MolePenaltyThreshold)
                return DelamType.Singulo;
        }

        if (sm.Power >= sm.PowerPenaltyThreshold)
            return DelamType.Tesla;

        return DelamType.Explosion;
    }

    /// <summary>
    ///     Handle the end of the station.
    /// </summary>
    private void HandleDelamination(EntityUid uid, SupermatterComponent sm)
    {
        var xform = Transform(uid);

        _delamType = ChooseDelamType(uid, sm);

        if (!sm.Delamming)
        {
            sm.Delamming = true;
            HandleAnnouncements(uid, sm);
        }
        if (sm.Damage < sm.DelaminationPoint && sm.Delamming)
        {
            sm.Delamming = false;
            HandleAnnouncements(uid, sm);
        }

        sm.DelamTimerAccumulator++;

        if (sm.DelamTimer > sm.DelamTimerAccumulator)
            return;

        switch (_delamType)
        {
            case DelamType.Explosion:
            default:
                _explosion.TriggerExplosive(uid);
                break;

            case DelamType.Singulo:
                Spawn(sm.SingularityPrototypeId, xform.Coordinates);
                break;

            case DelamType.Tesla:
                Spawn(sm.TeslaPrototypeId, xform.Coordinates);
                break;

            case DelamType.Cascade:
                Spawn(sm.SupermatterKudzuPrototypeId, xform.Coordinates);
                break;
        }
    }

    private void HandleSoundLoop(SupermatterComponent sm)
    {
        var isAggressive = sm.Damage > SupermatterComponent.WarningPoint;
        var isDelamming = sm.Damage > sm.DelaminationPoint;

        if (!isAggressive && !isDelamming)
        {
            sm.AudioStream = _audio.Stop(sm.AudioStream);
            return;
        }

        var smSound = isDelamming ? SuperMatterSound.Delam : SuperMatterSound.Aggressive;

        if (sm.SmSound == smSound)
            return;

        sm.AudioStream = _audio.Stop(sm.AudioStream);
        sm.SmSound = smSound;
    }

    #endregion

    #region Event Handlers

    private void OnCollideEvent(EntityUid uid, SupermatterComponent sm, ref StartCollideEvent args)
    {
        var target = args.OtherEntity;

        // Stop immune entities from activating the sm.
        if (args.OtherBody.BodyType == BodyType.Static
            || HasComp<SupermatterImmuneComponent>(target)
            || MetaData(target).EntityPrototype?.ID == sm.AshPrototypeId
            || _container.IsEntityInContainer(uid))
            return;

        if (!sm.Activated)
        {
            // Extra logging for supermatter
            var activator = ToPrettyString(args.OtherEntity);

            _sharedChat.SendAdminAlert($"Supermatter activated by {activator} at {Transform(uid).Coordinates}");

            _adminLog.Add(LogType.Action, LogImpact.High,
                $"Supermatter activated by {activator} at {Transform(uid).Coordinates}");

            sm.Activated = true;
        }

        if (TryComp<SupermatterFoodComponent>(target, out var food))
            sm.Power += food.Energy;
        else if (TryComp<ProjectileComponent>(target, out var projectile))
            sm.Power += (float)projectile.Damage.GetTotal();
        else
            sm.Power++;

        sm.MatterPower += HasComp<MobStateComponent>(target) ? 10 : 0;

        if (!HasComp<ProjectileComponent>(target))
        {
            _adminLog.Add(LogType.EntityDelete, LogImpact.Medium, $"Supermatter {ToPrettyString(uid)} has consumed {ToPrettyString(target)}");
            EntityManager.SpawnAttachedTo(sm.AshPrototypeId, Transform(target).Coordinates);
            _audio.PlayPvs(sm.DustSound, uid);
        }

        EntityManager.QueueDeleteEntity(target);
    }

    private void OnHandInteract(EntityUid uid, SupermatterComponent sm, ref InteractHandEvent args)
    {
        var target = args.User;

        if (HasComp<SupermatterImmuneComponent>(target))
            return;

        if (!sm.Activated)
            sm.Activated = true;

        sm.MatterPower += 10;

        EntityManager.SpawnEntity("Ash", Transform(target).Coordinates);
        _audio.PlayPvs(sm.DustSound, uid);
        EntityManager.QueueDeleteEntity(target);
    }

    private void OnItemInteract(EntityUid uid, SupermatterComponent sm, ref InteractUsingEvent args)
    {
        if (!HasComp<SupermatterImmuneComponent>(args.User))
            return;

        if (!sm.Activated)
            sm.Activated = true;

        if (sm.SliverRemoved)
            return;

        if (!HasComp<SharpComponent>(args.Used))
            return;

        var dae = new DoAfterArgs(EntityManager, args.User, 30f, new SupermatterDoAfterEvent(), uid)
        {
            BreakOnDamage = true,
            BreakOnHandChange = false,
            BreakOnMove = true,
            BreakOnWeightlessMove = false,
            NeedHand = true,
            RequireCanInteract = true,
        };

        _doAfter.TryStartDoAfter(dae);
    }

    private void OnGetSliver(EntityUid uid, SupermatterComponent sm, ref SupermatterDoAfterEvent args)
    {
        if (args.Cancelled)
            return;

        // your criminal actions will not go unnoticed
        sm.Damage += sm.DelaminationPoint / 10;
        sm.DamageDelta += sm.DelaminationPoint / 10;

        var integrity = GetIntegrity(sm).ToString("0.00");
        SupermatterAnnouncement(uid, Loc.GetString("supermatter-announcement-cc-tamper", ("integrity", integrity)), true, "Central Command");

        Spawn(sm.SliverPrototypeId, _transform.GetMapCoordinates(args.User));

        if (sm.DelamTimer > 30f)
            sm.DelamTimer -= 10f;
    }

    private void OnExamine(EntityUid uid, SupermatterComponent sm, ref ExaminedEvent args)
    {
        // get all close and personal to it
        if (args.IsInDetailsRange)
        {
            args.PushMarkup(Loc.GetString("supermatter-examine-integrity", ("integrity", GetIntegrity(sm).ToString("0.00"))));
        }
    }

    #endregion

    /// <summary>
    /// <para>Simple <see cref="IDisposable"/> wrapper around <see cref="GasMixture"/></para>
    /// <para>Splits off a part of gas, and merges them together later</para>
    /// <para><see cref="AtmosphereSystem.Merge(GasMixture, GasMixture)"/> is automatically called at the end -
    /// use <see cref="GasMixture"/> instead if you want to handle it manually</para>
    /// </summary>
    private readonly struct GasWrapper(GasMixture surroundingMix, float ratio) : IDisposable
    {
        private readonly GasMixture _surrounding = surroundingMix;

        /// <summary>
        /// The split off part of your gas. 
        /// </summary>
        public readonly GasMixture Gas = surroundingMix.RemoveRatio(ratio);

        public void Dispose()
        {
            Atmosphere.Merge(_surrounding, Gas);
        }
    }
}

public static class SmExtensions
{
    /// <summary>
    /// Get SM related data about a provided gas mix.
    /// </summary>
    /// <param name="absorbedGas">Mix to be parsed</param>
    /// <returns>A selection of values, check <see cref="SupermatterComponent.GasDataFields(Gas)"/></returns>
    public static (float radModifier, float zapModifier, float moleModifier, float heatModifier, float heatResistModifier) GetGasModifiers(this GasMixture absorbedGas)
    {
        var totalMoles = absorbedGas.TotalMoles;

        // Safety check: Prevent a divide-by-zero NaN cascade if the mix is completely empty
        if (totalMoles <= 0f)
        {
            return (1f, 1f, 1f, 1f, 1f);
        }

        var radModifier = 1f;
        var zapModifier = 1f;
        var moleModifier = 1f;
        var heatModifier = 1f;
        var heatResistModifier = 1f;

        // Safely iterate through the actual enum values, regardless of their integer backing
        foreach (Gas gas in Enum.GetValues<Gas>())
        {
            var proportion = absorbedGas.GetGasMolarPercentage(gas);

            // Skip doing math if there's none of this gas in the mix
            if (proportion <= 0f) continue;

            var facts = SupermatterComponent.GasDataFields(gas);

            radModifier += proportion * facts.RadMod;
            zapModifier += proportion * facts.ZapMod;
            moleModifier += proportion * facts.MoleMod;
            heatModifier += proportion * facts.HeatMod;
            heatResistModifier += proportion * facts.HeatResistMod;
        }

        // Ensure we don't do something stupid later
        return (
            Math.Max(radModifier, 0f),
            Math.Max(zapModifier, 0f),
            Math.Max(moleModifier, 0f),
            Math.Max(heatModifier, 0f),
            Math.Max(heatResistModifier, 0f)
        );
    }

    public static float GetGasMolarPercentage(this GasMixture gasMix, Gas gas)
    {
        if (!(gasMix.TotalMoles > 0f))
            return 0f;
        return gasMix.GetMoles(gas) / gasMix.TotalMoles;
    }

    public static float GetGasMolarPercentage(this GasMixture gasMix, int gas)
    {
        if (!(gasMix.TotalMoles > 0f))
            return 0f;
        return gasMix.GetMoles(gas) / gasMix.TotalMoles;
    }
}
