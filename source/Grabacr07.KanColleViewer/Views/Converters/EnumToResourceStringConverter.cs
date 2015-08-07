using Grabacr07.KanColleViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Grabacr07.KanColleViewer.Views.Converters
{
    public class EnumToResourceStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var key = parameter?.ToString() + value?.ToString();
            var description = ResourceService.Current.Resources.GetType().GetProperty(key, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)?.GetValue(ResourceService.Current.Resources, null)?.ToString();
            return description ?? key ?? value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
