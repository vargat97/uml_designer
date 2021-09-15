using MetaDslx.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DiagramDesigner.Model
{
    public class MyEdgeBingindHelper
    {
        private readonly ImmutableObject edge;
        public MyEdgeBingindHelper(ImmutableObject edge)
        {
            this.edge = edge;
            this.definePenFromEdgeType();
        }


        public Pen Pen { get; set; }
        
        private void definePenFromEdgeType()
        {
            var metaClassName = edge.MMetaClass.MName;
            if (metaClassName.Equals("InterfaceRealization"))
            {
                Pen = new Pen() { Brush = Brushes.Red, Thickness = 2.0 };
                Pen.DashStyle = DashStyles.Dash;
            }
            else if (metaClassName.Equals("Generalization"))
            {
                Pen = new Pen() { Brush = Brushes.Black, Thickness = 2.0 };
            }
            else if (metaClassName.Equals("Dependency"))
            {
                Pen = new Pen() { Brush = Brushes.Blue, Thickness = 2.0 };
            }
            else if (metaClassName.Equals("Association"))
            {
                Pen = new Pen() { Brush = Brushes.Green, Thickness = 2.0 };
            }
        }
    }
}
