#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace App.Settings;

public class AppSettings : IAppSettings
{
    public string[] EnvironmentVariableKeys { get; set; }
}