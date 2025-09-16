using System.Text.Json;
using App.Common.Extensions;
using App.Domain;
using Nito.AsyncEx;

namespace App.Services.EnvironmentVariables;

public class EnvironmentVariablesProvider : IEnvironmentVariablesProvider
{
    private readonly string _environmentVariablesFilePath;
    private readonly AsyncReaderWriterLock _readerWriterLock = new();

    public EnvironmentVariablesProvider(string environmentVariablesFileName)
    {
        _environmentVariablesFilePath = Path.Combine(AppContext.BaseDirectory, environmentVariablesFileName);
    }

    public async Task<ICollection<EnvironmentVariable>> GetEnvironmentVariablesAsync()
    {
        using var _ = await _readerWriterLock.ReaderLockAsync();

        if (!File.Exists(_environmentVariablesFilePath))
            return [];

        var stream = File.OpenRead(_environmentVariablesFilePath);

        await using (stream.ConfigureAwait(false))
        {
            var environmentVariables = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream).ConfigureAwait(false);
            return environmentVariables.EmptyIfNull().Select(pair => new EnvironmentVariable { Key = pair.Key, Value = pair.Value }).ToArray();
        }
    }

    public async Task SaveEnvironmentVariablesAsync(ICollection<EnvironmentVariable> environmentVariables)
    {
        using var _ = await _readerWriterLock.WriterLockAsync();

        var options = new JsonSerializerOptions { WriteIndented = true };
        var dictionary = environmentVariables.ToDictionary(variable => variable.Key, variable => variable.Value);

        var stream = File.Create(_environmentVariablesFilePath);

        await using (stream.ConfigureAwait(false))
            await JsonSerializer.SerializeAsync(stream, dictionary, options).ConfigureAwait(false);
    }
}