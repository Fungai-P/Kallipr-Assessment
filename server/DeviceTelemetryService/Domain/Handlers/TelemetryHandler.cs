using DeviceTelemetryService.Domain.Entities;
using DeviceTelemetryService.Repositories;

namespace DeviceTelemetryService.Domain.Handlers;

public class TelemetryHandler(
    ITelemetryRepository _telemetryRepository,
    IDeviceRepository _deviceRepository,
    ILogger<TelemetryHandler> _logger) : ITelemetryHandler
{
    public async Task<TelemetryEvent?> AddEventAsync(string customerId, TelemetryEvent telemetryEvent, CancellationToken cancellationToken)
    {
        // Does device exist?
        var device = await _deviceRepository.GetSingleAsync(d => d.CustomerId == customerId && d.DeviceId == telemetryEvent.DeviceId, cancellationToken);
        if (device == null)
        {
            _logger.LogError($"Device {telemetryEvent.DeviceId} not found for customer {customerId}. Cannot add telemetry event.");
            return null;
        }

        await _telemetryRepository.AddEvent(customerId, telemetryEvent, cancellationToken);

        _logger.LogInformation($"Added telemetry event {telemetryEvent.EventId} for device {telemetryEvent.DeviceId} of customer {telemetryEvent.CustomerId}");

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
