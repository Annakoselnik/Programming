using System;
using System.Globalization;
using System.Windows.Data;

namespace View.Converters
{
    /// <summary>Конвертер, инвертирующий bool значение. true -> false, false -> true.</summary>
    public class InverseBoolConverter : IValueConverter
    {
        /// <summary>Прямое преобразование: возвращает противоположное значение.</summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            !(value is bool b && b);

        /// <summary>Обратное преобразование: симметрично прямому.</summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            !(value is bool b && b);
    }
}