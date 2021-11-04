using MetaDslx.Languages.Uml.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace UML_Diagram_Designer.Models
{
    public class InterfaceRealizationEdgeLayout: EdgeLayout
    {
        public InterfaceRealizationEdgeLayout(GraphLayout graph, NodeLayout source, NodeLayout target, object edgeObject) : base(graph, source, target, edgeObject) {

        }

        protected override bool IsClosedTriangle()
        {
            return true;
        }
    }
}
