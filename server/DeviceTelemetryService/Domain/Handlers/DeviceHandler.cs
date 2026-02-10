using DeviceTelemetryService.Domain.Entities;
using DeviceTelemetryService.Repositories;

namespace DeviceTelemetryService.Domain.Handlers;

public class DeviceHandler(IDeviceRepository _deviceRepository) : IDeviceHandler
{
    public async Task<List<Device>> GetDevices(string customerId, CancellationToken cancellationToken)
    {
        return await _deviceRepository.GetDevices(customerId, cancellationToken);
    }
}
