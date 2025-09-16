using App.Domain;
using App.Models;

namespace App.Extensions.Models;

public static class EnvironmentVariableModelExtensions
{
    public static EnvironmentVariable ToEntity(this EnvironmentVariableModel environmentVariable)
    {
        return new EnvironmentVariable
        {
            Key = environmentVariable.Key,
            Value = environmentVariable.Value
        };
    }
}