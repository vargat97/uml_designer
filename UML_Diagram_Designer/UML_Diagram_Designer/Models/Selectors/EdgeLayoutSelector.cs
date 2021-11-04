using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UML_Diagram_Designer.Models
{
    public class EdgeLayoutSelector: DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (element != null && item != null && item is EdgeLayout)
            {
                var itemType = item.GetType();
                if (itemType.Name == "InterfaceRealizationEdgeLayout")
                    return
                        element.FindResource("InterfaceRealizationEdgeTemplate") as DataTemplate;
                else if (itemType.Name == "GeneralizationEdgeLayout")
                {
                    return
                        element.FindResource("GeneralizationEdgeTemplate") as DataTemplate;
                }
                else if (itemType.Name == "DependencyEdgeLayout")
                {
                    return
                        element.FindResource("DependencyEdgeTemplate") as DataTemplate;
                }
                else if (itemType.Name == "AssociationEdgeLayout")
                {
                    return
                        element.FindResource("AssociationEdgeTemplate") as DataTemplate;
                }
                else
                            return
                       element.FindResource("InterfaceRealizationEdgeTemplate") as DataTemplate;

            }

            return null;
        }
    
    }
}
