using System.Globalization;
using System.Windows;
using System.Windows.Data;
using App.Wpf.Common.Converters.Base;

namespace App.Wpf.Common.Converters;

public class BoolToVisibilityConverter : MarkupConverterBase
{
    public Visibility TrueValue { get; set; } = Visibility.Visible;
    public Visibility FalseValue { get; set; } = Visibility.Collapsed;
    public Visibility NullValue { get; set; } = Visibility.Collapsed;

    protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return NullValue;

        if (value is not bool valueBool)
            return Binding.DoNothing;

        return valueBool ? TrueValue : FalseValue;
    }

    protected override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (Equals(value, null))
            return null;

        if (Equals(value, TrueValue))
            return true;

        if (Equals(value, FalseValue))
            return false;

        return Binding.DoNothing;
    }
}