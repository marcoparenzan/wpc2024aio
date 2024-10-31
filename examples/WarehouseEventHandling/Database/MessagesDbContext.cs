using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WarehouseEventHandling.Database;

public partial class MessagesDbContext : DbContext
{
    public MessagesDbContext(DbContextOptions<MessagesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<History> Histories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_100_BIN2_UTF8");

        modelBuilder.Entity<History>(entity =>
        {
            entity.HasKey(e => new { e.AssetName, e.VariableName, e.StartTimeStamp })
                .HasName("PK_events_History")
                .IsClustered(false);

            entity.ToTable("History", "events");

            entity.Property(e => e.AssetName)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.VariableName)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.StartTimeStamp).HasPrecision(2);
            entity.Property(e => e.DateTimeValue).HasPrecision(6);
            entity.Property(e => e.DecimalValue).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.EndTimeStamp).HasPrecision(2);
            entity.Property(e => e.JsonValue)
                .HasMaxLength(8000)
                .IsUnicode(false);
            entity.Property(e => e.StringValue)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.VariableType)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
