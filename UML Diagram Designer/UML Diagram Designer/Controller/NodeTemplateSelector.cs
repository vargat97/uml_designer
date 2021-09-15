using DiagramDesigner.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DiagramDesigner.Controller
{
    public class NodeTemplateSelector: DataTemplateSelector
    {
        public override DataTemplate  SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (element != null && item != null && item is MyNodeBindingHelper)
            {
                MyNodeBindingHelper node = item as MyNodeBindingHelper;
                if (node.NodeType == MyNodeBindingHelper.NodeTypeEnums.Class)
                    return
                        element.FindResource("ClassNodeTemplate") as DataTemplate;
                else if(node.NodeType == MyNodeBindingHelper.NodeTypeEnums.Interface)
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
