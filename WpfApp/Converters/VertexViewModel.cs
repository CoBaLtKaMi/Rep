using System;
using System.Globalization;
using System.Windows.Data;
using GraphLibrary.Models;

namespace WpfApp.Converters
{
    public class VertexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Vertex vertex)
            {
                return $"{vertex.Label}-{vertex.Id}";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}