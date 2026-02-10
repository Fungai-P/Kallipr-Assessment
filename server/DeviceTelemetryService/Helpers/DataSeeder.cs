using DeviceTelemetryService.Domain.Entities;
using DeviceTelemetryService.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DeviceTelemetryService.Helpers;

public static class DataSeeder
{
    public static async Task EnsureSeededAsync(AppDbContext db, IOptions<IdempotencyOptions> options)
    {
        var secretKey = options.Value.Secret
                  ?? throw new InvalidOperationException("Idempotency secret not configured");

        await db.Database.MigrateAsync();

        if (await db.Devices.AnyAsync()) return;

        db.Devices.AddRange(
            new Device { CustomerId = "acme-123", DeviceId = "dev-001", Label = "Device 1", Location = "Site A" },
            new Device { CustomerId = "acme-123", DeviceId = "dev-002", Label = "Device 2", Location = "Site A" },
            new Device { CustomerId = "beta-456", DeviceId = "dev-100", Label = "Device 1", Location = "Site B" }
        );

        var event1 = new TelemetryEvent { CustomerId = "acme-123", DeviceId = "dev-001", EventId = "evt-a1", Type = "Temperature", Unit = "Celcius", RecordedAt = DateTime.UtcNow, Value = 21.5 };
        event1.IdempotencyKey = IdempotencyHelper.ComputeKey(event1, secretKey);
        var event2 = new TelemetryEvent { CustomerId = "acme-123", DeviceId = "dev-001", EventId = "evt-a2", Type = "Temperature", Unit = "Celcius", RecordedAt = DateTime.UtcNow, Value = 22.0 };
        event2.IdempotencyKey = IdempotencyHelper.ComputeKey(event1, secretKey);
        // out-of-order
        var event3 = new TelemetryEvent { CustomerId = "acme-123", DeviceId = "dev-001", EventId = "evt-a0", Type = "Temperature", Unit = "Celcius", RecordedAt = DateTime.UtcNow, Value = 21.0 };
        event3.IdempotencyKey = IdempotencyHelper.ComputeKey(event1, secretKey);
        var event4 = new TelemetryEvent { CustomerId = "acme-123", DeviceId = "dev-002", EventId = "evt-b1", Type = "Temperature", Unit = "Celcius", RecordedAt = DateTime.UtcNow, Value = 6.8 };
        event4.IdempotencyKey = IdempotencyHelper.ComputeKey(event1, secretKey);
        var event5 = new TelemetryEvent { CustomerId = "beta-456", DeviceId = "dev-100", EventId = "evt-c1", Type = "Temperature", Unit = "Celcius", RecordedAt = DateTime.UtcNow, Value = 55.2 };
        event5.IdempotencyKey = IdempotencyHelper.ComputeKey(event1, secretKey);

        db.TelemetryEvents.AddRange(
            event1,
            event2,
            event3,
            event4,
            event5
        );

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException) { }
    }
}
