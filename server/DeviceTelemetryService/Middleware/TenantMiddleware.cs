namespace DeviceTelemetryService.Middleware;

public class TenantMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext ctx, TenantContext tenant)
    {
        var customerId =
            ctx.Request.Headers["X-Customer-Id"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(customerId))
        {
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            await ctx.Response.WriteAsync("Missing tenant. No X-Customer-Id header provided.");
            return;
        }

        tenant.CustomerId = customerId.Trim();
        await next(ctx);
    }
}
