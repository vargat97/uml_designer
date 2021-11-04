using MetaDslx.Languages.Uml.Model;
using MetaDslx.Languages.Uml.Serialization;
using MetaDslx.Modeling;
using Microsoft.CodeAnalysis;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using UML_Diagram_Designer.Models;
using UML_Diagram_Designer.Models.EdgeLayouts;

namespace UML_Diagram_Designer.ViewModels
{
    public class DiagramViewModel : Screen, IHandle<ImmutableModelChangedEvent>
    {
        private NodeLayout _edgestart;
        private Functions _functions;
        private DiagramEditorViewModel _diagramEditorViewModel;
        private ImmutableModel _immutableModel;
        private GraphLayout _graphLayout;
        private readonly double _scaleStep = 0.1;
        private double _scale = 1;
        private Point _canvasSize;
        public Line CreateEdgeLine
        {
            get;
            set;
        }
        public Point CanvasSize
        {
            get { return this._canvasSize; }
            set
            {
                SetAndNotify(ref this._canvasSize, value);
            }
        }
        private ImmutableModel ImmutableModel
        {
            get { return this._immutableModel; }
            set
            {
                this._immutableModel = value;
                var graphLayout = LoadLayoutFromModel();
                GraphLayout = graphLayout;
                this.RefreshCanvasSize();
            }
        }

        public double Scale
        {
            get { return this._scale; }
            set
            {
                SetAndNotify(ref this._scale, value);
            }
        }

        public GraphLayout GraphLayout
        {
            get { return this._graphLayout; }
            set
            {
                SetAndNotify(ref this._graphLayout, value);
            }
        }

