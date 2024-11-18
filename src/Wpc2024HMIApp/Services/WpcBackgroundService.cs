
using MQTTnet.Client;
using MQTTnet;
using Wpc2024HMIApp.Options;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Text.Json;
using Wpc2024HMIApp.Models;
using MQTTnet.Server;

namespace Wpc2024HMIApp.Services
{
    public class WpcBackgroundService : BackgroundService
    {
        private readonly ILogger<WpcBackgroundService> _logger;
        private readonly IHmiService hmiService;
        private readonly MqttOptions _mqttOptions;
        private readonly IotHubOptions _iotHubOptions;
        private MqttFactory _mqttFactory = new();
        private IMqttClient _mqttClient;
        private IMqttClient _iotHubClient;

        public WpcBackgroundService(ILogger<WpcBackgroundService> logger,
            IOptions<MqttOptions> mqttOptions,
            IOptions<IotHubOptions> iotHubOptions,
            IHmiService hmiService)
        {
            this._logger = logger;
            _mqttOptions = mqttOptions.Value;
            _mqttClient = _mqttFactory.CreateMqttClient();
            _iotHubOptions = iotHubOptions.Value;
            _iotHubClient = _mqttFactory.CreateMqttClient();
            this.hmiService = hmiService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hosted Service running.");

            #region MQTT Explorer
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId(_mqttOptions.ClientId)
                .WithTcpServer(_mqttOptions.HostName, _mqttOptions.PortNumber) // Port is optional
                //.WithCredentials(_mqttOptions.UserName, _mqttOptions.Password) // No auth for the demo
                .WithCleanSession()
                .Build();
            _mqttClient.ApplicationMessageReceivedAsync += MQTTClient_ApplicationMessageReceivedAsync;
            var result = await _mqttClient.ConnectAsync(mqttClientOptions, cancellationToken);
            _ = await _mqttClient.SubscribeAsync("#", cancellationToken: cancellationToken);
            _logger.LogInformation("MqttExplorer running...");
            #endregion

            #region IoT Hub
            var iotHubClientOptions = new MqttClientOptionsBuilder()
                .WithClientId(_iotHubOptions.ClientId)
                .WithTcpServer(_iotHubOptions.HostName, _iotHubOptions.PortNumber) // Port is optional
                .WithCredentials($"{_iotHubOptions.HostName}/{_iotHubOptions.ClientId}/api-version=2021-04-12", _iotHubOptions.Password)
                .WithTlsOptions(new MqttClientTlsOptions
                {
                    AllowUntrustedCertificates = true,
                    UseTls = true,
                })
                .WithCleanSession()
                .Build();
            await _iotHubClient.ConnectAsync(iotHubClientOptions, CancellationToken.None);
            _logger.LogInformation("IoTHub sender running...");
            #endregion
        }

        private async Task MQTTClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            switch (arg.ApplicationMessage.Topic)
            {
                case "azure-iot-operations/data/wpc2024opcuavm2":
                    var value = JsonSerializer.Deserialize<BoilerStatus>(arg.ApplicationMessage.PayloadSegment);
                    if (value != null)
                    {
                        await hmiService.AddHmiValueAsync("Boiler1", "Temperature", value.Status.Value.Temperature.Top);
                        await hmiService.AddHmiValueAsync("Boiler1", "Pressure", value.Status.Value.Pressure);
                        
                        //await this.SendMessageToIotHub(value.Status.Value.Temperature.Top, value.Status.Value.Pressure);
                    }

                    break;
            }
        }

        private async Task SendMessageToIotHub(long temperature, long pressure)
        {
            var now = DateTimeOffset.Now;

            var message = new Message
            {
                Timestamp = now,
                MessageType = "ua-deltaframe",
                Payload = new Dictionary<string, PayloadItem>
                {
                    ["State"] = new PayloadItem { SourceTimestamp = now, Value = "running" },
                    ["Temperature"] = new PayloadItem { SourceTimestamp = now, Value = temperature },
                    ["Pressure"] = new PayloadItem { SourceTimestamp = now, Value = pressure },
                },
                DataSetWriterName = $"{_iotHubOptions.ClientId}",
                SequenceNumber = Random.Shared.Next(0, 100000)
            };

            var messageJson = JsonSerializer.Serialize(message);

            var devicePublishTopic = $"devices/{_iotHubOptions.ClientId}/messages/events/$.ct=application%2Fjson%3Bcharset%3Dutf-8";

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(devicePublishTopic)
                .WithPayload(JsonSerializer.Serialize(message))
                .Build();

            var result = await _iotHubClient.PublishAsync(applicationMessage, CancellationToken.None);

            Console.WriteLine($"Message was published. {result.IsSuccess} {result.ReasonCode}");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_iotHubClient != null && _iotHubClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
            }

            if (_iotHubClient != null && _iotHubClient.IsConnected)
            {
                await _iotHubClient.DisconnectAsync(cancellationToken: cancellationToken);
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
