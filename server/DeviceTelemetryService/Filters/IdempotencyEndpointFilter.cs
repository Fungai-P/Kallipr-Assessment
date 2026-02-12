using DeviceTelemetryService.Api.Contracts;
using DeviceTelemetryService.Domain.Handlers.Mappers;
using DeviceTelemetryService.Helpers;
using DeviceTelemetryService.Middleware;
using DeviceTelemetryService.Repositories;
using Microsoft.Extensions.Options;

namespace DeviceTelemetryService.Filters;

public class IdempotencyEndpointFilter(
    ITelemetryRepository _telemetryRepository,
    IOptions<IdempotencyOptions> _options,
    ILogger<IdempotencyEndpointFilter> _logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var secretKey = _options.Value.Secret
                  ?? throw new InvalidOperationException("Idempotency secret not configured");

        var request = context.GetArgument<TelemetryRequest>(0);
        var tenant = context.GetArgument<TenantContext>(1);

        var keyMatched = await _telemetryRepository.IdempotencyKeyMatch(
            tenant.CustomerId,
            request.DeviceId,
            IdempotencyHelper.ComputeKey(request.Map(tenant.CustomerId), secretKey),
            CancellationToken.None);

        if (keyMatched)
        {
            _logger.LogInformation($"Possible duplicate: Idempotency key matched for device {request.DeviceId} for tenant customer {tenant.CustomerId}");
            return Results.Conflict();
        }            

        var result = await next(context);

        return result;
    }
}