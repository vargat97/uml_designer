using System;
using System.Collections.Generic;
using System.Text;

namespace UML_Diagram_Designer.Models.EdgeLayouts
{
    public class AssociationEdgeLayout: EdgeLayout
    {

        public AssociationEdgeLayout(GraphLayout graph, NodeLayout source, NodeLayout target, object edgeObject) : base(graph, source, target, edgeObject) { }

    }
}
