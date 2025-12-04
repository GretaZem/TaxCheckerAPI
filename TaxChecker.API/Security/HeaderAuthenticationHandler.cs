using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace TaxChecker.API.Security;

public sealed class HeaderAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public HeaderAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Role", out var roleValue))
            return Task.FromResult(AuthenticateResult.Fail("Missing X-Role header"));

        string rawRole = roleValue.ToString();
        if (!AppRoles.TryNormalize(rawRole, out var role))
            return Task.FromResult(AuthenticateResult.Fail("Invalid role"));

        var claims = new[]
        {
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
