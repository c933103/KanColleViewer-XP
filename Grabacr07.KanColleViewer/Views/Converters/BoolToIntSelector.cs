using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Grabacr07.KanColleViewer.Views.Converters
{
    public class BoolToIntSelector : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value is bool?) {
                if(((bool?)value).HasValue) value = ((bool?)value).Value;
            }
            if(value is bool) {
                var selector = parameter.ToString().Split(':').Select(s => {
                    int o;
                    if(int.TryParse(s, out o)) return o;
                    return 0;
                }).ToArray();
                if((bool)value) return selector[0];
                if(selector.Length > 1) return selector[1];
                return 0;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
