using Microsoft.Azure.Devices.Client;

var hostName = "";
var auth = new DeviceAuthenticationWithRegistrySymmetricKey("device01", "");
var transportType = TransportType.Mqtt;
var options = new ClientOptions {
    ModelId = "xxx"
};

var deviceClient = DeviceClient.Create(hostName, auth, transportType, options);

var message = new Message();

await deviceClient.SendEventAsync(message);

Console.ReadLine();