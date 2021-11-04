using System;

using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UML_Diagram_Designer.Models
{
    public class AddRelationShipButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
            {
                return SystemColors.ControlBrush;
            }

            return (bool)value
                ? Brushes.Blue 
                : SystemColors.ControlBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
