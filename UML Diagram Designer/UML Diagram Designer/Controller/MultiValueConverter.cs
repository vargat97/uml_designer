using DiagramDesigner.Model;
using MetaDslx.Languages.Uml.Model;
using MetaDslx.Modeling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DiagramDesigner.Controller
{
    public class MultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, System.Type targetType, object parameter, CultureInfo culture)
        {
            var sBuilder = new StringBuilder();
            sBuilder.AppendLine((string)values[0]);

            var properties = (List<ImmutableObject>)values[1];
            var operations= (List<ImmutableObject>)values[2];
            var enumKinds = (List<ImmutableObject>)values[3];
            //Properties, if there any
            if(properties.Count > 0)
            {
                foreach (var obj in properties)
                {
                    var namedElement = (NamedElement)obj;
                    sBuilder.AppendLine(namedElement.MName + ":" + namedElement.MType.MName);
                }
            }

            //Operations, if there any
            if (operations.Count > 0)
            {
                foreach (var obj in operations)
                {
                    var namedElement = (NamedElement)obj;
                   sBuilder.AppendLine(namedElement.MName);
                }
            }


            if (enumKinds.Count > 0)
            { 
                foreach (var obj in enumKinds)
                {
                    var namedElement = (NamedElement)obj;
                    sBuilder.AppendLine(namedElement.MName);
                }


            }

            return sBuilder.ToString();
         }

        public object[] ConvertBack(object value, System.Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
