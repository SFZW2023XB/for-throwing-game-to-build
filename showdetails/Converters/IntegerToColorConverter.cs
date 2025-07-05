using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace showdetails.Converters
{
    public class IntegerToColorConverter : IValueConverter
    {
        public static IntegerToColorConverter Instance { get; } = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return count > 0 ? new SolidColorBrush(Color.Parse("#4CAF50")) : new SolidColorBrush(Color.Parse("#F44336"));
            }
            return new SolidColorBrush(Color.Parse("#F44336"));
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}