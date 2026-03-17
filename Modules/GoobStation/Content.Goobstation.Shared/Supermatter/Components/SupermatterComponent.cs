// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Shared.Atmos;
using Content.Shared.DoAfter;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Goobstation.Shared.Supermatter.Components;

[RegisterComponent, NetworkedComponent]
public sealed class SupermatterComponent : Component
{
    #region SM Flags

    /// <summary>The SM will only cycle if activated.</summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool Activated { get; set; }

    /// <summary>
    /// Affects delamination timer. If removed - delamination timer is divided by 2.
    /// Also prevents spawning infinite slivers, or instadelamming.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool SliverRemoved { get; set; }

    /// <summary>Are we delamming?</summary>
    [ViewVariables]
    public bool Delamming { get; set; }

    [ViewVariables]
    public bool DelamAnnounced { get; set; }

    #endregion SM Flags

    #region SM Knobs

    #region Health Knobs

    /// <summary>Received environmental damage is scaled by this</summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float DamageIncreaseMultiplier { get; } = 0.25f;

    /// <summary>
    /// Multiplier on damage the core takes from absorbing hot gas
    /// Default is ~3/7
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float HeatDamageMult { get; } = 0.4286f;

    /// <summary>
    /// The point at which we should start sending messages
    /// about the damage to the engi channels.
    /// </summary>
    public const float WarningPoint = 50;

    /// <summary>The point at which we start sending messages to the common channel</summary>
    public const float EmergencyPoint = 500;

    /// <summary>
    /// Damage per cycle can't increase by more health ratio than this.
    /// It's inverse of effectively a minimum number of seconds for SM to delam (+ delam timer).
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float DamageHardcapPercentage { get; } = 0.002f;

    #endregion Health Knobs

    #region Generic Knobs

    /// <summary>The portion of the gasmix we're on. Relevant for gas processing but not for plain checks.</summary>
    [DataField]
    public float GasEfficiency { get; set; } = 0.15f;

    /// <summary>Ratio of matter power to power conversion rate</summary>
    [DataField]
    public float MatterPowerConversion { get; } = 1f;

    /// <summary>How much matter power is consumed and converted to power per cycle.</summary>
    [DataField]
    public float MatterPowerConsumedPerCycle { get; } = 1f;


    /// <summary>
    /// <para>How much we lose in a single cycle; <c>(1-0.023)^30~=0.5 => </c> 30 second half life (the exponent).</para>
    /// <para>Emitter fires 3 bolts per (on avg) 14 seconds <c>(2+2+2+avg(6,10))</c> so 1 bolt every <c>4.(6)</c> seconds.</para>
    /// <para>2 emitters is the base, and should result in rads ~= t3 singulo. 4 should be >= T4.</para>
    /// <para>Since 1 dmg = 1 pwr, it's <c>14/4.(6)=3</c> pwr per second per emitter</para>
    /// <para>Solving for x in <c>x*0.023=3*e</c> (how much power we need to achieve equlibrium) we get:</para>
    /// <para>1 emitter ~= 130 power, 260 power ~= 8 rads but we apply increased power gain at 200 to get it to ~=200 rads.</para>
    /// <para>To get power after 1 cycle for stuff betwween softcap and hardcap start points, plot <c>y=x(0.977-((x-200)*0.0002))</c>
    /// or check <see cref="PowerToRemove"/></para>
    /// </summary>
    private const float PowerlossPerCycle = 0.023f;

    private const float SoftcapStartPoint = 200f;

    private const float SoftcapScaler = 0.0002f;

    /// <summary>Hardcoded since we want to lower the power to the cap value, not to 0</summary>
    private const float HardcapStartPoint = 2500f;

    private const float HardcapTargetPoint = 1292.5f;

    #endregion Generic Knobs

    #region Atmos Input Knobs

    /// <summary>Amount of energy a mole of ammonia gives.</summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float AmmoniaEnergyPerMole { get; } = 1f;

    /// <summary>
    /// Higher == Higher percentage of inhibitor gas needed
    /// before the charge inertia chain reaction effect starts.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float Co2PercentageForPowerInhibition { get; } = 0.20f;

