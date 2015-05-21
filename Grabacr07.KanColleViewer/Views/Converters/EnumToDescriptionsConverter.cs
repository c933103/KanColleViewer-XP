using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Grabacr07.KanColleViewer.Views.Converters
{
    public class EnumToDescriptionsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if("Flags".Equals(parameter)) {
                return ConvertFlag(value);
            }
            return ConvertSingle(value);
        }

        private string ConvertSingle(object value)
        {
            if(value is Enum) {
                Type t = value.GetType();
                var member = t.GetMember(((Enum)value).ToString());
                if(member != null && member.Length > 0) {
                    var attr = member[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if(attr != null && attr.Length > 0) {
                        return ((DescriptionAttribute)attr[0]).Description;
                    }
                }
            }
            return value?.ToString();
        }

        private string[] ConvertFlag(object value)
        {
            List<string> r = new List<string>();
            if(value is Enum) {
                Type t = value.GetType();
                Enum val = (Enum)value;
                foreach(var bit in Enum.GetValues(t)) {
                    if(((int)bit != 0 && val.HasFlag((Enum)bit)) || ((int)bit == 0 && System.Convert.ToInt32(val) == 0)) {
                        var member = t.GetMember(bit.ToString());
                        if(member != null && member.Length > 0) {
                            var attr = member[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                            if(attr != null && attr.Length > 0) {
                                r.Add(((DescriptionAttribute)attr[0]).Description);
                            } else {
                                r.Add(bit.ToString());
                            }
                        } else {
                            r.Add(bit.ToString());
                        }
                    }
                }
            }
            if(r.Count != 0) return r.ToArray();
            return new string[] { value.ToString() };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
