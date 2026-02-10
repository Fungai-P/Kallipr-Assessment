namespace DeviceTelemetryService.Helpers;

public sealed class IdempotencyOptions
{
    public string Secret { get; init; }
}
