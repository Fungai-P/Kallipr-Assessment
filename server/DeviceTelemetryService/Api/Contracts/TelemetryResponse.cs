namespace DeviceTelemetryService.Api.Contracts;

public class TelemetryResponse
{
    public Guid Id { get; set; }
    public string DeviceId { get; set; }
    public string EventId { get; set; }
    public string Type { get; set; }
    public double Value { get; set; }
    public string Unit { get; set; }
    public DateTime RecordedAt { get; set; }
}
