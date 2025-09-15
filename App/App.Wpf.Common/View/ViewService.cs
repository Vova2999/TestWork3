using System.Diagnostics;
using System.Windows;
using App.Wpf.Common.Base;
using App.Wpf.Common.Dispatcher;
using App.Wpf.Common.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Grace.DependencyInjection;

namespace App.Wpf.Common.View;

public class ViewService : IViewService, IDisposable
{
    private readonly IMessenger _messenger;
    private readonly IDispatcher _dispatcher;
    private readonly ILocatorService _locatorService;
    private readonly List<Window> _openedWindows;

    public ViewService(IMessenger messenger, IDispatcher dispatcher, ILocatorService locatorService)
    {
        _messenger = messenger;
        _dispatcher = dispatcher;
        _locatorService = locatorService;
        _openedWindows = new List<Window>();

        _messenger.Register<RequestCloseMessage>(this, OnRequestClose);
    }

    [DebuggerStepThrough]
    public void OpenWindow<TViewModel>() where TViewModel : IViewModel
    {
        CreateWindow<TViewModel>(WindowMode.Window).Show();
    }

    [DebuggerStepThrough]
    public void OpenWindow(IViewModel viewModel)
    {
        CreateWindow(viewModel, WindowMode.Window).Show();
    }

    [DebuggerStepThrough]
    public bool? OpenDialog<TViewModel>() where TViewModel : IViewModel
    {
        return CreateWindow<TViewModel>(WindowMode.Dialog).ShowDialog();
    }

    [DebuggerStepThrough]
    public bool? OpenDialog(IViewModel viewModel)
    {
        return CreateWindow(viewModel, WindowMode.Dialog).ShowDialog();
    }

    [DebuggerStepThrough]
    public Window CreateWindow<TViewModel>(WindowMode windowMode) where TViewModel : IViewModel
    {
        return CreateWindow(_locatorService.Locate<TViewModel>(), windowMode);
    }

    [DebuggerStepThrough]
    public Window CreateWindow(IViewModel viewModel, WindowMode windowMode)
    {
        var window = (Window) viewModel.View;
        window.DataContext = viewModel;
        window.Closed += OnClosed;

        lock (_openedWindows)
        {
            if (windowMode == WindowMode.Dialog && _openedWindows.Any())
            {
                var lastOpened = _openedWindows.Last();
                if (lastOpened.IsActive && !Equals(window, lastOpened))
                    window.Owner = lastOpened;
            }

            _openedWindows.Add(window);
        }

        return window;
    }

    public int GetOpenedWindowsCount()
    {
        lock (_openedWindows)
            return _openedWindows.Count;
    }

    private void OnRequestClose(object recipient, RequestCloseMessage message)
    {
        lock (_openedWindows)
        {
            var window = _openedWindows.SingleOrDefault(w => w.DataContext == message.ViewModel);
            if (window == null)
                return;

            _dispatcher.InvokeInUi(() =>
            {
                if (message.DialogResult != null)
                    window.DialogResult = message.DialogResult;
                else
                    window.Close();
            });
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        if (sender is not Window window)
            return;

        window.Closed -= OnClosed;
        lock (_openedWindows)
            _openedWindows.Remove(window);
    }

    public void Dispose()
    {
        _messenger.UnregisterAll(this);
    }
}