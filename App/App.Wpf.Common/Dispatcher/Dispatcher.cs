using System.Windows;

namespace App.Wpf.Common.Dispatcher;

public class Dispatcher : IDispatcher
{
    public void InvokeInUi(Action action)
    {
        Application.Current.Dispatcher.Invoke(action);
    }

    public Task InvokeInUiAsync(Action action)
    {
        return Application.Current.Dispatcher.InvokeAsync(action).Task;
    }
}