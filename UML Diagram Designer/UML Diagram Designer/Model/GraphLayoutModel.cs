using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.Model
{
    public class GraphLayoutModel
    {
        public GraphLayout GraphLayout { get; set; }
        private  UMLModel model;
        public GraphLayoutModel(GraphLayout _layout,UMLModel model)
        {
            this.GraphLayout = _layout;
            this.model = model;
        }

        public void LoadGraphLayoutModel()
        {
            this.AddClasses(model.Model.Objects.OfType<Class>());
            this.AddInterfaces(model.Model.Objects.OfType<Interface>());
            this.AddEnumerations(model.Model.Objects.OfType<Enumeration>());
            this.AddInterfaceRealizations(model.Model.Objects.OfType<InterfaceRealization>());
            this.AddGeneralizations(model.Model.Objects.OfType<Generalization>());
            this.AddDependecies(model.Model.Objects.OfType<Dependency>());
            this.AddAssociations(model.Model.Objects.OfType<Association>());

            GraphLayout.NodeSeparation = 10;
            GraphLayout.RankSeparation = 50;
            GraphLayout.EdgeLength = 30;
            GraphLayout.NodeMargin = 20;

            GraphLayout.ComputeLayout();
        }
        public void ReloadGraphLayoutModel(UMLModel model)
        {
            this.model = model;
            this.GraphLayout = new GraphLayout("dot");
            this.LoadGraphLayoutModel();
        }
        public void setUMLModel(UMLModel model)
        {
            this.model = model;
        }
        private void AddClasses(IEnumerable<Class> classes)
        {

            foreach (var cls in classes)
            {
                if (cls.MMetaClass.Name == "Class" /* && cls.MParent.MName == "Logical View"*/)
                {
                    var n = GraphLayout.AddNode(cls);
                }

            }
        }
        private void AddInterfaces(IEnumerable<Interface> interfaces)
        {
            foreach (var interf in interfaces)
            {
                if (interf.MMetaClass.Name == "Interface")
                {
                    var n = GraphLayout.AddNode(interf);
                }

            }
        }

        private void AddEnumerations(IEnumerable<Enumeration> enums)
        {
            foreach (var enu in enums)
            {
                var n = GraphLayout.AddNode(enu);
            }
        }
        private void AddUseCases(IEnumerable<UseCase> useCases)
        {
            foreach (var uc in useCases)
            {

                GraphLayout.AddNode(uc);

            }
        }
        private void AddActors(IEnumerable<Actor> actors)
        {
            foreach (var ac in actors)
            {
                GraphLayout.AddNode(ac);
            }
        }
        private void AddGeneralizations(IEnumerable<Generalization> generalizations)
        {
            foreach (var gen in generalizations)
            {

                var allnodes = GraphLayout.AllNodes.ToList();
                NodeLayout specNL = null;
                NodeLayout genNL = null;
                foreach (var n in allnodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, gen.Specific.Name)) specNL = GraphLayout.FindNodeLayout(namedNode);
                    else if (string.Equals(namedNode.Name, gen.General.Name)) genNL = GraphLayout.FindNodeLayout(namedNode);

                }
                //if (specObject == null) { Layout.AddNode(gen.Specific.Name); specObject = Layout.FindNodeLayout(gen.Specific.Name); }
                //if (genObject == null) { Layout.AddNode(gen.General.Name); genObject = Layout.FindNodeLayout(gen.General.Name); }
                if (specNL != null && genNL != null)
                {
                    var e = GraphLayout.AddEdge(specNL.NodeObject, genNL.NodeObject, gen);

                }
            }

        }

        private void AddInterfaceRealizations(IEnumerable<InterfaceRealization> interfaceRealizations)
        {
            foreach (var intrf in interfaceRealizations)
            {
                var allNodes = GraphLayout.AllNodes.ToList();
                GraphLayout.FindNodeLayout(intrf.Supplier.FirstOrDefault().Name);
                NodeLayout clientNL = null;
                NodeLayout supplierNL = null;
                foreach (var n in allNodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, intrf.Client.FirstOrDefault().Name))
                    {
                        clientNL = GraphLayout.FindNodeLayout(namedNode);
                    }  
                    else if (string.Equals(namedNode.Name, intrf.Supplier.FirstOrDefault().Name)) supplierNL = GraphLayout.FindNodeLayout(namedNode);
                }

                if (clientNL != null && supplierNL != null)
                {
                    // "--|>"
                    
                    var e = GraphLayout.AddEdge(clientNL.NodeObject, supplierNL.NodeObject,intrf);
                }
            }

        }

        private void AddDependecies(IEnumerable<Dependency> dependencies)
        {
            foreach (var dep in dependencies)
            {
                var allNodes = GraphLayout.AllNodes.ToList();

                NodeLayout clientNL = null;
                NodeLayout supplierNL = null;

                foreach (var n in allNodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, dep.Client.FirstOrDefault().Name)) clientNL = GraphLayout.FindNodeLayout(namedNode);
                    else if (string.Equals(namedNode.Name, dep.Supplier.FirstOrDefault().Name)) supplierNL = GraphLayout.FindNodeLayout(namedNode);
                }

                if (clientNL != null && supplierNL != null)
                {
                    var e = GraphLayout.AddEdge(clientNL.NodeObject, supplierNL.NodeObject,dep);
                }
            }
        }

        private void AddAssociations(IEnumerable<Association> associations)
        {

            foreach (var aso in associations)
            {

                var allNodes = GraphLayout.AllNodes.ToList();
                NodeLayout memberEndNL = null;
                NodeLayout memberEnd1NL = null;

                foreach (var n in allNodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, aso.MemberEnd[0].Type.Name)) memberEndNL = GraphLayout.FindNodeLayout(namedNode);
                    else if (string.Equals(namedNode.Name, aso.MemberEnd[1].Type.Name)) memberEnd1NL = GraphLayout.FindNodeLayout(namedNode);
                }

                if (memberEndNL != null && memberEnd1NL != null)
                {

                    var e = GraphLayout.AddEdge(memberEndNL.NodeObject, memberEnd1NL.NodeObject,aso);

                }


            }
        }

        private void AddIncludes(IEnumerable<Include> includes)
        {
            foreach (var inc in includes)
            {
                var allNodes = GraphLayout.AllNodes.ToList();

                NodeLayout includingNL = null;
                NodeLayout additionNL = null;

                foreach (var n in allNodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, inc.IncludingCase.Name)) includingNL = GraphLayout.FindNodeLayout(namedNode);
                    else if (string.Equals(namedNode.Name, inc.Addition.Name)) additionNL = GraphLayout.FindNodeLayout(namedNode);
                }

                if (includingNL != null && additionNL != null)
                {
                    var e = GraphLayout.AddEdge(additionNL.NodeObject, includingNL.NodeObject, additionNL.NodeObject.ToString() + "--|>" + includingNL.NodeObject.ToString());
                }
            }

        }

        private void AddExtends(IEnumerable<Extend> extends)
        {
            foreach (var ext in extends)
            {
                var allNodes = GraphLayout.AllNodes.ToList();

                NodeLayout extendCaseNL = null;
                NodeLayout extensionNL = null;

                foreach (var n in allNodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, ext.ExtendedCase.Name)) extendCaseNL = GraphLayout.FindNodeLayout(namedNode);
                    else if (string.Equals(namedNode.Name, ext.Extension.Name)) extensionNL = GraphLayout.FindNodeLayout(namedNode);
                }

                if (extendCaseNL != null && extensionNL != null)
                {
                    var e = GraphLayout.AddEdge(extensionNL.NodeObject, extendCaseNL.NodeObject, extensionNL.NodeObject.ToString() + "--|>" + extendCaseNL.NodeObject.ToString());
                }
            }
        }
    }

}

