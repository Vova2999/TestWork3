using App.Domain;

namespace App.Services.EnvironmentVariables;

public interface IEnvironmentVariablesProvider
{
    Task<ICollection<EnvironmentVariable>> GetEnvironmentVariablesAsync();
    Task SaveEnvironmentVariablesAsync(ICollection<EnvironmentVariable> environmentVariables);
}