using Content.Goobstation.Shared.Supermatter.Consoles;
using Content.Goobstation.Shared.Supermatter.Monitor;
using Content.Shared.Atmos;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Goobstation.Shared.Supermatter.Consoles;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedSupermatterConsoleSystem))]
public sealed partial class SupermatterConsoleComponent : Component
{
    /// <summary>
    /// The current entity of interest (selected via the console UI)
    /// </summary>
    [ViewVariables]
    public NetEntity? FocusSupermatter;
}

/// <summary>
/// Populate the supermatter console nav map with a single entity
/// </summary>
[Serializable, NetSerializable]
public struct SupermatterNavMapData(NetEntity netEntity, NetCoordinates netCoordinates)
{
    /// <summary>
    /// The entity in question
    /// </summary>
    public NetEntity NetEntity = netEntity;

    /// <summary>
    /// Location of the entity
    /// </summary>
    public NetCoordinates NetCoordinates = netCoordinates;
}

/// <summary>
/// Populates the supermatter console focus entry with supermatter data
/// </summary>
[Serializable, NetSerializable]
public struct SupermatterFocusData(NetEntity netEntity,
    float integrity,
    float power,
    float radiation,
    float absorbedMoles,
    float temperature,
    float temperatureLimit,
    float wasteMultiplier,
    float absorptionRatio,
    Dictionary<Gas, float> gasStorage)
{
    /// <summary>
    /// Focus entity
    /// </summary>
    public NetEntity NetEntity = netEntity;

    /// <summary>
    /// The supermatter's integrity, from 0 to 100
    /// </summary>
    public float Integrity = integrity;

    /// <summary>
    /// The supermatter's power
    /// </summary>
    public float Power = power;

    /// <summary>
    /// The supermatter's emitted radiation
    /// </summary>
    public float Radiation = radiation;

    /// <summary>
    /// The supermatter's total absorbed moles
    /// </summary>
    public float AbsorbedMoles = absorbedMoles;

    /// <summary>
    /// The supermatter's temperature
    /// </summary>
    public float Temperature = temperature;

    /// <summary>
    /// The supermatter's temperature limit
    /// </summary>
    public float TemperatureLimit = temperatureLimit;

    /// <summary>
    /// The supermatter's waste multiplier
    /// </summary>
    public float WasteMultiplier = wasteMultiplier;

    /// <summary>
    /// The supermatter's absorption ratio
    /// </summary>
    public float AbsorptionRatio = absorptionRatio;

    /// <summary>
    /// The supermatter's gas storage
    /// </summary>
    [DataField]
    public Dictionary<Gas, float> GasStorage = gasStorage;
}

/// <summary>
/// Sends data from the server to the client to populate the atmos monitoring console UI
/// </summary>
[Serializable, NetSerializable]
public sealed class SupermatterConsoleBoundInterfaceState(SupermatterConsoleEntry[] supermatters, SupermatterFocusData? focusData) : BoundUserInterfaceState
{
    /// <summary>
    /// A list of all supermatters
    /// </summary>
    public SupermatterConsoleEntry[] Supermatters = supermatters;

    /// <summary>
    /// Data for the UI focus (if applicable)
    /// </summary>
    public SupermatterFocusData? FocusData = focusData;
}

/// <summary>
/// Used to populate the supermatter console UI with data from a single supermatter
/// </summary>
[Serializable, NetSerializable]
public struct SupermatterConsoleEntry(NetEntity entity,
    string entityName,
    SupermatterStatusType status)
{
    /// <summary>
    /// The entity in question
    /// </summary>
    public NetEntity NetEntity = entity;

    /// <summary>
    /// Name of the entity
    /// </summary>
    public string EntityName = entityName;

    /// <summary>
    /// Current alert level
    /// </summary>
    public SupermatterStatusType EntityStatus = status;
}

/// <summary>
/// Used to inform the server that the specified focus for the atmos monitoring console has been changed by the client
/// </summary>
[Serializable, NetSerializable]
public sealed class SupermatterConsoleFocusChangeMessage(NetEntity? focusSupermatter) : BoundUserInterfaceMessage
{
    public NetEntity? FocusSupermatter = focusSupermatter;
}

[NetSerializable, Serializable]
public enum SupermatterConsoleVisuals
{
    ComputerLayerScreen,
}

/// <summary>
/// UI key associated with the supermatter monitoring console
/// </summary>
[Serializable, NetSerializable]
public enum SupermatterConsoleUiKey
{
    Key
}
