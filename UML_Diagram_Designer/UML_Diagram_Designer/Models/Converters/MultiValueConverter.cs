using MetaDslx.Modeling;
using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace UML_Diagram_Designer.Models
{
    /// <summary>
    /// Converter for the attributes in the nodelayout
    /// </summary>
    public class AttributeVisibilityConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts the values into a string in the following format:
        /// +|-|~|# visivility, depends on the object's visibility
        /// name of the attribute
        /// ":" and type of the attribute
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return null;

            var visibility = "";
            var value = values[0].ToString();
            switch (value)
            {
                case "Public":
                    visibility = "+";
                    break;
                case "Private":
                    visibility = "-";
                    break;
                case "Protected":
                    visibility = "~";
                    break;
                case "Package":
                    visibility = "#";
                    break;
                default:
                    visibility = "+";
                    break;
            }

            return visibility + values[1] + ": " + values[2];
        }

        /// <summary>
        /// Not implemented yet
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Converter for the operations in the nodelayout
    /// </summary>
    public class OperationVisibilityConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts values into string in the following format:
        /// +|-|~|# visibility, depends on the operation's visibility
        /// Name of the operation
        /// Parameter names and types of the operation
        /// ":" and the return type of the operation
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return null;

            var visibility = "";
            var value = values[0].ToString();
            switch (value)
            {
                case "Public":
                    visibility = "+";
                    break;
                case "Private":
                    visibility = "-";
                    break;
                case "Protected":
                    visibility = "~";
                    break;
                case "Package":
                    visibility = "#";
                    break;
                default:
                    visibility = "+";
                    break;
            }

            //Create a string which contains the parameter names and types of the operation
            StringBuilder sb_parameters = new StringBuilder(50);
            var op_params = (ImmutableModelList<MetaDslx.Languages.Uml.Model.Parameter>)values[2];
            var return_type = "";
            for(int i = 0; i < op_params.Count; i++)
            {
                if (op_params[i].Direction == MetaDslx.Languages.Uml.Model.ParameterDirectionKind.Return)
                {
                    return_type = op_params[i].MType.MName;
                    break;
                }
                if (op_params[i].MType != null)
                {
                    sb_parameters.Append(op_params[i].MName);
                    sb_parameters.Append(": ");
                    sb_parameters.Append(op_params[i].MType.MName);
                }
                if (op_params.Count > 1 && i < op_params.Count - 1)
                    sb_parameters.Append(",");

            }


            return visibility + values[1] + "(" + sb_parameters.ToString() + "): "  + return_type;
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Converter for the enumerationliterals in the nodelayout
    /// </summary>
    public class EnumVisibilityConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts the values into a string in the following format:
        /// +|-|~|# visivility, depends on the object's visibility
        /// Name of the enumrationliteral
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return null;

            var visibility = "";
            var value = values[0].ToString();
            switch (value)
            {
                case "Public":
                    visibility = "+";
                    break;
                case "Private":
                    visibility = "-";
                    break;
                case "Protected":
                    visibility = "~";
                    break;
                case "Package":
                    visibility = "#";
                    break;
                default:
                    visibility = "+";
                    break;
            }

            return visibility + values[1];
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
