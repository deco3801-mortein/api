using Microsoft.AspNetCore.Mvc;
using Mortein.Mqtt.Services;
using Mortein.Types;
using MQTTnet.Client;
using NodaTime.Serialization.SystemTextJson;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mortein.Controllers;


/// <summary>
/// API controller for device commands.
/// </summary>
/// 
/// <param name="context">The context which enables interaction with the database.</param>
/// <param name="clientService">The service exposing the client which enables publishing to the MQTT client.</param>
[ApiController]
[Route("Device/{deviceId}/[controller]")]
public class CommandController(DatabaseContext context, MqttClientService clientService) : ControllerBase
{
    /// The context which enables interaction with the database.
    private readonly DatabaseContext _context = context;

    /// The client which enables publishing to the MQTT client.
    private readonly IMqttClient _client = clientService.MqttClient;

    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        Converters =
        {
            NodaConverters.InstantConverter,
            new JsonStringEnumConverter()
        }
    };

    /// <summary>
    /// Publish a command to a device.
    /// </summary>
    /// 
    /// <param name="deviceId">The device to which to publish a command.</param>
    /// <param name="command">The command to publish.</param>
    private async void PublishCommand(Guid deviceId, Command command)
    {
        await _client.ConnectAsync(_client.Options);
        await _client.PublishStringAsync(ConstructTopicName(deviceId), JsonSerializer.Serialize(command, jsonSerializerOptions));
        await _client.DisconnectAsync();
    }

    /// <summary>
    /// Construct a topic name for a device
    /// </summary>
    /// <param name="deviceId">The device for which to construct a topic name.</param>
    /// <returns></returns>
    private static string ConstructTopicName(Guid deviceId)
    {
        return deviceId.ToString();
    }

    /// <summary>
    /// Toggle Device Vibration
    /// </summary>
    ///
    /// <remarks>
    /// Toggles the vibration of a device by ID.
    /// </remarks>
    ///
    /// <param name="deviceId">The device to vibrate.</param>
    [HttpPost("Toggle")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult ToggleDeviceVibration(Guid deviceId)
    {
        PublishCommand(deviceId, new ToggleVibrationCommand()
        {
            DeviceId = deviceId,
        });
        return NoContent();
    }

    /// <summary>
    /// Vibrate Device for Duration
    /// </summary>
    ///
    /// <remarks>
    /// Vibrate a device by ID for a specified duration.
    /// </remarks>
    ///
    /// <param name="deviceId">The device to vibrate.</param>
    /// <param name="seconds">The duration of the vibration.</param>
    [HttpPost("VibrateForDuration")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult VibrateDeviceForDuration(Guid deviceId, int seconds)
    {
        PublishCommand(deviceId, new VibrateForDurationCommand()
        {
            DeviceId = deviceId,
            Seconds = seconds,
        });
        return NoContent();
    }
}
