namespace Wpc2024HMIApp.Models
{
    public class Sample
    {
        public DateTimeOffset Timestamp { get; set; }
        public long Temperature { get; set; }
        public long Pressure { get; set; }
    }
}
