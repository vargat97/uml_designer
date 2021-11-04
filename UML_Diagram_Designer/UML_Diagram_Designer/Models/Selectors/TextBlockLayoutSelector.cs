using MetaDslx.Modeling;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UML_Diagram_Designer.Models
{
    public class TextBlockLayoutSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (element != null && item != null && item is ImmutableObject)
            {
                var itemType = item.GetType();
                if (itemType.Name == "PropertyImpl")
                    return
                        element.FindResource("PropertyImplTemplate") as DataTemplate;
                else if (itemType.Name == "OperationImpl")
                    return
                        element.FindResource("OperationImplTemplate") as DataTemplate;
                else
                    return
                       element.FindResource("EnumNodeTemplate") as DataTemplate;

            }

            return null;
        }

    }
}
