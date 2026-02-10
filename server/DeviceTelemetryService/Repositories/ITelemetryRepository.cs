using DeviceTelemetryService.Domain.Entities;
using System.Linq.Expressions;

namespace DeviceTelemetryService.Repositories;

public interface ITelemetryRepository
{
    Task<TelemetryEvent> AddEvent(string customerId, TelemetryEvent telemetryEvent, CancellationToken cancellationToken);
    Task<List<TelemetryEvent>> GetEventsAsync(string customerId, CancellationToken cancellationToken);
    Task<List<TelemetryEvent>> GetEventsAsync(Expression<Func<TelemetryEvent, bool>> predicate, CancellationToken cancellationToken);
    Task<bool> IdempotencyKeyMatch(string customerId, string deviceId, string idempotencyKey, CancellationToken cancellationToken);
}
