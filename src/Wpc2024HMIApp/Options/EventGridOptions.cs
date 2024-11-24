namespace Wpc2024HMIApp.Options
{
    public sealed class EventGridOptions
    {
        public bool SendToEventGrid { get; set; }
        public string ClientId { get; set; }
        public string HostName { get; set; }
        public int PortNumber { get; set; }
        public string Password { get; set; }
    }
}
