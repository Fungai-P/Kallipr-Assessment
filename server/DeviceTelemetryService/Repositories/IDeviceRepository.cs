using DeviceTelemetryService.Domain.Entities;
using System.Linq.Expressions;

namespace DeviceTelemetryService.Repositories;

public interface IDeviceRepository
{
    Task<List<Device>> GetDevices(string customerId, CancellationToken cancellationToken);
    Task<Device?> GetSingleAsync(Expression<Func<Device, bool>> predicate, CancellationToken cancellationToken);
}
