using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace showdetails.Converters
{
    public class ObjectConverters : IValueConverter
    {
        public static readonly ObjectConverters IsNotNull = new ObjectConverters();

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}