    /// <summary>
    /// Higher == More moles of the gas are needed before the charge
    /// inertia chain reaction effect starts.
    /// Scales powerloss inhibition down until this amount of moles is reached
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float MoleCountForPowerInhibition { get; } = 20f;

    /// <summary>bonus powerloss inhibition boost if this amount of moles is reached</summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float MoleCountForPowerInhibitionBoost { get; } = 500f;

    /// <summary>
    /// Above this value we can get lord singulo and independent mol damage,
    /// below it we can heal damage
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float MoleDamageThreshold { get; } = 900f;

    /// <summary>
    /// The cutoff on power properly doing damage, pulling shit around,
    /// and delamming into a tesla. Low chance of pyro anomalies, +2 bolts of electricity
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float PowerDamageThreshold { get; } = 2500f;

    /// <summary>
    /// Maximum safe operational temperature in degrees Celsius. Supermatter begins taking damage above this temperature.
    /// If you go above this you'll have plasma burn in your SM chamber anyways.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float HeatDamageThreshold { get; } = 100f;

    #endregion Atmos Input Knobs

    #region Atmos Output Knobs

    /// <summary>
    /// Acts as a multiplier on the amount that nuclear reactions increase the supermatter core temperature
    /// </summary>
    [DataField]
    public float ThermalReleaseModifier { get; } = 0.2f;

    /// <summary>
    /// Multiplier on how much plasma is released during supermatter reactions
    /// Default is ~1/750
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float PlasmaReleaseModifier { get; } = 0.001333f;

    /// <summary>
    /// Multiplier on how much oxygen is released during supermatter reactions.
    /// Default is ~1/325
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float OxygenReleaseEfficiencyModifier { get; } = 0.0031f;

    #endregion Atmos Output Knobs

    #region Output Knobs

    /// <summary>Multiplier on power generated by nuclear reactions</summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float ReactionPowerModifier { get; } = 0.55f;

    /// <summary>Multiply outgoing rads by this.</summary>
    [DataField]
    public float RadiationOutputFactor { get; set; } = 0.03f;

    #endregion Output Knobs

    #endregion SM Knobs

    #region SM Base

    /// <summary>This is what you're here for</summary>
    [ViewVariables(VVAccess.ReadWrite)]

    public float Power { get; set; }

    /// <summary>The amount of damage we have currently</summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float Damage { get; set; }

    /// <summary>Damage change since last cycle</summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float DamageDelta { get; set; }

    /// <summary>Temporary power gained from mob consumption. Purely to not instaspike power to 2000.</summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float MatterPower { get; set; }

    #endregion SM Base

    #region SM Timer

    /// <summary>we yell every YapPeriod Seconds</summary>
    [DataField]
    public float YapPeriod { get; } = 60f;

    /// <summary>set to YapPeriod at first so it doesnt yell a minute after being hit</summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float YapTimer { get; set; } = 60f;

    /// <summary>Time until delam</summary>
    [DataField]
    public float DelamDuration { get; set; } = 120f;

    /// <summary>Timer for delam</summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float DelamTimer { get; set; }

    [ViewVariables(VVAccess.ReadOnly)]
    public float UpdateAccumulator { get; set; }

    [DataField]
    public float UpdatePeriod { get; } = 1f;

    [ViewVariables(VVAccess.ReadOnly)]
    public float ZapAccumulator { get; set; }

    [DataField]
    public float ZapPeriod { get; } = 10f;
    #endregion SM Timer

    #region SM Delamm

    /// <summary>The point at which we delamm, effectively health</summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public int DelaminationPoint { get; } = 900;

    #endregion SM Delamm

    #region SM Gas Facts

