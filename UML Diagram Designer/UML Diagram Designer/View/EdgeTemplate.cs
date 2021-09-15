using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DiagramDesigner.View
{
    public class EdgeTemplate: DependencyObject
    {
        // Static variable that must be initialized at run time.
        static readonly Pen DefaultPen = new Pen() { Brush = Brushes.Black, Thickness = 1.0 };

        // Static constructor is called at most one time, before any
        // instance constructor is invoked or member is accessed.
        static EdgeTemplate()
        {
            
        }

        //Register the dependencyproperty into WPF property system.
        public static readonly DependencyProperty PenProperty = DependencyProperty.Register("Pen", typeof(Pen), typeof(EdgeTemplate), new PropertyMetadata(DefaultPen));
        
        //Wrapper implementation for the PenProperty DependencyProperty
        public Pen Pen
        {
            get { return (Pen)GetValue(PenProperty); }
            set { SetValue(PenProperty, value); }
        }

        //Register the dependencyproperty into WPF property system.
        public static readonly DependencyProperty EdgeTypeProperty = DependencyProperty.Register("EdgeType", typeof(object), typeof(EdgeTemplate), new PropertyMetadata(null));
        
        //Wrapper implemetation for the EdgeTypeProperty dependency property
        public object EdgeType
        {
            get { return (object)GetValue(EdgeTypeProperty); }
            set { SetValue(EdgeTypeProperty, value); }
        }
    }
}
