using System.Diagnostics.CodeAnalysis;

namespace TaxChecker.API.Security;

public static class AppRoles
{
    public const string User = "User";
    public const string Admin = "Admin";

    // Convenience: list roles for validation
    public static readonly string[] All = { User, Admin };

    // Should move to extension, keeping here for simplicity
    public static bool TryNormalize(
        string input,
        [NotNullWhen(true)] out string? normalized)
    {
        normalized = All.FirstOrDefault(r =>
            r.Equals(input, StringComparison.OrdinalIgnoreCase));

        return normalized != null;
    }

}
