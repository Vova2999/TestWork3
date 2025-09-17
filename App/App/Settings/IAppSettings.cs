namespace App.Settings;

public interface IAppSettings
{
    string[] EnvironmentVariableKeys { get; }
}