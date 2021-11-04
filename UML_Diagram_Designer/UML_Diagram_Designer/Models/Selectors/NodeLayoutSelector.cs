using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UML_Diagram_Designer.Models
{
    public class NodeLayoutSelector: DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (element != null && item != null && item is NodeLayout)
            {
                var itemType = item.GetType();
                if (itemType.Name == "ClassLayout")
                    return
                        element.FindResource("ClassNodeTemplate") as DataTemplate;
                else if (itemType.Name == "InterfaceLayout")
                    return
                        element.FindResource("InterfaceNodeTemplate") as DataTemplate;
                else
                    return
                       element.FindResource("EnumNodeTemplate") as DataTemplate;

            }

            return null;
        }
    }
}
