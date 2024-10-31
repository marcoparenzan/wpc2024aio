using Microsoft.EntityFrameworkCore;

namespace ReadFabricSqlEndpoint.Database;

public partial class TelemetriesDbContext : DbContext
{
    public virtual DbSet<ExecRequestsHistory> ExecRequestsHistories { get; set; }

    public virtual DbSet<FrequentlyRunQuery> FrequentlyRunQueries { get; set; }

    public virtual DbSet<LongRunningQuery> LongRunningQueries { get; set; }

    public virtual DbSet<Thermometer01> Thermometer01s { get; set; }

    public virtual DbSet<Thermometer01Over50> Thermometer01Over50s { get; set; }

    public virtual DbSet<Thermometer02> Thermometer02s { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_100_BIN2_UTF8");

        modelBuilder.Entity<ExecRequestsHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("exec_requests_history", "queryinsights");

            entity.Property(e => e.BatchId).HasColumnName("batch_id");
            entity.Property(e => e.Command)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("command");
            entity.Property(e => e.ConnectionId).HasColumnName("connection_id");
            entity.Property(e => e.DistributedStatementId).HasColumnName("distributed_statement_id");
            entity.Property(e => e.EndTime)
                .HasPrecision(6)
                .HasColumnName("end_time");
            entity.Property(e => e.LoginName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("login_name");
            entity.Property(e => e.ProgramName)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("program_name");
            entity.Property(e => e.QueryHash)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("query_hash");
            entity.Property(e => e.RootBatchId).HasColumnName("root_batch_id");
            entity.Property(e => e.RowCount).HasColumnName("row_count");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.StartTime)
                .HasPrecision(6)
                .HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TotalElapsedTimeMs).HasColumnName("total_elapsed_time_ms");
        });

        modelBuilder.Entity<FrequentlyRunQuery>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("frequently_run_queries", "queryinsights");

            entity.Property(e => e.AvgTotalElapsedTimeMs).HasColumnName("avg_total_elapsed_time_ms");
            entity.Property(e => e.LastDistStatementId).HasColumnName("last_dist_statement_id");
            entity.Property(e => e.LastRunCommand)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("last_run_command");
            entity.Property(e => e.LastRunStartTime)
                .HasPrecision(6)
                .HasColumnName("last_run_start_time");
            entity.Property(e => e.LastRunTotalElapsedTimeMs).HasColumnName("last_run_total_elapsed_time_ms");
            entity.Property(e => e.MaxRunTotalElapsedTimeMs).HasColumnName("max_run_total_elapsed_time_ms");
            entity.Property(e => e.MinRunTotalElapsedTimeMs).HasColumnName("min_run_total_elapsed_time_ms");
            entity.Property(e => e.NumberOfCanceledRuns).HasColumnName("number_of_canceled_runs");
            entity.Property(e => e.NumberOfFailedRuns).HasColumnName("number_of_failed_runs");
            entity.Property(e => e.NumberOfRuns).HasColumnName("number_of_runs");
            entity.Property(e => e.NumberOfSuccessfulRuns).HasColumnName("number_of_successful_runs");
        });

        modelBuilder.Entity<LongRunningQuery>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("long_running_queries", "queryinsights");

            entity.Property(e => e.LastDistStatementId).HasColumnName("last_dist_statement_id");
            entity.Property(e => e.LastRunCommand)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("last_run_command");
            entity.Property(e => e.LastRunSessionId).HasColumnName("last_run_session_id");
            entity.Property(e => e.LastRunStartTime)
                .HasPrecision(6)
                .HasColumnName("last_run_start_time");
            entity.Property(e => e.LastRunTotalElapsedTimeMs).HasColumnName("last_run_total_elapsed_time_ms");
            entity.Property(e => e.MedianTotalElapsedTimeMs).HasColumnName("median_total_elapsed_time_ms");
            entity.Property(e => e.NumberOfRuns).HasColumnName("number_of_runs");
        });

        modelBuilder.Entity<Thermometer01>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Thermometer01");

            entity.Property(e => e.AssetName)
                .HasMaxLength(8000)
                .IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(8000)
                .IsUnicode(false);
            entity.Property(e => e.Timestamp)
                .HasMaxLength(8000)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Thermometer01Over50>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Thermometer01Over50");

            entity.Property(e => e.AssetName)
                .HasMaxLength(8000)
                .IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(8000)
                .IsUnicode(false);
            entity.Property(e => e.Timestamp)
                .HasMaxLength(8000)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Thermometer02>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Thermometer02");

            entity.Property(e => e.AssetName)
                .HasMaxLength(8000)
                .IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(8000)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
