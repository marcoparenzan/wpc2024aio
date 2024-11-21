
using MQTTnet.Client;
using MQTTnet;
using Wpc2024HMIApp.Options;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Text.Json;
using Wpc2024HMIApp.Models;
using MQTTnet.Server;
using Azure.Storage.Files.DataLake;
using System.Diagnostics.Eventing.Reader;
using Parquet.File.Values.Primitives;

namespace Wpc2024HMIApp.Services
{
    public class WpcBackgroundService : BackgroundService
    {
        private TimeSpan BufferInterval = new(0, 1, 0);
        private readonly ILogger<WpcBackgroundService> _logger;
        private readonly IHmiService hmiService;
        private readonly MqttOptions _mqttOptions;
        private readonly IotHubOptions _iotHubOptions;
        private readonly EventGridOptions _eventGridOptions;
        private MqttFactory _mqttFactory = new();
        private IMqttClient _mqttClient;
        private IMqttClient _iotHubClient;
        private IMqttClient _eventGridClient;
        private List<Sample> _samplesBuffer = [];
        private DateTime _lastBufferCleanTime = DateTime.UtcNow;
        private readonly object _lock = new();

        public WpcBackgroundService(ILogger<WpcBackgroundService> logger,
            IOptions<MqttOptions> mqttOptions,
            IOptions<IotHubOptions> iotHubOptions,
            IOptions<EventGridOptions> eventGridOptions,
            IHmiService hmiService)
        {
            this._logger = logger;
            _mqttOptions = mqttOptions.Value;
            _mqttClient = _mqttFactory.CreateMqttClient();
            _iotHubOptions = iotHubOptions.Value;
            _iotHubClient = _mqttFactory.CreateMqttClient();
            _eventGridOptions = eventGridOptions.Value;
            _eventGridClient = _mqttFactory.CreateMqttClient();
            this.hmiService = hmiService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hosted Service running.");

            #region Our MQTT Explorer - from MQBroker AIO
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

            #region EventGrid
            /*
            var eventGridClientOptions = new MqttClientOptionsBuilder()
                .WithClientId(_eventGridOptions.ClientId)
                .WithTcpServer(_eventGridOptions.HostName, _eventGridOptions.PortNumber) // Port is optional
                .WithCredentials($"{_iotHubOptions.HostName}/{_iotHubOptions.ClientId}/api-version=2021-04-12", _iotHubOptions.Password)
                .WithTlsOptions(new MqttClientTlsOptions
                {
                    AllowUntrustedCertificates = true,
                    UseTls = true,
                })
                .WithCleanSession()
                .Build();
            await _iotHubClient.ConnectAsync(eventGridClientOptions, CancellationToken.None);
            _logger.LogInformation("IoTHub sender running...");
            */
            #endregion

            #region Task with a periodic timer
            using PeriodicTimer timer = new(BufferInterval);
            while (!cancellationToken.IsCancellationRequested)
            {
                await SendBufferedDataAsync();
                await timer.WaitForNextTickAsync(cancellationToken);
            }
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
                        var temperature = value.Status.Value.Temperature.Top;
                        var pressure = value.Status.Value.Pressure;

                        await hmiService.AddHmiValueAsync("Boiler1", "Temperature", temperature);
                        await hmiService.AddHmiValueAsync("Boiler1", "Pressure", pressure);
                        
                        await this.SendMessageToIotHubAsync(temperature, pressure);

                        //TODO: Send to Event Grid

                        this.DataBuffering(temperature, pressure);
                    }

                    break;
            }
        }

        private async Task SendMessageToIotHubAsync(long temperature, long pressure)
        {
            var now = DateTimeOffset.Now;

            var message = new Message
            {
                Timestamp = now,
                MessageType = "status",
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
                .WithPayload(messageJson)
                .Build();

            var result = await _iotHubClient.PublishAsync(applicationMessage, CancellationToken.None);

            Console.WriteLine($"Message was published. {result.IsSuccess} {result.ReasonCode}");
        }

        private void DataBuffering(long temperature, long pressure)
        {
            var sample = new Sample { Temperature = temperature, Pressure = pressure };
            lock (_lock)
            {
                _samplesBuffer.Add(sample);
            }
        }

        private async Task SendBufferedDataAsync()
        {
            lock (_lock)
            {
                //TODO: Create CSV from Sample


                //TODO: Send CSV to parquet


                // Reset buffer
                _lastBufferCleanTime = DateTime.UtcNow;
                _samplesBuffer.Clear();
            }
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
