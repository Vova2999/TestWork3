using System.ComponentModel;
using System.Windows;
using App.Wpf.Common.Dispatcher;
using App.Wpf.Common.View;

namespace App.Views.Main.Logic;

public class MainWindowProvider : IMainWindowProvider
{
    private readonly IViewService _viewService;
    private readonly IDispatcher _dispatcher;

    private Window? _mainWindow;

    public MainWindowProvider(IViewService viewService, IDispatcher dispatcher)
    {
        _viewService = viewService;
        _dispatcher = dispatcher;
    }

    public void Show()
    {
        _dispatcher.InvokeInUi(() =>
        {
            _mainWindow ??= CreateWindow();
            Application.Current.MainWindow = _mainWindow;
            _mainWindow.Show();
        });
    }

    public void CloseIfCreated()
    {
        _dispatcher.InvokeInUi(() => _mainWindow?.Close());
    }

    private Window CreateWindow()
    {
        var window = _viewService.CreateWindow<MainViewModel>(WindowMode.Window);
        window.Closing += OnWindowClosing;
        return window;
    }

    private void OnWindowClosing(object? sender, CancelEventArgs e)
    {
        _dispatcher.InvokeInUi(() =>
        {
            if (_mainWindow == null)
                return;

            _mainWindow.Closing -= OnWindowClosing;
            _mainWindow = null;
        });
    }
}