using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace View.Converters
{
    /// <summary>Конвертер: true → Visible, false → Collapsed.</summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>Прямое преобразование: true возвращает Visible, false — Collapsed.</summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (value is bool b && b) ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>Обратное преобразование: Visible → true, любой другой статус → false.</summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is Visibility v && v == Visibility.Visible;
    }
}