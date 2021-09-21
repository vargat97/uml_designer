namespace UML_Diagram_Designer.Controller
{
    public class ApplicationController
    {
        private DiagramController _diagramController;
        private DiagramControllerHelper helper;
        

        public void CreateDiagram()
        {
            this.helper = new DiagramControllerHelper(null);
            this._diagramController = new DiagramController(this.helper.Connector);
            this._diagramController.Create();
        }
        public void OpenDiagram(string filename)
        {
            this.helper = new DiagramControllerHelper(filename);
            this._diagramController = new DiagramController(this.helper.Connector);
            this._diagramController.Open();
        }
        public void SaveDiagram()
        {

        }
        public void InteractDiagram()
        {

        }
        public Connector GetConnector()
        {
            return this._diagramController.Connector;
        }
    }
}
