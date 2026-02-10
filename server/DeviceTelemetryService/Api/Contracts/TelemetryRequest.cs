namespace DeviceTelemetryService.Api.Contracts
{
    public record TelemetryRequest(
        string DeviceId,
        string EventId,
        string Type,
        double Value,
        string Unit,
        DateTime RecordedAt
    );
}
