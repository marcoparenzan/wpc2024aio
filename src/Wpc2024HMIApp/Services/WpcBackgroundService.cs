
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
using Wpc2024HMIApp.Utilities;
using System.Collections.Generic;

namespace Wpc2024HMIApp.Services
{
    public class WpcBackgroundService : BackgroundService
    {
        private readonly TimeSpan BufferInterval = new(0, 0, 10);
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
        private string _fileName = GetFileName();

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
                await timer.WaitForNextTickAsync(cancellationToken);
                await SendBufferedDataAsync();
            }
            #endregion
        }

        private static string GetFileName()
        {
            return $"wpc2024_{DateTime.UtcNow:yyyyMMddHHmmss}";
        }

        private async Task MQTTClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            switch (arg.ApplicationMessage.Topic)
            {
                case "azure-iot-operations/data/wpc2024opcuavm":
                    var value = JsonSerializer.Deserialize<BoilerStatus>(arg.ApplicationMessage.PayloadSegment);
                    if (value?.Status != null)
                    {
                        var sourceTimestamp = value.Status.SourceTimestamp;
                        var temperature = value.Status.Value.Temperature.Top;
                        var pressure = value.Status.Value.Pressure;

                        await hmiService.AddHmiValueAsync("Boiler1", "Temperature", temperature);
                        await hmiService.AddHmiValueAsync("Boiler1", "Pressure", pressure);

                        if (_iotHubOptions.SendToIotHub)
                        {
                            await this.SendMessageToIotHubAsync(sourceTimestamp, temperature, pressure);
                        }

                        //TODO: Send to Event Grid

                        this.DataBuffering(sourceTimestamp, temperature, pressure);
                    }

                    break;
            }
        }

        private async Task SendMessageToIotHubAsync(DateTimeOffset sourceTimestamp, long temperature, long pressure)
        {
            var message = new Message
            {
                Timestamp = sourceTimestamp,
                MessageType = "status",
                Payload = new Dictionary<string, PayloadItem>
                {
                    ["State"] = new PayloadItem { SourceTimestamp = sourceTimestamp, Value = "running" },
                    ["Temperature"] = new PayloadItem { SourceTimestamp = sourceTimestamp, Value = temperature },
                    ["Pressure"] = new PayloadItem { SourceTimestamp = sourceTimestamp, Value = pressure },
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

            Console.WriteLine($"Message was published to IoT Hub. Success: {result.IsSuccess} - ReasonCode: {result.ReasonCode}");
        }

        private void DataBuffering(DateTimeOffset sourceTimeStamp, long temperature, long pressure)
        {
            var sample = new Sample {Timestamp = sourceTimeStamp, Temperature = temperature, Pressure = pressure };
            lock (_lock)
            {
                _samplesBuffer.Add(sample);
            }
        }

        private async Task SendBufferedDataAsync()
        {
            if(_samplesBuffer.Count == 0)
            {
                return;
            }

            List<Sample> samplesBufferCopy = [];
            string csvFile = $"{_fileName}.csv";
            string parquetFile = $"{_fileName}.parquet";

            lock (_lock)
            {
                // Reset buffer
                _lastBufferCleanTime = DateTime.UtcNow;
                samplesBufferCopy = [.. _samplesBuffer];
                _samplesBuffer.Clear();
                _fileName = GetFileName();
            }

            // Create CSV from Samples
            await DataSerializer.WriteCsvAsync(_samplesBuffer, csvFile);

            // Create Parquet from Samples
            await DataSerializer.WriteParquet(_samplesBuffer, parquetFile);

            // Send CSV to Data Lake
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
