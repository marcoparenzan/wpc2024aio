using System;
using System.Collections.Generic;

namespace ReadFabricSqlEndpoint.Database;

public partial class Thermometer02
{
    public string? State { get; set; }

    public bool? AlarmSubsystem1 { get; set; }

    public bool? AlarmSubsystem2 { get; set; }

    public double? AbsorbedEnergy { get; set; }

    public string? AssetName { get; set; }
}
