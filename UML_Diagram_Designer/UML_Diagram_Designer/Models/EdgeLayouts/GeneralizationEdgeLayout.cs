using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UML_Diagram_Designer.Models.EdgeLayouts
{
    public class GeneralizationEdgeLayout: EdgeLayout
    {

        public GeneralizationEdgeLayout(GraphLayout graph, NodeLayout source, NodeLayout target, object edgeObject) : base(graph, source, target, edgeObject)
        { 

        }

        protected override bool IsClosedTriangle()
        {
            return true;
        }

        private double CalculateRotation(Point p1, Point p2)
        {
            double lenght = Math.Sqrt(Math.Pow((p1.X - p2.X), 2) + Math.Pow((p1.Y - p2.Y), 2));
            double m = Math.Abs(p1.X - p2.X);

            return Math.Asin(m / lenght);

        }
    }
}
