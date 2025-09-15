namespace App.Wpf.Common.Dispatcher;

public interface IDispatcher
{
    void InvokeInUi(Action action);
    Task InvokeInUiAsync(Action action);
}