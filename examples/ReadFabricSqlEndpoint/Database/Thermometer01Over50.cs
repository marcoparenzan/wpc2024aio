using System;
using System.Collections.Generic;

namespace ReadFabricSqlEndpoint.Database;

public partial class Thermometer01Over50
{
    public string? Timestamp { get; set; }

    public string? AssetName { get; set; }

    public string? State { get; set; }

    public bool? AlarmSubsystem1 { get; set; }

    public bool? AlarmSubsystem2 { get; set; }

    public double? AbsorbedEnergy { get; set; }
}
