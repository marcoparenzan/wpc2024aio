using System.Text.Json.Serialization;

namespace Wpc2024HMIApp.Models
{
    public partial class BoilerStatus
    {
        [JsonPropertyName("Boiler #1 Status")]
        public Boiler1Status Status { get; set; }
    }

    public partial class Boiler1Status
    {
        [JsonPropertyName("SourceTimestamp")]
        public DateTimeOffset SourceTimestamp { get; set; }

        [JsonPropertyName("Value")]
        public Value Value { get; set; }
    }

    public partial class Value
    {
        [JsonPropertyName("Temperature")]
        public Temperature Temperature { get; set; }

        [JsonPropertyName("Pressure")]
        public long Pressure { get; set; }

        [JsonPropertyName("HeaterState")]
        public string HeaterState { get; set; }
    }

    public partial class Temperature
    {
        [JsonPropertyName("Top")]
        public long Top { get; set; }

        [JsonPropertyName("Bottom")]
        public long Bottom { get; set; }
    }
}