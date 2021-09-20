using DiagramDesigner.Model;
using MetaDslx.GraphViz;
using System;
using System.Collections.Generic;
using System.Text;

namespace UML_Diagram_Designer.Controller
{
    public class DiagramControllerHelper
    {
        private Connector connector;

        public Connector Connector
        {
            get { return this.connector; }
        }
        public DiagramControllerHelper(string filename = null)
        {
            UMLModel model = new UMLModel(filename);
            GraphLayout graphLayout = new MetaDslx.GraphViz.GraphLayout("dot");

            Connector connector = new Connector(model, graphLayout);
            this.connector = connector;
        }

    }
}
