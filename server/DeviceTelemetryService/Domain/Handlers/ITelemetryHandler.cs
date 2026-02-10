using DeviceTelemetryService.Domain.Entities;

namespace DeviceTelemetryService.Domain.Handlers;

public interface ITelemetryHandler
{
    Task<TelemetryEvent?> AddEventAsync(string customerId, TelemetryEvent telemetryEvent, CancellationToken cancellationToken);
    Task<List<TelemetryEvent>> GetEventsAsync(string customerId, CancellationToken cancellationToken);
    Task<List<TelemetryEvent>> GetEventsByDevice(string customerId, string deviceId, CancellationToken cancellationToken);
}
