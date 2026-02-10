using DeviceTelemetryService.Domain.Entities;
using DeviceTelemetryService.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace DeviceTelemetryService.Repositories;

public class TelemetryRepository(AppDbContext _dbContext, IOptions<IdempotencyOptions> _options) : ITelemetryRepository
{
    public async Task<TelemetryEvent> AddEvent(string customerId, TelemetryEvent telemetryEvent, CancellationToken cancellationToken)
    {
        var secretKey = _options.Value.Secret
            ?? throw new InvalidOperationException("Idempotency secret not configured");

        telemetryEvent.IdempotencyKey = IdempotencyHelper.ComputeKey(telemetryEvent, secretKey);
        await _dbContext.TelemetryEvents.AddAsync(telemetryEvent, cancellationToken);
        _dbContext.SaveChanges();

        return telemetryEvent;
    }

    public async Task<List<TelemetryEvent>> GetEventsAsync(string customerId, CancellationToken cancellationToken)
    {
        return await _dbContext.TelemetryEvents
            .Where(x => x.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TelemetryEvent>> GetEventsAsync(Expression<Func<TelemetryEvent, bool>> predicate, CancellationToken cancellationToken)
    {
        IQueryable<TelemetryEvent> query = _dbContext.TelemetryEvents
            .Where(predicate)
            .AsNoTracking();

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<bool> IdempotencyKeyMatch(string customerId, string deviceId, string idempotencyKey, CancellationToken cancellationToken)
    {
        var lastEvent = await _dbContext.TelemetryEvents
            .Where(x => x.CustomerId == customerId)
            .Where(x => x.DeviceId == deviceId)
            .OrderByDescending(e => e.RecordedAt)
            .AsNoTracking()
            .FirstOrDefaultAsync();
        if (lastEvent == null) return false;

        return (!string.IsNullOrEmpty(lastEvent.IdempotencyKey) && lastEvent.IdempotencyKey == idempotencyKey) ? true : false;
    }
}