        public DiagramViewModel(ImmutableModel immutableModel,DiagramEditorViewModel diagramEditorViewModel,IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);
            this._diagramEditorViewModel = diagramEditorViewModel;
            this._immutableModel = immutableModel;
            GraphLayout = this.LoadLayoutFromModel();
            Scale = 1;
            CreateEdgeLine = new Line();
        }
        public void CanvasSizeChanged(Panel sender, SizeChangedEventArgs e)
        {
            var nodes = (ItemsControl)sender.Children[0];
            double max_width = 0;
            double max_height = 0;
            foreach (NodeLayout itemsource in nodes.ItemsSource)
            {


                if (itemsource.Position.X > max_width) max_width = itemsource.Position.X + itemsource.Size.X;
                if (itemsource.Position.Y > max_height) max_height = itemsource.Position.Y + itemsource.Size.Y;

            }
            CanvasSize = new Point(max_width, max_height);
        }
        public void SelectNode(object sender, MouseButtonEventArgs e)
        {
            var relativeTo = (FrameworkElement)sender;
            var datacontext = relativeTo.DataContext;
            NodeLayout layout = (NodeLayout)datacontext;
            this._edgestart = layout;
            if (_diagramEditorViewModel.IsCreateRealtionShip)
            {
                var p = layout.Position;
                this.DrawCreateEdgeLine(new Point(p.X,p.Y));
                return;
            }

            this._diagramEditorViewModel.EdgeLayout = null;
            layout.Selected = true;
            object layoutInstance = Convert.ChangeType(layout, datacontext.GetType());


            this._diagramEditorViewModel.NodeLayout = (NodeLayout)layoutInstance;
        }
        public void MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!this._diagramEditorViewModel.IsCreateRealtionShip) return;

            var my_sender = (FrameworkElement)sender;
            var datacontext = my_sender.DataContext;
            NodeLayout layout = (NodeLayout)datacontext;

            this.CreateNewEdge((ImmutableObject)layout.NodeObject);

            CreateEdgeLine.X1 = CreateEdgeLine.X2 = 0;
            CreateEdgeLine.Y1 = CreateEdgeLine.Y2 = 0;

            this._diagramEditorViewModel.IsCreateRealtionShip = false;
        }


        public void CreateEdgeMouseMove(object sender, MouseEventArgs e)
        {
            if (CreateEdgeLine.X1 == 0 && CreateEdgeLine.Y1 == 0) return;

            var relativeTo = (FrameworkElement)sender;
            var p = e.GetPosition(relativeTo);

            CreateEdgeLine.X2 = p.X;
            CreateEdgeLine.Y2 = p.Y;

        }

        public void SelectEdge(object sender, MouseButtonEventArgs e)
        {
            this._diagramEditorViewModel.NodeLayout = null;
            var datacontext = (((Path)sender).DataContext);
            var layout = (EdgeLayout)datacontext;
            object layoutInstance = Convert.ChangeType(layout, datacontext.GetType());    

            this._diagramEditorViewModel.EdgeLayout = (EdgeLayout)layoutInstance;
        }

        public void TextBlockClick(object sender, MouseButtonEventArgs e)
        {
            this._diagramEditorViewModel.NodeLayout = null;
            var datacontext = (((TextBlock)sender).DataContext);
            this._diagramEditorViewModel.DetailsObject = (Property)datacontext;
        }
        public void SelectOperation(object sender, MouseButtonEventArgs e)
        {
            this._diagramEditorViewModel.NodeLayout = null;
            var datacontext = (((TextBlock)sender).DataContext);
            this._diagramEditorViewModel.DetailsObject = (Operation)datacontext;
        }
        public void Handle(ImmutableModelChangedEvent message)
        {
            ImmutableModel = message.ImmutableModel;
            
        }
        public void ZoomOut()
        {
            if (!(Scale - _scaleStep > 0)) return;
            Scale -= _scaleStep;
        }
        public void ZoomIn()
        {
            Scale += _scaleStep;
        }
        public void ZoomGrid(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) Scale += _scaleStep;
            else { Scale -= _scaleStep; }
        }

        private void DrawCreateEdgeLine(Point point)
        {
            CreateEdgeLine.X1 = CreateEdgeLine.X2 = point.X;
            CreateEdgeLine.Y1 = CreateEdgeLine.Y2 = point.Y;
        }
        private void CreateNewEdge(ImmutableObject supplier)
        {
            var client = (ImmutableObject)this._edgestart.NodeObject;
            var edgeObject = this._diagramEditorViewModel.RelationShipObject;

            this._functions = new Functions(this._immutableModel);
            var my_model = this._functions.CreateRelationship(client, supplier, edgeObject);
            this.ImmutableModel = my_model;
        }
        private void RefreshCanvasSize()
        {
            double max_width = 0;
            double max_height = 0;
            foreach (var nodelayout in GraphLayout.AllNodes)
            {
                if (nodelayout.Position.X > max_width) max_width = nodelayout.Position.X + nodelayout.Size.X;
                if (nodelayout.Position.Y > max_height) max_height = nodelayout.Position.Y + nodelayout.Size.Y;
            }
            CanvasSize = new Point(max_width, max_height);
        }
        private GraphLayout LoadLayoutFromModel()
        {
            var graphLayout = new GraphLayout("dot");

            this.AddClasses(_immutableModel.Objects.OfType<Class>(), graphLayout);
            this.AddInterfaces(_immutableModel.Objects.OfType<Interface>(), graphLayout);
            this.AddEnumerations(_immutableModel.Objects.OfType<Enumeration>(), graphLayout);

            this.AddInterfaceRealizations(_immutableModel.Objects.OfType<InterfaceRealization>(), graphLayout);
            this.AddGeneralizations(_immutableModel.Objects.OfType<Generalization>(), graphLayout);
            this.AddDependecies(_immutableModel.Objects.OfType<Dependency>(), graphLayout);
            this.AddAssociations(_immutableModel.Objects.OfType<Association>(), graphLayout);

            graphLayout.NodeSeparation = 30;
            graphLayout.RankSeparation = 50;
            graphLayout.EdgeLength = 30;
            graphLayout.NodeMargin = 20;
            graphLayout.ComputeLayout();

            return graphLayout;
        }
        /*
        
        private void LoadUmlLayoutFromModel()
        {
            this.AddClasses(_immutableModel.Objects.OfType<Class>());
            this.AddInterfaces(_immutableModel.Objects.OfType<Interface>());
            this.AddEnumerations(_immutableModel.Objects.OfType<Enumeration>());

            this.AddInterfaceRealizations(_immutableModel.Objects.OfType<InterfaceRealization>());
            this.AddGeneralizations(_immutableModel.Objects.OfType<Generalization>());
            this.AddDependecies(_immutableModel.Objects.OfType<Dependency>());
            this.AddAssociations(_immutableModel.Objects.OfType<Association>());
        }
        
        
        
        private void LoadUmlLayoutFromModel(GraphLayout graphLayout)
        {
            foreach (var cls in _immutableModel.Objects.OfType<Class>())
            {
                if (cls.MMetaClass.Name == "Class")
                {
                    var node = new ClassLayout(graphLayout, cls);

                    graphLayout.AddNode(node);
                }

            }
            foreach (var interf in _immutableModel.Objects.OfType<Interface>())
            {
                if (interf.MMetaClass.Name == "Interface")
                {
                    var node = new InterfaceLayout(graphLayout, interf);

                    graphLayout.AddNode(node);
                }

            }

            foreach (var enu in _immutableModel.Objects.OfType<Enumeration>())
            {
                var node = new EnumLayout(graphLayout, enu);
                graphLayout.AddNode(node);
            }

            foreach (var gen in _immutableModel.Objects.OfType<Generalization>())
            {

                var allnodes = graphLayout.AllNodes.ToList();
                NodeLayout specNL = null;
                NodeLayout genNL = null;
                foreach (var n in allnodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, gen.Specific.Name)) specNL = graphLayout.FindNodeLayout(namedNode);
                    else if (string.Equals(namedNode.Name, gen.General.Name)) genNL = graphLayout.FindNodeLayout(namedNode);

                }
                if (specNL != null && genNL != null)
                {
                    var g = new EdgeLayout(graphLayout, specNL, genNL, gen);
                    graphLayout.AddEdge(specNL.NodeObject, genNL.NodeObject, g);

                }
            }
            foreach (var intrf in _immutableModel.Objects.OfType<InterfaceRealization>())
            {
                var allNodes = graphLayout.AllNodes.ToList();
                NodeLayout clientNL = null;
                NodeLayout supplierNL = null;
                foreach (var n in allNodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, intrf.Client.FirstOrDefault().Name))
                    {
                        clientNL = graphLayout.FindNodeLayout(namedNode);
                    }
                    else if (string.Equals(namedNode.Name, intrf.Supplier.FirstOrDefault().Name)) supplierNL = graphLayout.FindNodeLayout(namedNode);
                }

                if (clientNL != null && supplierNL != null)
                {
                    // "--|>"
                    var g = new EdgeLayout(graphLayout, clientNL, supplierNL, intrf);

                    graphLayout.AddEdge(clientNL.NodeObject, supplierNL.NodeObject, g);
                }
            }

        }
       
        */
        private void AddClasses(IEnumerable<Class> classes,GraphLayout graphLayout)
        {

            foreach (var cls in classes)
            {
                if (cls.MMetaClass.Name == "Class" )
                {
                    var node = new ClassLayout(graphLayout, cls);

                    graphLayout.AddNode(node);
                }

            }
        }
        
        private void AddInterfaces(IEnumerable<Interface> interfaces, GraphLayout graphLayout)
        {
            foreach (var interf in interfaces)
            {
                if (interf.MMetaClass.Name == "Interface")
                {
                    var node = new InterfaceLayout(graphLayout, interf);

                    graphLayout.AddNode(node);
                }

            }
        }

        private void AddEnumerations(IEnumerable<Enumeration> enums, GraphLayout graphLayout)
        {
            foreach (var enu in enums)
            {
                var node = new EnumLayout(graphLayout, enu);
                graphLayout.AddNode(node);
            }
        }
        
        private void AddGeneralizations(IEnumerable<Generalization> generalizations, GraphLayout graphLayout)
        {
            foreach (var gen in generalizations)
            {

                var allnodes = graphLayout.AllNodes.ToList();
                NodeLayout specNL = null;
                NodeLayout genNL = null;
                foreach (var n in allnodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, gen.Specific.Name)) specNL = graphLayout.FindNodeLayout(namedNode);
                    else if (string.Equals(namedNode.Name, gen.General.Name)) genNL = graphLayout.FindNodeLayout(namedNode);

                }
                if (specNL != null && genNL != null)
                {
                    var g = new GeneralizationEdgeLayout(graphLayout, specNL, genNL, gen);
                    graphLayout.AddEdge(specNL.NodeObject, genNL.NodeObject, g);

                }
            }

        }
        
        private void AddInterfaceRealizations(IEnumerable<InterfaceRealization> interfaceRealizations, GraphLayout graphLayout)
        {
            foreach (var intrf in interfaceRealizations)
            {
                var allNodes = graphLayout.AllNodes.ToList();
                NodeLayout clientNL = null;
                NodeLayout supplierNL = null;
                foreach (var n in allNodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, intrf.Client.FirstOrDefault().Name))
                    {
                        clientNL = graphLayout.FindNodeLayout(namedNode);
                    }
                    else if (string.Equals(namedNode.Name, intrf.Supplier.FirstOrDefault().Name)) supplierNL = graphLayout.FindNodeLayout(namedNode);
                }

                if (clientNL != null && supplierNL != null)
                {
                    // "--|>"
                    var g = new InterfaceRealizationEdgeLayout(graphLayout, clientNL, supplierNL, intrf);

                    graphLayout.AddEdge(clientNL.NodeObject, supplierNL.NodeObject, g);
                }
            }

        }
        
        private void AddDependecies(IEnumerable<Dependency> dependencies, GraphLayout graphLayout)
        {
            foreach (var dep in dependencies)
            {
                var allNodes = graphLayout.AllNodes.ToList();

                NodeLayout clientNL = null;
                NodeLayout supplierNL = null;

                foreach (var n in allNodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, dep.Client.FirstOrDefault().Name)) clientNL = graphLayout.FindNodeLayout(namedNode);
                    else if (string.Equals(namedNode.Name, dep.Supplier.FirstOrDefault().Name)) supplierNL = graphLayout.FindNodeLayout(namedNode);
                }

                if (clientNL != null && supplierNL != null)
                {
                    var d = new DependencyEdgeLayout(graphLayout, clientNL, supplierNL, dep);

                    graphLayout.AddEdge(clientNL.NodeObject, supplierNL.NodeObject, d);
                }
            }
        }

        private void AddAssociations(IEnumerable<Association> associations, GraphLayout graphLayout)
        {

            foreach (var aso in associations)
            {
                if (aso.MemberEnd.Count == 0) return;
                var allNodes = graphLayout.AllNodes.ToList();
                NodeLayout memberEndNL = null;
                NodeLayout memberEnd1NL = null;

                foreach (var n in allNodes)
                {
                    var namedNode = (MetaDslx.Languages.Uml.Model.NamedElement)n.NodeObject;
                    if (string.Equals(namedNode.Name, aso.MemberEnd[0].Type.Name)) 
                        memberEndNL = graphLayout.FindNodeLayout(namedNode);
                    else if (string.Equals(namedNode.Name, aso.MemberEnd[1].Type.Name)) memberEnd1NL = graphLayout.FindNodeLayout(namedNode);
                }

                if (memberEndNL != null && memberEnd1NL != null)
                {
                    var a = new AssociationEdgeLayout(graphLayout, memberEndNL, memberEnd1NL, aso);

                    graphLayout.AddEdge(memberEndNL.NodeObject, memberEnd1NL.NodeObject, a);

                }


            }
        }

        
        
    }
}
