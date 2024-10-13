namespace Mortein.Mqtt.Services;

/// <summary>
/// Provides a managed MQTT client service.
/// </summary>
///
/// <param name="mqttClientService">The provided service.</param>
public class MqttClientServiceProvider(IMqttClientService mqttClientService)
{
    /// <summary>
    /// The provided service.
    /// </summary>
    public IMqttClientService MqttClientService { get; } = mqttClientService;
}
