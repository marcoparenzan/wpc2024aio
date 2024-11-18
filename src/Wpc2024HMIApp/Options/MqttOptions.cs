namespace Wpc2024HMIApp.Options
{
    public sealed class MqttOptions
    {
        public string ClientId { get; set; }
        public string HostName { get; set; }
        public int PortNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Topic { get; set; }
    }
}
