using System;
using System.Collections.Generic;

namespace ReadFabricSqlEndpoint.Database;

public partial class ExecRequestsHistory
{
    public Guid? DistributedStatementId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? TotalElapsedTimeMs { get; set; }

    public string? LoginName { get; set; }

    public long? RowCount { get; set; }

    public string? Status { get; set; }

    public int? SessionId { get; set; }

    public Guid? ConnectionId { get; set; }

    public string? ProgramName { get; set; }

    public Guid? BatchId { get; set; }

    public Guid? RootBatchId { get; set; }

    public string? QueryHash { get; set; }

    public string? Command { get; set; }
}
