namespace DeviceTelemetryService.Domain.Entities;

public class TelemetryEvent
{
    public Guid Id { get; set; }
    public string IdempotencyKey { get; set; }
    public string CustomerId { get; set; }
    public string DeviceId { get; set; }
    public string EventId { get; set; }
    public DateTime RecordedAt { get; set; }
    public string Type { get; set; }
    public double Value { get; set; }
    public string Unit { get; set; }
}
