using DeviceTelemetryService.Domain.Handlers;
using DeviceTelemetryService.Middleware;

namespace DeviceTelemetryService.Api.Endpoints;

public static class DeviceEndpoints
{
    public static IEndpointRouteBuilder Map(this IEndpointRouteBuilder app)
    {
        app.MapGet("/devices", async (
            TenantContext tenant,
            IDeviceHandler handler,
            CancellationToken cancellationToken) =>
        {
            var items = await handler.GetDevices(tenant.CustomerId, cancellationToken);

            return Results.Ok(items);
        });

        return app;
    }
}
