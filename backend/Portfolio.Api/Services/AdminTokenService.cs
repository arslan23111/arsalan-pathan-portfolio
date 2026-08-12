using System.Security.Cryptography;
using System.Text;

namespace Portfolio.Api.Services;

public sealed class AdminTokenService(IConfiguration configuration)
{
    public string Create(string email)
    {
        var expires = DateTimeOffset.UtcNow.AddHours(2).ToUnixTimeSeconds();
        var payload = $"{email.Trim().ToLowerInvariant()}|{expires}";
        return $"{Encode(payload)}.{Sign(payload)}";
    }

    public bool TryValidate(string token, out string email)
    {
        email = string.Empty;
        var parts = token.Split('.', 2);
        if (parts.Length != 2) return false;

        try
        {
            var payload = Decode(parts[0]);
            if (!FixedEquals(parts[1], Sign(payload))) return false;
            var values = payload.Split('|', 2);
            if (values.Length != 2 || !long.TryParse(values[1], out var expiry)) return false;
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= expiry) return false;
            email = values[0];
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private string Sign(string payload)
    {
        var secret = configuration["Admin:Password"]
            ?? throw new InvalidOperationException("Admin password is not configured.");
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static string Encode(string value) => Convert.ToBase64String(Encoding.UTF8.GetBytes(value))
        .TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static string Decode(string value)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        padded += new string('=', (4 - padded.Length % 4) % 4);
        return Encoding.UTF8.GetString(Convert.FromBase64String(padded));
    }

    private static bool FixedEquals(string left, string right) => CryptographicOperations.FixedTimeEquals(
        Encoding.UTF8.GetBytes(left), Encoding.UTF8.GetBytes(right));
}
