using DeviceTelemetryService.Api.Contracts;
using DeviceTelemetryService.Domain.Entities;

namespace DeviceTelemetryService.Domain.Handlers.Mappers;

public static class DeviceMapper
{
    public static DeviceResponse Map(this Device device)
    {
        return new DeviceResponse
        {
            CustomerId = device.CustomerId,
            DeviceId = device.DeviceId,
            Id = device.Id,
            Label = device.Label,
            Location = device.Location
        };
    }

    public static List<DeviceResponse> Map(this List<Device> devices)
    {
        return devices.Select(x => x.Map()).ToList();
    }
}
