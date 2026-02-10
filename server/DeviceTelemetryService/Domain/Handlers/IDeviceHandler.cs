using DeviceTelemetryService.Domain.Entities;

namespace DeviceTelemetryService.Domain.Handlers;

public interface IDeviceHandler
{
    Task<List<Device>> GetDevices(string customerId, CancellationToken cancellationToken);
}
