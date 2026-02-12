using DeviceTelemetryService.Api.Contracts;
using DeviceTelemetryService.Domain.Handlers;
using DeviceTelemetryService.Domain.Handlers.Mappers;
using DeviceTelemetryService.Filters;
using DeviceTelemetryService.Middleware;

namespace DeviceTelemetryService.Api.Endpoints;

public static class TelemetryEndpoints
{
    public static IEndpointRouteBuilder Map(this IEndpointRouteBuilder app)
    {
        // Add a new telemetry entry
        app.MapPost("/telemetry", async (
            TelemetryRequest request,
            TenantContext tenant,
            ITelemetryHandler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.AddEventAsync(tenant.CustomerId, request.Map(tenant.CustomerId), cancellationToken);
            if (result == null)
            {
                return Results.BadRequest("Failed to add telemetry event.");
            }

            return Results.Ok(result?.Map());
        })
            .AddEndpointFilter<IdempotencyEndpointFilter>()
            .Produces<TelemetryResponse>();

        // Get all telemetry events for the customer
        app.MapGet("/telemetry", async (
            TenantContext tenant,
            ITelemetryHandler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.GetEventsAsync(tenant.CustomerId, cancellationToken);

            return Results.Ok(result?.Map());
        }).Produces<List<TelemetryResponse>>();

        // Get all telemetry events for the customers device
        app.MapGet("/devices/{deviceId}/telemetry", async (
            string deviceId,
            TenantContext tenant,
            ITelemetryHandler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.GetEventsByDevice(tenant.CustomerId, deviceId, cancellationToken);
            return Results.Ok(result?.Map());
        }).Produces<List<TelemetryResponse>>();

        return app;
    }
}
