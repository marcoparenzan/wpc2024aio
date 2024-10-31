using MQTTnet.Client;
using MQTTnet;
using System.Security.Cryptography.X509Certificates;
using AzureIoTOperations.Models;
using System.Text.Json;

string hostname = "mpiotfabric.northeurope-1.ts.eventgrid.azure.net";
string clientId = "client1-session1";  //client ID can be the session identifier.  A client can have multiple sessions using username and clientId.
string clientName = "client1-authnID";
string x509_pem = @$"D:\IoTHub\AzureIoT2024\certs\{clientName}.pem";  //Provide your client certificate .cer.pem file path
string x509_key = @$"D:\IoTHub\AzureIoT2024\certs\{clientName}.key";  //Provide your client certificate .key.pem file path

var certificate = new X509Certificate2(X509Certificate2.CreateFromPemFile(x509_pem, x509_key).Export(X509ContentType.Pkcs12));

var mqttClient = new MqttFactory().CreateMqttClient();

var connAck = await mqttClient!.ConnectAsync(new MqttClientOptionsBuilder()
    .WithTcpServer(hostname, 8883)
    .WithClientId(clientId)
    .WithCredentials(clientName, "")  //use client authentication name in the username
    .WithTlsOptions(configure => 
        configure
            .UseTls()
            .WithClientCertificates(new X509Certificate2Collection(certificate))
    )
    .Build());

Console.WriteLine($"Client Connected: {mqttClient.IsConnected} with CONNACK: {connAck.ResultCode}");

mqttClient.ApplicationMessageReceivedAsync += async m => await Console.Out.WriteAsync($"Received message on topic: '{m.ApplicationMessage.Topic}' with content: '{m.ApplicationMessage.ConvertPayloadToString()}'\n\n");

var suback = await mqttClient.SubscribeAsync("test1/topic1");
suback.Items.ToList().ForEach(s => Console.WriteLine($"subscribed to '{s.TopicFilter.Topic}' with '{s.ResultCode}'"));

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
            ["AbsorbedEnergy"] = new() { SourceTimestamp = now, Value = Random.Shared.NextDouble() * 100 }
        },
        DataSetWriterName = "solarcamp-north",
        SequenceNumber = Random.Shared.Next(0, 100000)
    };

    var messageJson = JsonSerializer.Serialize(message);

    var applicationMessage = new MqttApplicationMessageBuilder()
    .WithTopic("test1/topic1")
    //.WithTopic("azure-iot-operations/data/opc.tcp/opc.tcp-1/solarcamp-north")
    .WithPayload(JsonSerializer.Serialize(message))
    .Build();

    var puback = await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
    Console.WriteLine(puback.ReasonString);
    await Task.Delay(5000);
}

await mqttClient.DisconnectAsync();
