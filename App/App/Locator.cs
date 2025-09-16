using App.Services.EnvironmentVariables;
using App.Settings;
using App.Views.Main.Logic;
using App.Wpf.Common.Dispatcher;
using App.Wpf.Common.MessageBox;
using App.Wpf.Common.View;
using CommunityToolkit.Mvvm.Messaging;
using Grace.DependencyInjection;
using Grace.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace App;

public static class Locator
{
    private static readonly Lazy<ILocatorService> Lazy = new(CreateContainer);
    public static ILocatorService Current => Lazy.Value;

    public static string FactoryName => "IFactory";

    private static ILocatorService CreateContainer()
    {
        var container = new DependencyInjectionContainer();
        container.Configure(RegisterDependencies);

        return container;
    }

    private static void RegisterDependencies(IExportRegistrationBlock registration)
    {
        var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environmentName}.json", true)
            .Build();

        registration.ExportInterfaceFactories(type => type.Name == FactoryName);

        RegisterAppSettings(registration, config);
        RegisterLogger(registration, config);

        RegisterServices(registration);
    }

    private static void RegisterAppSettings(IExportRegistrationBlock registration, IConfiguration config)
    {
        var appSettings = config.GetSection(nameof(AppSettings)).Get<AppSettings>();

        RegisterSingleton<IAppSettings>(registration, appSettings);
    }

    private static void RegisterLogger(IExportRegistrationBlock registration, IConfiguration config)
    {
        LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
        LogManager.ReconfigExistingLoggers();

        RegisterSingleton(registration, LoggerFactory.Create(builder => builder.AddNLog()));
        registration.Export(typeof(Logger<>)).As(typeof(ILogger<>)).Lifestyle.Singleton();
    }

    private static void RegisterServices(IExportRegistrationBlock registration)
    {
        RegisterSingleton<IMainWindowProvider, MainWindowProvider>(registration);

        RegisterSingleton<IDispatcher, Dispatcher>(registration);
        RegisterSingleton<IViewService, ViewService>(registration);
        RegisterSingleton<IMessageBoxService, MessageBoxService>(registration);

        RegisterSingleton<IMessenger, WeakReferenceMessenger>(registration);

        RegisterSingleton<IEnvironmentVariablesProvider, EnvironmentVariablesProvider>(registration,
            _ => new EnvironmentVariablesProvider(Constants.EnvironmentVariablesFileName));
    }

    private static void RegisterSingleton<TType>(IExportRegistrationBlock registrationBlock, TType? instance)
    {
        registrationBlock.ExportInstance(instance).As<TType>().Lifestyle.Singleton();
    }

    private static void RegisterSingleton<TFrom, TTo>(IExportRegistrationBlock registrationBlock) where TTo : TFrom
    {
        registrationBlock.Export<TTo>().As<TFrom>().Lifestyle.Singleton();
    }

    private static void RegisterSingleton<TFrom, TTo>(IExportRegistrationBlock registrationBlock, Func<IExportLocatorScope, TTo> injectionFactory) where TTo : TFrom
    {
        registrationBlock.ExportFactory(injectionFactory).As<TFrom>().Lifestyle.Singleton();
    }
}