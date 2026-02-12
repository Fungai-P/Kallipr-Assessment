using DeviceTelemetryService.Api.Endpoints;
using DeviceTelemetryService.Domain.Handlers;
using DeviceTelemetryService.Helpers;
using DeviceTelemetryService.Middleware;
using DeviceTelemetryService.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

var services = builder.Services;

services.Configure<IdempotencyOptions>(
    builder.Configuration.GetSection("Idempotency"));

services.AddSingleton<TenantContext>();
services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});
services.AddScoped<IDeviceRepository, DeviceRepository>();
services.AddScoped<ITelemetryRepository, TelemetryRepository>();
services.AddScoped<IDeviceHandler, DeviceHandler>();
services.AddScoped<ITelemetryHandler, TelemetryHandler>();

services.AddCors(opt =>
{
    opt.AddDefaultPolicy(p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed(_ => true));
});

services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("sqlite");

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Device Telemetry API",
        Version = "v1"
    });

    // Define the tenant header
    c.AddSecurityDefinition("Tenant", new OpenApiSecurityScheme
    {
        Name = "X-Customer-Id",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Tenant / Customer ID (e.g. acme-123 or beta-456)"
    });

    // Apply it globally to all endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Tenant"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseCors();
app.Use(async (ctx, next) =>
{
    // simple request logging
    app.Logger.LogInformation("HTTP {Method} {Path}", ctx.Request.Method, ctx.Request.Path);
    await next();
});

// health does NOT require tenant header
app.MapHealthChecks("/healthz");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Apply tenant middleware ONLY for API routes
app.UseWhen(
    ctx => ctx.Request.Path.StartsWithSegments("/api"),
    branch =>
    {
        branch.UseMiddleware<TenantMiddleware>();
    }
);
app.UseMiddleware<GlobalErrorMiddleware>();

DeviceEndpoints.Map(app.MapGroup("/api"));
TelemetryEndpoints.Map(app.MapGroup("/api"));

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var options = scope.ServiceProvider.GetRequiredService<IOptions<IdempotencyOptions>>();
    await DataSeeder.EnsureSeededAsync(db, options);
}

app.Run();
