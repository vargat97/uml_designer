using MetaDslx.Modeling;
using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace UML_Diagram_Designer.Models
{
    public class AttributeVisibilityConverter : IMultiValueConverter
    {
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

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class OperationVisibilityConverter : IMultiValueConverter
    {
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
                sb_parameters.Append(op_params[i].MName);
                sb_parameters.Append(": ");
                sb_parameters.Append(op_params[i].MType.MName);
                if (op_params.Count > 1 && i < op_params.Count - 1)
                    sb_parameters.Append(",");

            }


            return visibility + values[1] + "(" + sb_parameters.ToString() + "): "  + return_type;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class EnumVisibilityConverter : IMultiValueConverter
    {
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
