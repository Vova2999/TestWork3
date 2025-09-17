using System.Text.Json;
using App.Common.Extensions;
using App.Domain;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

namespace App.Services.EnvironmentVariables;

public class EnvironmentVariablesProvider : IEnvironmentVariablesProvider
{
    private readonly string _environmentVariablesFilePath;
    private readonly ILogger<EnvironmentVariablesProvider> _logger;

    private readonly AsyncReaderWriterLock _readerWriterLock = new();

    public EnvironmentVariablesProvider(
        string environmentVariablesFileName,
        ILogger<EnvironmentVariablesProvider> logger)
    {
        _environmentVariablesFilePath = Path.Combine(AppContext.BaseDirectory, environmentVariablesFileName);
        _logger = logger;
    }

    public async Task<ICollection<EnvironmentVariable>> GetEnvironmentVariablesAsync()
    {
        try
        {
            return await GetEnvironmentVariablesInternalAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error on read environment variables");
            return [];
        }
    }

    private async Task<ICollection<EnvironmentVariable>> GetEnvironmentVariablesInternalAsync()
    {
        using var _ = await _readerWriterLock.ReaderLockAsync();

        if (!File.Exists(_environmentVariablesFilePath))
            return [];

        var stream = File.OpenRead(_environmentVariablesFilePath);

        await using (stream.ConfigureAwait(false))
        {
            var deserializedData = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream).ConfigureAwait(false);
            return deserializedData.EmptyIfNull().Select(pair => new EnvironmentVariable { Key = pair.Key, Value = pair.Value }).ToArray();
        }
    }

    public async Task SaveEnvironmentVariablesAsync(ICollection<EnvironmentVariable> environmentVariables)
    {
        try
        {
            await SaveEnvironmentVariablesInternalAsync(environmentVariables).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error on save environment variables");
        }
    }

    private async Task SaveEnvironmentVariablesInternalAsync(ICollection<EnvironmentVariable> environmentVariables)
    {
        using var _ = await _readerWriterLock.WriterLockAsync();

        var options = new JsonSerializerOptions { WriteIndented = true };
        var serializedData = environmentVariables.ToDictionary(variable => variable.Key, variable => variable.Value);

        var stream = File.Create(_environmentVariablesFilePath);

        await using (stream.ConfigureAwait(false))
            await JsonSerializer.SerializeAsync(stream, serializedData, options).ConfigureAwait(false);
    }
}