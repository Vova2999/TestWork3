using Client.Domain.Dtos;
using RestSharp.Authenticators;

namespace Client.Api.Token;

public interface IAuthenticatorProvider
{
    Task LoginAsync(LoginDto login);
    void Logout();

    Task<TResult> ExecuteWithAuthenticatorAsync<TResult>(Func<IAuthenticator, Task<TResult>> action);
}