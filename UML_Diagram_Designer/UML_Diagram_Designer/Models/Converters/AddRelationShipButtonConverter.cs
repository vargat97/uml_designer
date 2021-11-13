using System;

using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UML_Diagram_Designer.Models
{
    /// <summary>
    /// Converts value into a brush, depends on the value of the "value"
    /// </summary>
    public class AddRelationShipButtonConverter : IValueConverter
    {
     
        /// <summary>
        /// Converts value into a Brush.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
