using TaxChecker.API.Security;

namespace TaxChecker.Tests;

public sealed class AppRolesTests
{
    [Theory]
    [InlineData("User", AppRoles.User)]
    [InlineData("user", AppRoles.User)]
    [InlineData("ADMIN", AppRoles.Admin)]
    [InlineData("Admin", AppRoles.Admin)]
    public void TryNormalize_RecognizesValidRoles(string input, string expected)
    {
        var ok = AppRoles.TryNormalize(input, out var normalized);

        Assert.True(ok);
        Assert.Equal(expected, normalized);
    }

    [Theory]
    [InlineData("")]
    [InlineData("SomethingElse")]
    [InlineData("Root")]
    public void TryNormalize_ReturnsFalse_ForInvalidRoles(string input)
    {
        var ok = AppRoles.TryNormalize(input, out var normalized);

        Assert.False(ok);
        Assert.Null(normalized);
    }
}
