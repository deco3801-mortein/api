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

    private readonly MqttClientOptions options = options;

    private readonly ILogger<MqttClientService> _logger = logger;

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await mqttClient.ConnectAsync(options, cancellationToken);

        // Discard task, not await it, since it is to run in the background.
        _ = Task.Run(
        async () =>
        {
            // Repeatedly ensure the client is connected.
            while (true)
            {
                try
                {
                    if (!await mqttClient.TryPingAsync())
                    {
                        _logger.LogInformation("MQTT client is disconnected. Reconnecting...");

                        await mqttClient.ConnectAsync(mqttClient.Options, CancellationToken.None);

                        _logger.LogInformation("MQTT client is connected.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "MQTT client connection failed");
                }
                finally
                {
                    // Check the connection state every 5 seconds.
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
        }, cancellationToken);
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            var disconnectOption = new MqttClientDisconnectOptions
            {
                Reason = MqttClientDisconnectOptionsReason.NormalDisconnection,
                ReasonString = "NormalDiconnection"
            };
            await mqttClient.DisconnectAsync(disconnectOption, cancellationToken);
        }
        await mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
    }
}
