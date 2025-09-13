using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Server.Services.Managers;

namespace Server.Authentication;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string HeaderPrefix = "Basic ";

    private readonly ApplicationContextUserManager _applicationContextUserManager;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ApplicationContextUserManager applicationContextUserManager)
        : base(options, logger, encoder)
    {
        _applicationContextUserManager = applicationContextUserManager;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.Fail("Missing Authorization header");

        var header = Request.Headers["Authorization"].ToString();
        if (!header.StartsWith(HeaderPrefix, StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.Fail("Authorization header does not start with 'Basic'");

        try
        {
            var headerValue = Encoding.UTF8.GetString(Convert.FromBase64String(header[HeaderPrefix.Length..]));
            var authSplit = headerValue.Split(':', 2);

            if (authSplit.Length != 2)
                return AuthenticateResult.Fail("Invalid Authorization header format");

            var username = authSplit[0];
            var password = authSplit[1];

            var user = await _applicationContextUserManager.FindByNameAsync(username);
            if (user == null || !await _applicationContextUserManager.CheckPasswordAsync(user, password))
                return AuthenticateResult.Fail("Invalid credentials");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid Authorization header");
        }
    }
}