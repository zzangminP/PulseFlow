using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PulseFlow.Models;

public partial class PulseFlowDbContext : DbContext
{
    public PulseFlowDbContext()
    {
    }

    public PulseFlowDbContext(DbContextOptions<PulseFlowDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SensorLog> SensorLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=PulseFlowDB;Username=postgres;Password=0000");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SensorLog>(entity =>
        {
            entity.HasIndex(e => e.MachineName, "IX_SensorLogs_MachineName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
