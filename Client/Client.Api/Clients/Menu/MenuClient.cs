using Client.Api.Token;
using Client.Domain.Dtos;
using RestSharp;

namespace Client.Api.Clients.Menu;

public class MenuClient : HttpClientBase, IMenuClient
{
    private readonly IAuthenticatorProvider _authenticatorProvider;

    public MenuClient(IAuthenticatorProvider authenticatorProvider, string address, TimeSpan timeout)
        : base(address, timeout)
    {
        _authenticatorProvider = authenticatorProvider;
    }

    public Task<CommandResultDto> HandleAsync(
        CommandDto commandDto,
        CancellationToken cancellationToken = default)
    {
        return _authenticatorProvider.ExecuteWithAuthenticatorAsync(authenticator =>
            SendRequestAsync<CommandResultDto>(
                Method.Post,
                "api/menu",
                authenticator,
                request => request.AddBody(commandDto),
                cancellationToken));
    }
}