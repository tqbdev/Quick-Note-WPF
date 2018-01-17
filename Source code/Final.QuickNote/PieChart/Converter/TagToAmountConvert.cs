using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Final.QuickNote.PieChart.Converter
{
    public class TagToAmountConvert : IValueConverter
    {
        public object Convert(object value, System.Type targetType,
       object parameter, System.Globalization.CultureInfo culture)
        {
            Tag tag = value as Tag;

            if (tag != null)
            {
                return tag.Amount.ToString();
            }

            return null;
        }

        public object ConvertBack(object value, System.Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
