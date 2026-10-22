using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AcademiaDoZe.AppMaui.Converters
{
    public class DateOnlyToDateTimeConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateOnly dateOnly)
                return dateOnly.ToDateTime(TimeOnly.MinValue);
            return DateTime.Today;
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
                return DateOnly.FromDateTime(dateTime);
            return DateOnly.FromDateTime(DateTime.Today);
        }
    }
}
