using DeviceTelemetryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeviceTelemetryService.Repositories;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<TelemetryEvent> TelemetryEvents => Set<TelemetryEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>()
            .HasIndex(d => new { d.CustomerId, d.DeviceId })
            .IsUnique();

        modelBuilder.Entity<TelemetryEvent>()
            .HasIndex(e => new { e.CustomerId, e.DeviceId, e.EventId })
            .IsUnique();

        modelBuilder.Entity<TelemetryEvent>()
            .HasIndex(e => new { e.CustomerId, e.DeviceId, e.RecordedAt });
    }
}
