using Client.Domain.Dtos;
using RestSharp.Authenticators;

namespace Client.Api.Token;

public class BasicAuthenticatorProvider : IAuthenticatorProvider
{
    private IAuthenticator? _authenticator;

    public Task LoginAsync(LoginDto login)
    {
        _authenticator = new HttpBasicAuthenticator(login.Login, login.Password);

        return Task.CompletedTask;
    }

    public void Logout()
    {
        _authenticator = null;
    }

    public async Task<TResult> ExecuteWithAuthenticatorAsync<TResult>(Func<IAuthenticator, Task<TResult>> action)
    {
        return await action(_authenticator ?? throw new InvalidOperationException($"You must login before using the {nameof(_authenticator)}"));
    }
}