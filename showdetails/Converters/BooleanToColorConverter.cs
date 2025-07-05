using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace showdetails.Converters;

public class BooleanToColorConverter : IValueConverter
{
    public static readonly BooleanToColorConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isOn)
        {
            return isOn ? Brushes.Green : Brushes.Gray;
        }
        return Brushes.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}