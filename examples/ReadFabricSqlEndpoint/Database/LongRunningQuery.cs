using System;
using System.Collections.Generic;

namespace ReadFabricSqlEndpoint.Database;

public partial class LongRunningQuery
{
    public double? MedianTotalElapsedTimeMs { get; set; }

    public int? LastRunTotalElapsedTimeMs { get; set; }

    public DateTime? LastRunStartTime { get; set; }

    public Guid? LastDistStatementId { get; set; }

    public int? LastRunSessionId { get; set; }

    public int? NumberOfRuns { get; set; }

    public string? LastRunCommand { get; set; }
}
