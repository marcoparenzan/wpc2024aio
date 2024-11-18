using System.Text.Json.Serialization;

namespace Common.Models;

public partial class Message
{
    [JsonPropertyName("Timestamp")]
    public DateTimeOffset Timestamp { get; set; }

    [JsonPropertyName("MessageType")]
    public string MessageType { get; set; }

    [JsonPropertyName("payload")]
    public Dictionary<string, PayloadItem> Payload { get; set; }

    [JsonPropertyName("DataSetWriterName")]
    public string DataSetWriterName { get; set; }

    [JsonPropertyName("SequenceNumber")]
    public long SequenceNumber { get; set; }
}

public partial class PayloadItem
{
    [JsonPropertyName("SourceTimestamp")]
    public DateTimeOffset SourceTimestamp { get; set; }

    [JsonPropertyName("Value")]
    public object Value { get; set; }
}