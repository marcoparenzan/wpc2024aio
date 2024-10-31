using AzureIoTOperations.Models;
using MQTTnet;
using MQTTnet.Client;
using System.Text.Json;

var mqttFactory = new MqttFactory();

using var mqttClient = mqttFactory.CreateMqttClient();
var mqttClientOptions = new MqttClientOptionsBuilder()
    .WithTcpServer("4.207.152.105")
    .Build();

var connAck = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
Console.WriteLine($"Client Connected: {mqttClient.IsConnected} with CONNACK: {connAck.ResultCode}");

while (true)
{
    var now = DateTimeOffset.Now;

    var message = new Message
    {
        Timestamp = now,
        MessageType = "ua-deltaframe",
        Payload = new Dictionary<string, PayloadItem>
        {
            ["State"] = new() { SourceTimestamp = now, Value = "running" },
            ["AlarmSubsystem1"] = new() { SourceTimestamp = now, Value = true },
            ["AlarmSubsystem2"] = new() { SourceTimestamp = now, Value = false },
            ["AbsorbedEnergy"] = new() { SourceTimestamp = now, Value = Random.Shared.NextDouble()*100 }
        },
        DataSetWriterName = "solarcamp-north",
        SequenceNumber = Random.Shared.Next(0, 100000)
    };

    var messageJson = JsonSerializer.Serialize(message);

    var applicationMessage = new MqttApplicationMessageBuilder()
        .WithTopic("azure-iot-operations/data/opc.tcp/opc.tcp-1/solarcamp-north")
        .WithPayload(JsonSerializer.Serialize(message))
        .Build();

    var puback = await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
    Console.WriteLine(puback.ReasonString);
    await Task.Delay(1000);
}

await mqttClient.DisconnectAsync();
