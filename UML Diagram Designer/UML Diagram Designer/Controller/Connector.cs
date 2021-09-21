using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using System.Collections.Generic;
using System.Linq;
using UML_Diagram_Designer.Model;

namespace UML_Diagram_Designer.Controller
{
    /// <summary>
    /// Represents the connection between the UMLModel and the _graphLayout
    /// </summary>
    public class Connector
    {
        private readonly UMLModel _umlModel;
        private readonly GraphLayout _graphLayout;
        private readonly UmlFactory _factory;

        public UMLModel UMLModel
        {
            get { return this._umlModel; }
        }

        public GraphLayout GraphLayout
        {
            get { return this._graphLayout; }
        }

        public Connector(UMLModel model, GraphLayout graphLayout)
        {
            this._graphLayout = graphLayout;
            this._umlModel = model;

            this._factory = new UmlFactory(this._umlModel.Model.ToMutable());
        }
        public void ConnectUMLModelAndGraphLayout()
        {
            this.LoadGraphLayoutFromModel();
            this.ComputeGraphLayout();
        }
        private void LoadGraphLayoutFromModel()
        {
            this.AddClasses(_umlModel.Model.Objects.OfType<Class>());
            this.AddInterfaces(_umlModel.Model.Objects.OfType<Interface>());
            this.AddEnumerations(_umlModel.Model.Objects.OfType<Enumeration>());
            this.AddInterfaceRealizations(_umlModel.Model.Objects.OfType<InterfaceRealization>());
            this.AddGeneralizations(_umlModel.Model.Objects.OfType<Generalization>());
            this.AddDependecies(_umlModel.Model.Objects.OfType<Dependency>());
            this.AddAssociations(_umlModel.Model.Objects.OfType<Association>());
        }
        private void ComputeGraphLayout()
        {

            _graphLayout.NodeSeparation = 10;
            _graphLayout.RankSeparation = 50;
            _graphLayout.EdgeLength = 30;
            _graphLayout.NodeMargin = 20;

            _graphLayout.ComputeLayout();
        }
        private void AddClasses(IEnumerable<Class> classes)
        {

            foreach (var cls in classes)
            {
                if (cls.MMetaClass.Name == "Class" /* && cls.MParent.MName == "Logical View"*/)
                {
                    var n = _graphLayout.AddNode(cls);
                }

            }
        }
        private void AddInterfaces(IEnumerable<Interface> interfaces)
        {
            foreach (var interf in interfaces)
            {
                if (interf.MMetaClass.Name == "Interface")
                {
                    var n = _graphLayout.AddNode(interf);
                }

            }
        }

