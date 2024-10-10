using Mortein.Mqtt.Services;
using MQTTnet.Client;
using System.Security.Cryptography.X509Certificates;

namespace Mortein.Mqtt.Extensions;

/// <summary>
/// Extension methods for setting up MQTT client related services in an
/// <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtension
{
    // TODO: read from somewhere other than the local filesystem.
    private static readonly X509Certificate2 certificate = new("/workspaces/api/api.pfx");

    /// <summary>
    /// Registers a hosted MQTT client as a service in the <see cref="IServiceCollection" />.
    /// </summary>
    ///
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    ///
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMqttClientHostedService(this IServiceCollection services)
    {
        services.AddMqttClientServiceWithConfig(optionsBuilder =>
        {
            optionsBuilder
                .WithClientId(Environment.GetEnvironmentVariable("MQTT_CLIENT_ID"))
                .WithoutPacketFragmentation()
                .WithTcpServer(Environment.GetEnvironmentVariable("MQTT_BROKER_HOSTNAME"))
                .WithTlsOptions(options =>
                {
                    options
                        .UseTls()
                        .WithAllowUntrustedCertificates(false)
                        .WithClientCertificates([certificate])
                        .WithIgnoreCertificateChainErrors(false)
                        .WithIgnoreCertificateRevocationErrors(false);
                });
        });
        return services;
    }

    /// <summary>
    /// Registers a hosted MQTT client as a service in the <see cref="IServiceCollection" />.
    /// </summary>
    ///
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configure">
    /// A required action to configure the <see cref="MqttClientOptionsBuilder" /> for the client.
    /// </param>
    ///
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    private static IServiceCollection AddMqttClientServiceWithConfig(this IServiceCollection services, Action<MqttClientOptionsBuilder> configure)
    {
        services.AddSingleton(_ =>
        {
            var optionBuilder = new MqttClientOptionsBuilder();
            configure(optionBuilder);
            return optionBuilder.Build();
        });
        services.AddSingleton<MqttClientService>();
        services.AddSingleton<IHostedService>(serviceProvider =>
        {
            return serviceProvider.GetService<MqttClientService>()!;
        });
        services.AddSingleton(serviceProvider =>
        {
            var mqttClientService = serviceProvider.GetService<MqttClientService>();
            var mqttClientServiceProvider = new MqttClientServiceProvider(mqttClientService!);
            return mqttClientServiceProvider;
        });
        return services;
    }
}
