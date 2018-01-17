using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Final.QuickNote.Class.Converter
{
    public class ColorTagToBrush : IValueConverter
    {
        public object Convert(object value, System.Type targetType,
                                object parameter, System.Globalization.CultureInfo culture)
        {
            ColorTag colorTag = (ColorTag)value;
            return new SolidColorBrush(NoteWindow.colorsBackground[(int)colorTag]);
        }

        public object ConvertBack(object value, System.Type targetType,
                                    object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
