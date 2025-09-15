using App.Views.Main.Logic;
using App.Wpf.Common.Dispatcher;
using App.Wpf.Common.MessageBox;
using App.Wpf.Common.View;
using CommunityToolkit.Mvvm.Messaging;
using Grace.DependencyInjection;
using Grace.Factory;

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
        registration.ExportInterfaceFactories(type => type.Name == FactoryName);

        RegisterServices(registration);
    }

    private static void RegisterServices(IExportRegistrationBlock registration)
    {
        RegisterSingleton<IMainWindowProvider, MainWindowProvider>(registration);

        RegisterSingleton<IDispatcher, Dispatcher>(registration);
        RegisterSingleton<IViewService, ViewService>(registration);
        RegisterSingleton<IMessageBoxService, MessageBoxService>(registration);

        RegisterSingleton<IMessenger, WeakReferenceMessenger>(registration);
    }

    private static void RegisterSingleton<TFrom, TTo>(IExportRegistrationBlock registrationBlock) where TTo : TFrom
    {
        registrationBlock.Export<TTo>().As<TFrom>().Lifestyle.Singleton();
    }
}