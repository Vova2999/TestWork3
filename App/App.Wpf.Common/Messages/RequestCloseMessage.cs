using App.Wpf.Common.Base;

namespace App.Wpf.Common.Messages;

public class RequestCloseMessage
{
    public IViewModel ViewModel { get; }
    public bool? DialogResult { get; }

    public RequestCloseMessage(IViewModel viewModel, bool? dialogResult = null)
    {
        ViewModel = viewModel;
        DialogResult = dialogResult;
    }
}