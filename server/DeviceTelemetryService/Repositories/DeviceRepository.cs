using DeviceTelemetryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeviceTelemetryService.Repositories;

public class DeviceRepository(AppDbContext _dbContext) : IDeviceRepository
{

    public async Task<List<Device>> GetDevices(string customerId, CancellationToken cancellationToken)
    {
        return await _dbContext.Devices
            .Where(x => x.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Device?> GetSingleAsync(Expression<Func<Device, bool>> predicate, CancellationToken cancellationToken)
    {
        IQueryable<Device> query = _dbContext.Devices.AsNoTracking();

        return await query.FirstOrDefaultAsync(predicate);
    }
}
