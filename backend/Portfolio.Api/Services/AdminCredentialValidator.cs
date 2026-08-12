using System.Security.Cryptography;
using System.Text;

namespace Portfolio.Api.Services;

public sealed class AdminCredentialValidator(IConfiguration configuration)
{
    public bool IsValid(string email, string password)
    {
        var configuredEmail = configuration["Admin:Email"];
        var configuredPassword = configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(configuredEmail) || string.IsNullOrWhiteSpace(configuredPassword))
        {
            return false;
        }

        return FixedEquals(email.Trim().ToLowerInvariant(), configuredEmail.Trim().ToLowerInvariant())
            && FixedEquals(password, configuredPassword);
    }

    private static bool FixedEquals(string value, string expected) =>
        CryptographicOperations.FixedTimeEquals(
            SHA256.HashData(Encoding.UTF8.GetBytes(value)),
            SHA256.HashData(Encoding.UTF8.GetBytes(expected)));
}
