using System;
using System.Collections.Generic;

namespace WarehouseEventHandling.Database;

public partial class History
{
    public string AssetName { get; set; } = null!;

    public string VariableName { get; set; } = null!;

    public string VariableType { get; set; } = null!;

    public DateTime StartTimeStamp { get; set; }

    public DateTime? EndTimeStamp { get; set; }

    public bool? BoolValue { get; set; }

    public DateTime? DateTimeValue { get; set; }

    public long? IntValue { get; set; }

    public decimal? DecimalValue { get; set; }

    public string? JsonValue { get; set; }

    public string? StringValue { get; set; }
}
