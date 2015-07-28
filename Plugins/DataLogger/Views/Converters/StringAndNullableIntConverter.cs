using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LynLogger.Views.Converters
{
    class StringAndNullableIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value.ToString())) return null;
            int val;
            if(int.TryParse(value.ToString(), out val)) {
                return val;
            } else {
                return new System.Windows.Controls.ValidationResult(false, "Not a number");
            }
        }
    }
}
