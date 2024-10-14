using MQTTnet;
using MQTTnet.Client;

namespace Mortein.Mqtt.Services;

/// <summary>
/// ASP.NET Service which provides a managed MQTT client.
/// </summary>
///
/// <param name="options">MQTT client configuration options.</param>
/// <param name="logger">Logging facility.</param>
public class MqttClientService(MqttClientOptions options, ILogger<MqttClientService> logger)
    : IMqttClientService
{
    private readonly IMqttClient mqttClient = new MqttFactory().CreateMqttClient();

    /// <summary>
    /// Underlying MQTT Client connection.
    /// </summary>
    public IMqttClient MqttClient => mqttClient;

    private readonly MqttClientOptions options = options;

    private readonly ILogger<MqttClientService> _logger = logger;

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await mqttClient.ConnectAsync(options, cancellationToken);
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            var disconnectOption = new MqttClientDisconnectOptions
            {
                Reason = MqttClientDisconnectOptionsReason.NormalDisconnection,
                ReasonString = "NormalDisconnection"
            };
            await mqttClient.DisconnectAsync(disconnectOption, cancellationToken);
        }
        await mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
    }
}
