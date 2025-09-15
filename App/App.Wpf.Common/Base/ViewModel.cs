using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace App.Wpf.Common.Base;

public abstract class ViewModel<TView> : ObservableRecipient, IViewModel where TView : FrameworkElement, new()
{
    private readonly object _viewLock = new();

    private FrameworkElement? _view;

    public FrameworkElement View
    {
        get
        {
            if (_view != null)
                return _view;

            lock (_viewLock)
                return _view ??= CreateView();
        }
    }

    private TView CreateView()
    {
        var view = new TView { DataContext = this };

        view.Loaded += OnLoaded;
        view.Unloaded += OnUnloaded;

        if (view.IsLoaded)
            OnLoaded(view, new RoutedEventArgs());

        return view;
    }

    private static void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: ObservableRecipient observableRecipient })
            observableRecipient.IsActive = true;
    }

    private static void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: ObservableRecipient observableRecipient })
            observableRecipient.IsActive = false;
    }

    public void Dispose()
    {
        if (_view == null)
            return;

        _view.Loaded -= OnLoaded;
        _view.Unloaded -= OnUnloaded;
    }
}