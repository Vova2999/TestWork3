using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using App.Common.Extensions;
using App.Extensions.Models;
using App.Models;
using App.Services.EnvironmentVariables;
using App.Settings;
using App.Wpf.Common.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace App.Views.Main;

public partial class MainViewModel : ViewModel<MainWindow>
{
    private readonly IAppSettings _appSettings;
    private readonly IEnvironmentVariablesProvider _environmentVariablesProvider;
    private readonly ILogger<MainViewModel> _logger;

    [ObservableProperty]
    private ObservableCollection<EnvironmentVariableModel> _environmentVariables = new();

    public MainViewModel(
        IAppSettings appSettings,
        IEnvironmentVariablesProvider environmentVariablesProvider,
        ILogger<MainViewModel> logger)
    {
        _appSettings = appSettings;
        _environmentVariablesProvider = environmentVariablesProvider;
        _logger = logger;
    }

    protected override void OnActivated()
    {
        HockEnvironmentVariablesEvents();
    }

    protected override void OnDeactivated()
    {
        UnHockEnvironmentVariablesEvents();
    }

    [RelayCommand]
    private async Task Loaded()
    {
        var environmentVariableKeys = _appSettings.EnvironmentVariableKeys;

        var environmentVariables = await _environmentVariablesProvider.GetEnvironmentVariablesAsync().ConfigureAwait(false);
        var environmentVariablesDictionary = environmentVariables.ToDictionary(variable => variable.Key, variable => variable.Value);

        EnvironmentVariables = new ObservableCollection<EnvironmentVariableModel>(
            environmentVariableKeys
                .Select(key => new EnvironmentVariableModel
                {
                    Key = key,
                    Value = environmentVariablesDictionary.GetValueOrDefault(key).EmptyIfNull()
                })
                .Concat(environmentVariablesDictionary
                    .ExceptBy(environmentVariableKeys, pair => pair.Key)
                    .Select(pair => new EnvironmentVariableModel
                    {
                        Key = pair.Key,
                        Value = pair.Value
                    })));
    }

    private void HockEnvironmentVariablesEvents()
    {
        EnvironmentVariables.CollectionChanged += EnvironmentVariablesChanged;
        foreach (var environmentVariable in EnvironmentVariables)
        {
            environmentVariable.PropertyChanging += EnvironmentVariableChanging;
            environmentVariable.PropertyChanged += EnvironmentVariableChanged;
        }
    }

    private void UnHockEnvironmentVariablesEvents()
    {
        EnvironmentVariables.CollectionChanged -= EnvironmentVariablesChanged;
        foreach (var environmentVariable in EnvironmentVariables)
        {
            environmentVariable.PropertyChanging -= EnvironmentVariableChanging;
            environmentVariable.PropertyChanged -= EnvironmentVariableChanged;
        }
    }

    private void EnvironmentVariablesChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var environmentVariable in (args.NewItems?.Cast<EnvironmentVariableModel>()).EmptyIfNull())
                {
                    environmentVariable.PropertyChanging += EnvironmentVariableChanging;
                    environmentVariable.PropertyChanged += EnvironmentVariableChanged;

                    if (environmentVariable.Key.IsNullOrEmpty() && environmentVariable.Value.IsNullOrEmpty())
                        _logger.LogInformation("Added new empty environment variable");
                    else
                        _logger.LogInformation($"Added new environment variable with" +
                            $" {nameof(EnvironmentVariableModel.Key)}: '{environmentVariable.Key}'" +
                            $" {nameof(EnvironmentVariableModel.Value)}: '{environmentVariable.Value}'");
                }

                SaveEnvironmentVariablesAsync().FireAndForgetSafeAsync();

                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (var environmentVariable in (args.OldItems?.Cast<EnvironmentVariableModel>()).EmptyIfNull())
                {
                    environmentVariable.PropertyChanging -= EnvironmentVariableChanging;
                    environmentVariable.PropertyChanged -= EnvironmentVariableChanged;

                    _logger.LogInformation($"Remove environment variable with" +
                        $" {nameof(EnvironmentVariableModel.Key)}: '{environmentVariable.Key}'" +
                        $" {nameof(EnvironmentVariableModel.Value)}: '{environmentVariable.Value}'");
                }

                SaveEnvironmentVariablesAsync().FireAndForgetSafeAsync();

                break;
        }
    }

    private void EnvironmentVariableChanging(object? sender, PropertyChangingEventArgs args)
    {
        if (sender is not EnvironmentVariableModel environmentVariable)
            return;

        _logger.LogInformation($"Changing environment variable with" +
            $" {nameof(EnvironmentVariableModel.Key)}: '{environmentVariable.Key}'" +
            $" {nameof(EnvironmentVariableModel.Value)}: '{environmentVariable.Value}'." +
            $" Updating property: {args.PropertyName}");
    }

    private void EnvironmentVariableChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (sender is not EnvironmentVariableModel environmentVariable)
            return;

        _logger.LogInformation($"Changed environment variable with" +
            $" {nameof(EnvironmentVariableModel.Key)}: '{environmentVariable.Key}'" +
            $" {nameof(EnvironmentVariableModel.Value)}: '{environmentVariable.Value}'." +
            $" Updated property: {args.PropertyName}");

        SaveEnvironmentVariablesAsync().FireAndForgetSafeAsync();
    }

    private Task SaveEnvironmentVariablesAsync()
    {
        var environmentVariables = EnvironmentVariables.Select(variable => variable.ToEntity()).ToArray();
        return _environmentVariablesProvider.SaveEnvironmentVariablesAsync(environmentVariables);
    }
}