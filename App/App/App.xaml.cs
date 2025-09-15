using System.Windows;
using App.Views.Main.Logic;

namespace App;

public partial class App
{
    private readonly IMainWindowProvider _mainWindowProvider;

    public App(IMainWindowProvider mainWindowProvider)
    {
        _mainWindowProvider = mainWindowProvider;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _mainWindowProvider.Show();
    }
}