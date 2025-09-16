using CommunityToolkit.Mvvm.ComponentModel;

namespace App.Models;

public partial class EnvironmentVariableModel : ObservableObject
{
    [ObservableProperty]
    private string _key = string.Empty;

    [ObservableProperty]
    private string _value = string.Empty;
}