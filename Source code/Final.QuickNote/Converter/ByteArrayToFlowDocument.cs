using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;

namespace Final.QuickNote.Class.Converter
{
    public class ByteArrayToFlowDocument : IValueConverter
    {
        public object Convert(object value, System.Type targetType,
       object parameter, System.Globalization.CultureInfo culture)
        {
            var flowDocument = new FlowDocument();

            if (value != null)
            {
                var arrayByte = value as byte[];
                MemoryStream memoryStream = new MemoryStream(arrayByte);

                try
                {
                    TextRange textRange = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                    textRange.Load(memoryStream, System.Windows.DataFormats.XamlPackage);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Cannot convert byte[] to FlowDocument\r\nMessage Throw: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    flowDocument.Blocks.Clear();
                    return flowDocument;
                }
            }

            return flowDocument;
        }

        public object ConvertBack(object value, System.Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