        private void AddEnumerations(IEnumerable<Enumeration> enums)
        {
            foreach (var enu in enums)
            {
                var n = _graphLayout.AddNode(enu);
            }
        }
        private void AddUseCases(IEnumerable<UseCase> useCases)
        {
            foreach (var uc in useCases)
            {

                _graphLayout.AddNode(uc);

            }
        }
        private void AddActors(IEnumerable<Actor> actors)
        {
            foreach (var ac in actors)
            {
                _graphLayout.AddNode(ac);
            }
        }
        private void AddGeneralizations(IEnumerable<Generalization> generalizations)
        {
            foreach (var gen in generalizations)
            {

                var allnodes = _graphLayout.AllNodes.ToList();
                NodeLayout specNL = null;
                NodeLayout genNL = null;
                foreach (var n in allnodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, gen.Specific.Name)) specNL = _graphLayout.FindNodeLayout(namedNode);
                    else if (string.Equals(namedNode.Name, gen.General.Name)) genNL = _graphLayout.FindNodeLayout(namedNode);

                }
                //if (specObject == null) { Layout.AddNode(gen.Specific.Name); specObject = Layout.FindNodeLayout(gen.Specific.Name); }
                //if (genObject == null) { Layout.AddNode(gen.General.Name); genObject = Layout.FindNodeLayout(gen.General.Name); }
                if (specNL != null && genNL != null)
                {
                    var e = _graphLayout.AddEdge(specNL.NodeObject, genNL.NodeObject, gen);

                }
            }

        }
        private void AddInterfaceRealizations(IEnumerable<InterfaceRealization> interfaceRealizations)
        {
            foreach (var intrf in interfaceRealizations)
            {
                var allNodes = _graphLayout.AllNodes.ToList();
                _graphLayout.FindNodeLayout(intrf.Supplier.FirstOrDefault().Name);
                NodeLayout clientNL = null;
                NodeLayout supplierNL = null;
                foreach (var n in allNodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, intrf.Client.FirstOrDefault().Name))
                    {
                        clientNL = _graphLayout.FindNodeLayout(namedNode);
                    }
                    else if (string.Equals(namedNode.Name, intrf.Supplier.FirstOrDefault().Name)) supplierNL = _graphLayout.FindNodeLayout(namedNode);
                }

                if (clientNL != null && supplierNL != null)
                {
                    // "--|>"

                    var e = _graphLayout.AddEdge(clientNL.NodeObject, supplierNL.NodeObject, intrf);
                }
            }

        }
        private void AddDependecies(IEnumerable<Dependency> dependencies)
        {
            foreach (var dep in dependencies)
            {
                var allNodes = _graphLayout.AllNodes.ToList();

                NodeLayout clientNL = null;
                NodeLayout supplierNL = null;

                foreach (var n in allNodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, dep.Client.FirstOrDefault().Name)) clientNL = _graphLayout.FindNodeLayout(namedNode);
                    else if (string.Equals(namedNode.Name, dep.Supplier.FirstOrDefault().Name)) supplierNL = _graphLayout.FindNodeLayout(namedNode);
                }

                if (clientNL != null && supplierNL != null)
                {
                    var e = _graphLayout.AddEdge(clientNL.NodeObject, supplierNL.NodeObject, dep);
                }
            }
        }
        private void AddAssociations(IEnumerable<Association> associations)
        {

            foreach (var aso in associations)
            {

                var allNodes = _graphLayout.AllNodes.ToList();
                NodeLayout memberEndNL = null;
                NodeLayout memberEnd1NL = null;

                foreach (var n in allNodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, aso.MemberEnd[0].Type.Name)) memberEndNL = _graphLayout.FindNodeLayout(namedNode);
                    else if (string.Equals(namedNode.Name, aso.MemberEnd[1].Type.Name)) memberEnd1NL = _graphLayout.FindNodeLayout(namedNode);
                }

                if (memberEndNL != null && memberEnd1NL != null)
                {

                    var e = _graphLayout.AddEdge(memberEndNL.NodeObject, memberEnd1NL.NodeObject, aso);

                }


            }
        }
        private void AddIncludes(IEnumerable<Include> includes)
        {
            foreach (var inc in includes)
            {
                var allNodes = _graphLayout.AllNodes.ToList();

                NodeLayout includingNL = null;
                NodeLayout additionNL = null;

                foreach (var n in allNodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, inc.IncludingCase.Name)) includingNL = _graphLayout.FindNodeLayout(namedNode);
                    else if (string.Equals(namedNode.Name, inc.Addition.Name)) additionNL = _graphLayout.FindNodeLayout(namedNode);
                }

                if (includingNL != null && additionNL != null)
                {
                    var e = _graphLayout.AddEdge(additionNL.NodeObject, includingNL.NodeObject, additionNL.NodeObject.ToString() + "--|>" + includingNL.NodeObject.ToString());
                }
            }

        }
        private void AddExtends(IEnumerable<Extend> extends)
        {
            foreach (var ext in extends)
            {
                var allNodes = _graphLayout.AllNodes.ToList();

                NodeLayout extendCaseNL = null;
                NodeLayout extensionNL = null;

                foreach (var n in allNodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, ext.ExtendedCase.Name)) extendCaseNL = _graphLayout.FindNodeLayout(namedNode);
                    else if (string.Equals(namedNode.Name, ext.Extension.Name)) extensionNL = _graphLayout.FindNodeLayout(namedNode);
                }

                if (extendCaseNL != null && extensionNL != null)
                {
                    var e = _graphLayout.AddEdge(extensionNL.NodeObject, extendCaseNL.NodeObject, extensionNL.NodeObject.ToString() + "--|>" + extendCaseNL.NodeObject.ToString());
                }
            }
        }

    }
}
