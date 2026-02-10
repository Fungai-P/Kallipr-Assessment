using DeviceTelemetryService.Api.Contracts;
using DeviceTelemetryService.Domain.Entities;

namespace DeviceTelemetryService.Domain.Handlers.Mappers;

public static class TelemetryMapper
{
    public static TelemetryEvent Map(this TelemetryRequest request, string customerId)
    {
        return new TelemetryEvent
        {
            CustomerId = customerId,
            DeviceId = request.DeviceId,
            EventId = request.EventId,
            RecordedAt = request.RecordedAt,
            Type = request.Type,
            Unit = request.Unit,
            Value = request.Value
        };
    }

    public static TelemetryResponse Map(this TelemetryEvent telemetryEvent)
    {
        return new TelemetryResponse
        {
            Id = telemetryEvent.Id,
            DeviceId = telemetryEvent.DeviceId,
            EventId = telemetryEvent.EventId,
            RecordedAt = telemetryEvent.RecordedAt,
            Type = telemetryEvent.Type,
            Unit = telemetryEvent.Unit,
            Value = telemetryEvent.Value
        };
    }

    public static List<TelemetryResponse> Map(this List<TelemetryEvent> telemetryEvents)
    {
        return telemetryEvents.Select(e => e.Map()).ToList();
    }
}
