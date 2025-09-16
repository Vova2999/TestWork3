using System.Collections.ObjectModel;
using App.Models;
using App.Services.EnvironmentVariables;
using App.Settings;
using Microsoft.Extensions.Logging;

namespace App.Views.Main;

public class DemoModel : MainViewModel
{
    public DemoModel() : base(
        DemoLocator.Locate<IAppSettings>(),
        DemoLocator.Locate<IEnvironmentVariablesProvider>(),
        DemoLocator.Locate<ILogger<MainViewModel>>())
    {
        EnvironmentVariables = new ObservableCollection<EnvironmentVariableModel>([
            new EnvironmentVariableModel { Key = "Key1", Value = "Value1" },
            new EnvironmentVariableModel { Key = "Key2", Value = "Value2" },
            new EnvironmentVariableModel { Key = "Key3", Value = "Value3" }
        ]);
    }
}