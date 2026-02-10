using DeviceTelemetryService.Domain.Entities;
using DeviceTelemetryService.Repositories;

namespace DeviceTelemetryService.Domain.Handlers;

public class TelemetryHandler(ITelemetryRepository _telemetryRepository, IDeviceRepository _deviceRepository) : ITelemetryHandler
{
    public async Task<TelemetryEvent?> AddEventAsync(string customerId, TelemetryEvent telemetryEvent, CancellationToken cancellationToken)
    {
        // Does device exist?
        var device = await _deviceRepository.GetSingleAsync(d => d.CustomerId == customerId && d.DeviceId == telemetryEvent.DeviceId, cancellationToken);
        if (device == null) return null;

        await _telemetryRepository.AddEvent(customerId, telemetryEvent, cancellationToken);

        return telemetryEvent;
    }

    public async Task<List<TelemetryEvent>> GetEventsAsync(string customerId, CancellationToken cancellationToken)
    {
        return await _telemetryRepository.GetEventsAsync(customerId, cancellationToken);
    }

    public async Task<List<TelemetryEvent>> GetEventsByDevice(string customerId, string deviceId, CancellationToken cancellationToken)
    {
        return await _telemetryRepository.GetEventsAsync(e => e.CustomerId == customerId && e.DeviceId == deviceId, cancellationToken);
    }
}
