using System;
using System.ComponentModel;
using System.Windows.Data;

namespace LynLogger.Views
{
    public class EnumToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
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
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
