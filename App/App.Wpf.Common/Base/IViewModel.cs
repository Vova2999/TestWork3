using System.Windows;

namespace App.Wpf.Common.Base;

public interface IViewModel : IDisposable
{
    FrameworkElement View { get; }
}