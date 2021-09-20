using DiagramDesigner.Controller;
using DiagramDesigner.Model;
using MetaDslx.GraphViz;

namespace UML_Diagram_Designer.Controller
{
    public class ApplicationController
    {
        private DiagramController _diagramController;

        

        public void CreateDiagram()
        {
        }
        public void OpenDiagram(string filename)
        {

            DiagramControllerHelper diagramControllerHelper = new DiagramControllerHelper(filename);
            this._diagramController = new DiagramController(diagramControllerHelper.Connector);

            this._diagramController.Open();
        }
        public void SaveDiagram()
        {

        }
        public void InteractDiagram()
        {

        }
    }
}
