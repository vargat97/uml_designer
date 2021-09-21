using MetaDslx.GraphViz;
using UML_Diagram_Designer.Model;

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

            this.connector = new Connector(model, graphLayout);

        }

    }
}
