namespace DeviceTelemetryService.Domain.Entities;

public class Device
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; }
    public string DeviceId { get; set; }
    public string Label { get; set; }
    public string Location { get; set; }
}
