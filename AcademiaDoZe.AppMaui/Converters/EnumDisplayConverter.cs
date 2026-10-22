using AcademiaDoZe.Application.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AcademiaDoZe.AppMaui.Converters
{
    public sealed class EnumDisplayConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is Enum e ? e.GetDisplayName() : string.Empty;
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
