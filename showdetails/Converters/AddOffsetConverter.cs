using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace showdetails.Converters
{
    public class AddOffsetConverter : IValueConverter
    {
        public static AddOffsetConverter Instance { get; } = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double number && parameter is string offset && double.TryParse(offset, out double offsetValue))
            {
                return number + offsetValue;
            }
            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}