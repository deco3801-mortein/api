using System.Text.Json.Serialization;
using NodaTime;

namespace Mortein.Types;


public enum CommandType
{
    ToggleVibration,
    VibrateForDuration,
}


/// <summary>
/// Command for a device.
/// </summary>
[JsonDerivedType(typeof(ToggleVibrationCommand))]
[JsonDerivedType(typeof(VibrateForDurationCommand))]
public abstract class Command
{
    /// <summary>
    /// The unique identifier of a device.
    /// </summary>
    public required Guid DeviceId { get; set; }

    /// <summary>
    /// The timestamp for this command.
    /// </summary>
    public Instant Timestamp { get; set; } = SystemClock.Instance.GetCurrentInstant();

    /// <summary>
    /// The type of command.
    /// </summary>
    public abstract CommandType Type { get; }
}


public class ToggleVibrationCommand : Command
{
    public override CommandType Type { get => CommandType.ToggleVibration; }
}


public class VibrateForDurationCommand : Command
{
    public override CommandType Type { get => CommandType.VibrateForDuration; }

    /// <summary>
    /// The duration for which to vibrate the device.
    /// </summary>
    public required int Seconds { get; set; }
}
