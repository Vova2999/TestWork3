using System.Text.Json;
using System.Text.Json.Serialization;
using Client.Api.Converters;
using Client.Api.Exceptions;
using Client.Common.Extensions;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.Json;

namespace Client.Api.Clients;

public abstract class HttpClientBase : IDisposable
{
    private readonly RestClient _client;

    protected HttpClientBase(string address, TimeSpan timeout)
    {
        _client = new RestClient(address,
            options => options.Timeout = timeout,
            configureSerialization: config =>
                config.UseSystemTextJson(
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = null,
                        Converters = { new CommandDtoJsonConverter(), new CommandResultDtoJsonConverter() },
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    }));
    }

    protected Task<byte[]> SendRequestAsync(
        Method method,
        string path,
        CancellationToken cancellationToken)
    {
        return SendRequestAsync(method, path, null, null, cancellationToken);
    }

    protected Task<byte[]> SendRequestAsync(
        Method method,
        string path,
        IAuthenticator? authenticator,
        CancellationToken cancellationToken)
    {
        return SendRequestAsync(method, path, authenticator, null, cancellationToken);
    }

    protected Task<byte[]> SendRequestAsync(
        Method method,
        string path,
        Action<RestRequest>? setRequestParams,
        CancellationToken cancellationToken)
    {
        return SendRequestAsync(method, path, null, setRequestParams, cancellationToken);
    }

    protected async Task<byte[]> SendRequestAsync(
        Method method,
        string path,
        IAuthenticator? authenticator,
        Action<RestRequest>? setRequestParams,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var request = CreateRequest(method, path, authenticator, setRequestParams);
        var response = await _client.ExecuteAsync(request, cancellationToken);

        if (response.IsSuccessful && response.RawBytes?.Equals(null) == false)
            return response.RawBytes;

        var headers = response.Headers.EmptyIfNull().ToLookup(h => h.Name, h => h.Value);
        throw new SendRequestException(response.ResponseUri, response.Content, response.StatusCode, headers!, response.ErrorException);
    }

    protected Task<TResult> SendRequestAsync<TResult>(
        Method method,
        string path,
        CancellationToken cancellationToken)
    {
        return SendRequestAsync<TResult>(method, path, null, null, cancellationToken);
    }

    protected Task<TResult> SendRequestAsync<TResult>(
        Method method,
        string path,
        IAuthenticator? authenticator,
        CancellationToken cancellationToken)
    {
        return SendRequestAsync<TResult>(method, path, authenticator, null, cancellationToken);
    }

    protected Task<TResult> SendRequestAsync<TResult>(
        Method method,
        string path,
        Action<RestRequest>? setRequestParams,
        CancellationToken cancellationToken)
    {
        return SendRequestAsync<TResult>(method, path, null, setRequestParams, cancellationToken);
    }

    protected async Task<TResult> SendRequestAsync<TResult>(
        Method method,
        string path,
        IAuthenticator? authenticator,
        Action<RestRequest>? setRequestParams,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var request = CreateRequest(method, path, authenticator, setRequestParams);
        var response = await _client.ExecuteAsync<TResult>(request, cancellationToken);

        if (response.IsSuccessful && response.Data?.Equals(null) == false)
            return response.Data;

        var headers = response.Headers.EmptyIfNull().ToLookup(h => h.Name, h => h.Value);
        throw new SendRequestException<TResult>(response.ResponseUri, response.Content, response.StatusCode, headers!, response.Data, response.ErrorException);
    }

    private static RestRequest CreateRequest(Method method, string resource, IAuthenticator? authenticator, Action<RestRequest>? setRequestParams)
    {
        var request = new RestRequest(resource, method);

        if (authenticator != null)
            request.Authenticator = authenticator;

        setRequestParams?.Invoke(request);
        return request;
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}