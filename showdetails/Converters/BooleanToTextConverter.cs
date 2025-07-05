using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace showdetails.Converters
{
    public class BooleanToTextConverter : IValueConverter
    {
        public static BooleanToTextConverter Instance { get; } = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string options)
            {
                var parts = options.Split('|');
                return boolValue ? parts[0] : (parts.Length > 1 ? parts[1] : string.Empty);
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}