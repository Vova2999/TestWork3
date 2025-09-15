using App.Wpf.Common.Base;
using App.Wpf.Common.Dispatcher;
using App.Wpf.Common.View;

namespace App.Extensions;

public static class ViewModelExtensions
{
    /// <summary>
    ///     Открытие диалога. Использовать только внутри IDispatcher
    /// </summary>
    /// <param name="viewModel"></param>
    /// <returns></returns>
    public static bool? OpenDialog(this IViewModel viewModel)
    {
        var viewService = Locator.Current.Locate<IViewService>();
#pragma warning disable CS0618
        return viewService.OpenDialog(viewModel);
#pragma warning restore CS0618
    }

    /// <summary>
    ///     Открытие диалога. Использовать при отсутствии необходимости результата
    /// </summary>
    /// <param name="viewModel"></param>
    public static void OpenDialogInUi(this IViewModel viewModel)
    {
        var dispatcherHelper = Locator.Current.Locate<IDispatcher>();
        dispatcherHelper.InvokeInUi(() => viewModel.OpenDialog());
    }

    /// <summary>
    ///     Открытие диалога. Использовать при необходимости результата
    /// </summary>
    /// <param name="viewModel"></param>
    /// <returns></returns>
    public static Task<bool?> OpenDialogInUiAsync(this IViewModel viewModel)
    {
        var dispatcherHelper = Locator.Current.Locate<IDispatcher>();
        var tcs = new TaskCompletionSource<bool?>();
        dispatcherHelper.InvokeInUi(
            () =>
            {
                var res = viewModel.OpenDialog();
                tcs.SetResult(res);
            });

        return tcs.Task;
    }
}