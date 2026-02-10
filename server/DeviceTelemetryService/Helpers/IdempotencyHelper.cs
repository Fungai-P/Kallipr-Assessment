using DeviceTelemetryService.Domain.Entities;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace DeviceTelemetryService.Helpers;

public class IdempotencyHelper
{
    public static string ComputeKey(TelemetryEvent telemetryEvent, string secret)
    {
        var canonical = Canonicalize(telemetryEvent);
        return ComputeHmacSha256(canonical, secret);
    }

    private static string Canonicalize(TelemetryEvent telemetryEvent)
    {
        // Normalize DateTime to UTC + round-trip format
        var recordedAtUtc = telemetryEvent.RecordedAt.Kind == DateTimeKind.Utc
            ? telemetryEvent.RecordedAt
            : telemetryEvent.RecordedAt.ToUniversalTime();

        // Normalize floating-point representation
        var value = telemetryEvent.Value.ToString("R", CultureInfo.InvariantCulture);

        return string.Join("|",
            telemetryEvent.CustomerId?.Trim(),
            telemetryEvent.DeviceId?.Trim(),
            telemetryEvent.EventId?.Trim(),
            recordedAtUtc.ToString("O", CultureInfo.InvariantCulture),
            telemetryEvent.Type?.Trim(),
            value,
            telemetryEvent.Unit?.Trim()
        );
    }

    private static string ComputeHmacSha256(string input, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var inputBytes = Encoding.UTF8.GetBytes(input);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(inputBytes);

        // Hex string (64 chars)
        return Convert.ToHexString(hash);
    }
}
