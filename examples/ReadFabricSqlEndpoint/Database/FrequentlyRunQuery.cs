using System;
using System.Collections.Generic;

namespace ReadFabricSqlEndpoint.Database;

public partial class FrequentlyRunQuery
{
    public int? NumberOfRuns { get; set; }

    public int? MinRunTotalElapsedTimeMs { get; set; }

    public int? MaxRunTotalElapsedTimeMs { get; set; }

    public int? AvgTotalElapsedTimeMs { get; set; }

    public int? NumberOfSuccessfulRuns { get; set; }

    public int? NumberOfFailedRuns { get; set; }

    public int? NumberOfCanceledRuns { get; set; }

    public int? LastRunTotalElapsedTimeMs { get; set; }

    public DateTime? LastRunStartTime { get; set; }

    public Guid? LastDistStatementId { get; set; }

    public string? LastRunCommand { get; set; }
}
