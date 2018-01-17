using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Final.QuickNote.Class.Converter
{
    public class NullStringConvert : IValueConverter
    {
        public object Convert(object value, System.Type targetType,
       object parameter, System.Globalization.CultureInfo culture)
        {
            string str = value as string;

            if (string.IsNullOrEmpty(str))
            {
                return "(Untitled)";
            }

            return str;
        }

        public object ConvertBack(object value, System.Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
