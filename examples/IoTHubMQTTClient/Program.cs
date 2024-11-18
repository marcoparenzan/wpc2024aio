using AzureIoTOperations.Models;
using MQTTnet;
using MQTTnet.Client;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

var EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

var hostName = "mpiotfabric.azure-devices.net";
var portNumber = 8883;
var deviceId = "thermometer01";
//var symmetricKey = "IygqJGq9H9Wx0M07oKFgRMCrBe/9J8DN4AIoTNENmvk=";
// https://learn.microsoft.com/en-us/cli/azure/iot/hub?view=azure-cli-latest#az-iot-hub-generate-sas-token-examples
// az iot hub generate-sas-token -d thermometer01 -n mpiotfabric --duration 1000000
var password = "SharedAccessSignature sr=mpiotfabric.azure-devices.net%2Fdevices%2Fthermometer01&sig=yXUJi19pz5xgf%2F0RBBC3ZA3Aw7tbq7z6vIFMYXB8%2FnY%3D&se=1709467384";

const int QoS_AT_MOST_ONCE = 1;

var clientId = deviceId;
var resourceId = $"{hostName}/devices/{deviceId}";
var username = $"{hostName}/{deviceId}/api-version=2021-04-12";
// https://learn.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-messages-d2c#azure-storage-as-a-routing-endpoint
// https://learn.microsoft.com/en-us/azure/iot/iot-mqtt-connect-to-iot-hub#receiving-cloud-to-device-messages
// https://learn.microsoft.com/en-us/azure/iot/iot-mqtt-connect-to-iot-hub#sending-device-to-cloud-messages
var devicePublishTopic = $"devices/{deviceId}/messages/events/$.ct=application%2Fjson%3Bcharset%3Dutf-8";
var deviceSubscribeTopic = $"devices/{deviceId}/messages/devicebound";
//var password = CreateToken(resourceId, symmetricKey);

var mqttFactory = new MqttFactory();

// https://sandervandevelde.wordpress.com/2022/08/12/exploring-full-azure-iot-hub-device-support-using-mqtts-only/

var mqttClient = mqttFactory.CreateMqttClient();
var mqttClientOptions = new MqttClientOptionsBuilder()
    .WithClientId(clientId)
    .WithTcpServer(hostName, portNumber) // Port is optional
    .WithCredentials(username, password)
    .WithTlsOptions(new MqttClientTlsOptions {
        AllowUntrustedCertificates = true,
        UseTls = true,
    })
    .WithCleanSession()
    .Build();
await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

//mqttClient.UseApplicationMessageReceivedHandler(args => {

//    Console.WriteLine($"Message received: {args.ApplicationMessage.Topic}");
//    Console.WriteLine($"Payload: {Encoding.UTF8.GetString(args.ApplicationMessage.Payload)}");

//});
//mqttClient.UseConnectedHandler(async e =>
//{
//    Console.WriteLine("### CONNECTED WITH SERVER ###");

//    // Subscribe to a topic
//    var result = await mqttClient.SubscribeAsync(topicFilter);

//    Console.WriteLine("### SUBSCRIBED ###");
//});


while (true)
{
    var now = DateTimeOffset.Now;

    var message = new Message
    {
        Timestamp = now,
        MessageType = "ua-deltaframe",
        Payload = new Dictionary<string, PayloadItem>
        {
            ["State"] = new PayloadItem { SourceTimestamp = now, Value = "running" },
            ["AlarmSubsystem1"] = new PayloadItem { SourceTimestamp = now, Value = true },
            ["AlarmSubsystem2"] = new PayloadItem { SourceTimestamp = now, Value = false },
            ["AbsorbedEnergy"] = new PayloadItem { SourceTimestamp = now, Value = Random.Shared.NextDouble() * 100 }
        },
        DataSetWriterName = $"{deviceId}",
        SequenceNumber = Random.Shared.Next(0, 100000)
    };

    var messageJson = JsonSerializer.Serialize(message);

    var applicationMessage = new MqttApplicationMessageBuilder()
        .WithTopic(devicePublishTopic)
        .WithPayload(JsonSerializer.Serialize(message))
        .Build();

    await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

    Console.WriteLine("Message was published.");

    await Task.Delay(5000);
}

await mqttClient.DisconnectAsync();

string CreateToken(string resourceUri, string symmetricKey, int timeToLive = 86400)
{
    var sinceEpoch = DateTime.UtcNow - EpochTime;
    var expiry = Convert.ToString((int)sinceEpoch.TotalSeconds + timeToLive);
    string resourceUriEncoded = WebUtility.UrlEncode(resourceUri);

    string value;
    using (HMACSHA256 hMACSHA = new HMACSHA256(Convert.FromBase64String(symmetricKey)))
    {
        value = Convert.ToBase64String(hMACSHA.ComputeHash(Encoding.UTF8.GetBytes($"{resourceUriEncoded}\n{expiry}")));
    }

    return $"SharedAccessSignature sr={resourceUriEncoded}&sig={WebUtility.UrlEncode(value)}&se={expiry}";
}