    /// <summary>Stores every gas fact. Preferably use <see cref="GasMixture.GetGasModifiers()"/> extension instead.</summary>
    /// <param name="gas">The gas to retrieve the data for.</param>
    /// <returns>
    ///     Given gasses:
    ///     <list type="bullet">
    ///     <item>
    ///     <c>ExergyMod</c>
    ///     <description>- radiation/tesla zap power modifier.</description>
    ///     </item>
    ///     <item>
    ///     <c>HeatMod</c>
    ///     <description>- modifier to crystal power gain from heat.</description>
    ///     </item>
    ///     <item>
    ///     <c>MoleMod</c>
    ///     <description>- produced gas mole count modifier.</description>
    ///     </item>
    ///     <item>
    ///     <c>HeatResistMod</c>
    ///     <description>- modifies threshold for crystal receiving heat damage.</description>
    ///     </item>
    ///     </list>
    /// </returns>
    /// <remarks>
    /// <para>ExergyMod = Affects radiation and zapping; These are the primary useful outputs.</para>
    /// <para>HeatMod = "Heat Power Gain" on /tg/ wiki. Makes SM get energy from temperature.</para>
    /// <para>MoleMod ~= "Gas Waste Multiplier" on /tg/ wiki. Temperature (not energy!) of produced gas depends on crystal energy instead.</para>
    /// <para>HeatResistMod = "Heat Resistance" on /tg/ wiki. Should never be so high as to shield from a trit fire.</para>
    /// <para>These values are for a 100% mix of such gas. These are additive, not multiplicative (so, 100%+mod)</para>
    /// </remarks>
    public static (float ExergyMod, float HeatMod, float WasteMod, float HeatResistMod) GasDataFields(Gas gas)
    {
        return GasFacts.TryGetValue(gas, out var value) ? value : (0f, 0f, 0f, 0f);
    }

    private static readonly Dictionary<Gas, (float ExergyMod, float HeatMod, float WasteMod, float HeatResistMod)> GasFacts =
        new()
        {
            {Gas.Oxygen, (.8f, 1f, 0f, 0f)},
            {Gas.Nitrogen, (0f, -1f, -2.5f, 0f)},
            {Gas.CarbonDioxide, (0f, 1f, 1f, 0f)},
            {Gas.Plasma, (2f, 4f, 5f, 0f)},
            {Gas.Tritium, (4f, 30f, 10f, .25f)},
            {Gas.WaterVapor, (-.25f, 1f, 11f, .25f)},
            {Gas.Frezon, (-3f, -1f, -9f, -.5f)},
            {Gas.Ammonia, (0f, .3f, 0f, 0f)},
            {Gas.NitrousOxide, (0f, 0f, 0f, 5f)}
        }
        ;

    #endregion SM Gas Facts

    #region SM Prototypes

    [DataField("supermatterSliverPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string SliverPrototypeId { get; } = "SupermatterSliver";

    /// <summary>What will be spawned when an entity that's not supermatter immune interacts with it.</summary>
    [DataField("ashPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string AshPrototypeId { get; } = "Ash";

    /// <summary>There has to be a way to serialize this properly</summary>
    [DataField]
    public IReadOnlyList<string> LightningPrototypes { get; } =
    [
        "Lightning",
        "ChargedLightning",
        "SuperchargedLightning",
        "HyperchargedLightning"
    ];

    [DataField("singularitySpawnPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string SingularityPrototypeId { get; } = "Singularity";

    [DataField("teslaSpawnPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string TeslaPrototypeId { get; } = "TeslaEnergyBall";

    [DataField("supermatterKudzuSpawnPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string SupermatterKudzuPrototypeId { get; } = "SupermatterKudzu";

    #endregion SM Prototypes

    [DataField]
    public SoundSpecifier DustSound { get; } = new SoundPathSpecifier("/Audio/Supermatter/dust.ogg");

    #region Helper functions

    /// <summary>Returns the integrity as percentage rounded to hundreds, e.g. 100.00%</summary>
    public float Integrity
    {
        get
        {
            var integrity = Damage / DelaminationPoint;
            integrity = float.Round(100 - integrity * 100, 2);
            integrity = integrity < 0 ? 0 : integrity;
            return integrity;
        }
    }

    public string IntegrityString => GetFormattedIntegrityString("0.00", formatProvider: null);

    public string GetFormattedIntegrityString(string? format, IFormatProvider? formatProvider)
    {
        return Integrity.ToString(format, formatProvider);
    }

    public float PowerToRemove()
    {
        return Power switch
        {
            > HardcapStartPoint => Power - HardcapTargetPoint,
            > SoftcapStartPoint => Power * (PowerlossPerCycle + SoftcapScaler * (Power - SoftcapStartPoint)),
            _ => Power * PowerlossPerCycle,
        };
    }

    #endregion Helper functions
}

[Serializable, NetSerializable]
public sealed class SupermatterDoAfterEvent : SimpleDoAfterEvent;
