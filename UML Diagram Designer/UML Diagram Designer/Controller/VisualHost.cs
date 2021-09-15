using DiagramDesigner.Model;
using DiagramDesigner.View;
using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using MetaDslx.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DiagramDesigner.Controller
{
    public class VisualHost: FrameworkElement
    {
        private readonly DiagramView view;
        private DrawingVisual drawingVisual;
        private readonly Dictionary<NodeLayout, FrameworkElement> nodeContents;
        private double zoom = 1;
        public GraphLayout Layout{ get; set; }
        public VisualHost(DiagramView view)
        {
            this.view = view;
            this.nodeContents = new Dictionary<NodeLayout, FrameworkElement>();
        }

        public NodeLayout GetNodeLayoutFromNodeContents(Point2D point)
        {
            foreach(KeyValuePair<NodeLayout,FrameworkElement> entry in nodeContents)
            {
                double dx = Math.Abs(point.X - entry.Key.Position.X);
                double dy = Math.Abs(point.Y - entry.Key.Position.Y);
                if(dx < entry.Key.Width / 2 && dy < entry.Key.Height / 2)
                {

                }
            }
            var node = this.nodeContents.FirstOrDefault(n => ((Math.Abs(point.X - n.Key.Position.X) < n.Key.Width/2) && (Math.Abs(point.Y - n.Key.Position.Y) < n.Key.Height/2))).Key;
            return node;
        }

        private void NodeContentMouseRightButtonDown(object sender, MouseEventArgs e)
        {
            var content = sender as FrameworkElement;
            var node = this.nodeContents.FirstOrDefault(n => n.Value == content).Key;
            var nodeObject = (ImmutableObject)node.NodeObject;
           // var myNodeBindingHelper = new MyNodeBindingHelper(nodeObject);
            EditView editView = new EditView(nodeObject);
            var p = e.GetPosition(this);
            editView.Show();


            //var content = sender as FrameworkElement;
            //var node = this.nodeContents.FirstOrDefault(n => n.Value == content).Key;
            //var nodeObject = (ImmutableObject)node.NodeObject;

            //MainWindow.Controller.ModifyObjectName(MainWindow.Controller.GetUMLModel().Model.GetObject(nodeObject), "ASD");
            //MessageBoxResult result = MessageBox.Show(MainWindow.Controller.GetUMLModel().Model.GetObject(nodeObject).MName);

            //this.nodeContents.TryGetValue(node, out var frameworkElement);
            //MyNodeBindingHelper helper = new MyNodeBindingHelper(MainWindow.Controller.GetUMLModel().Model.GetObject(nodeObject));
            //frameworkElement.DataContext = helper;

            //var content = sender as FrameworkElement;
            //var node = this.nodeContents.FirstOrDefault(n => n.Value == content).Key;
            //var nodeObject = (ImmutableObject)node.NodeObject;
            //ImmutableObject operation = MainWindow.Controller.CreateOperation("FOSSSSS");
            //MainWindow.Controller.SetOperationToClass(nodeObject, operation);

            //this.nodeContents.TryGetValue(node, out var frameworkElement);
            //MyNodeBindingHelper helper = new MyNodeBindingHelper(MainWindow.Controller.GetUMLModel().Model.GetObject(nodeObject));
            //frameworkElement.DataContext = helper;
        }

        public double Zoom
        {
            set
            {
                this.zoom = value;
                InvalidateMeasure();
            }
            get
            {
                return this.zoom;
            }
        }
        internal void ZoomTo(Size size)
        {
            Size gs = GraphSize;
            double scaleY = size.Height / gs.Height;
            double scaleX = size.Width / gs.Width;
            Zoom = Math.Min(scaleX, scaleY);
        }
        private Size GraphSize
        {
            get
            {
                if (Layout == null || drawingVisual == null || Layout.Size.X == 0 || Layout.Size.Y == 0) return new Size(8, 8);
                return new Size(Layout.Size.X + 2, Layout.Size.Y + 2);
            }
        }
        internal void BindGraphLayout(GraphLayout layout)
        {

            if (layout == null) return;
            
            if(drawingVisual != null)
            {
                this.view.HostCanvasChildrenClear();
                RemoveVisualChild(drawingVisual);
         
            }
            foreach (var content in this.nodeContents.Values)
            {
                content.MouseRightButtonDown -= NodeContentMouseRightButtonDown;
            }
            this.nodeContents.Clear();

            Layout = layout;
            this.drawingVisual = new DrawingVisual();
            foreach(var node in Layout.Nodes)
            {
                MyNodeBindingHelper helper = new MyNodeBindingHelper((ImmutableObject)node.NodeObject);

                FrameworkElement nodeContent = helper.ChooseDataTemplateFromModel().LoadContent() as FrameworkElement;
                this.nodeContents.Add(node, nodeContent);
                nodeContent.DataContext = helper;
                if (!node.IsSubGraph)
                {
                   nodeContent.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                   Rect r = new Rect(nodeContent.DesiredSize);
                   nodeContent.Arrange(r);
                }
                node.PreferredSize = new Point2D(nodeContent.DesiredSize.Width, nodeContent.DesiredSize.Height);
            }
            
            Layout.ComputeLayout();
            foreach (var node in Layout.AllNodes)
            {
                this.DrawNode(node);
            }

            foreach (var edge in Layout.AllEdges)
            {
                this.DrawEdge(edge);
            }

            AddVisualChild(this.drawingVisual);
        }    
        private void DrawNode(NodeLayout node)
        {
            
            if(this.nodeContents.TryGetValue(node,out var nodeContent))
            {
                
                if (node.IsSubGraph)
                {
                        nodeContent.Arrange(new Rect(node.Left, node.Top, node.Width, node.Height));
                        nodeContent.Width = node.Width;
                        nodeContent.Height = node.Height;

                }
                nodeContent.SetValue(FrameworkElement.TagProperty, node);

                this.view._hostCanvas.Children.Add(nodeContent);
                Canvas.SetLeft(nodeContent, node.Left);
                Canvas.SetTop(nodeContent, node.Top);
                nodeContent.MouseRightButtonDown += NodeContentMouseRightButtonDown;
            }
            else
            {
                var myVisual = new DrawingVisual();
                DrawingContext drawingContext = myVisual.RenderOpen();

                Rect rect = new Rect(new Point(node.Position.X - node.Size.X / 2, node.Position.Y - node.Size.Y / 2), new Size(node.Size.X, node.Size.Y));
                drawingContext.DrawRectangle(Brushes.Black, node.IsSubGraph ? new Pen() : null, rect);
                
                drawingContext.Close();
                myVisual.SetValue(FrameworkElement.TagProperty, node);
                this.drawingVisual.Children.Add(myVisual);
            }

            if (node.IsSubGraph)
            {
                foreach (var childNode in node.Nodes)
                {
                    this.DrawNode(childNode);
                }
            }
        }

        private void DrawEdge(EdgeLayout edge)
        {
            var edgeBindingHelper = new MyEdgeBingindHelper((ImmutableObject)edge.EdgeObject);
            var visual = new DrawingVisual();
            DrawingContext drawingContext = visual.RenderOpen();
            var path = new PathGeometry();
            foreach (var spline in edge.Splines)
            {
                    var pathFigure = new PathFigure();
                    pathFigure.IsClosed = false;
                    pathFigure.StartPoint = new Point(spline[0].X, spline[0].Y);
                    for (int i = 1; i < spline.Length; i += 3)
                    {
                        var segment = new BezierSegment(new Point(spline[i].X, spline[i].Y), new Point(spline[i + 1].X, spline[i + 1].Y), new Point(spline[i + 2].X, spline[i + 2].Y), true);
                        pathFigure.Segments.Add(segment);
                    }
                    path.Figures.Add(pathFigure);
            }


            System.Windows.Shapes.Path p = new Path();
            p.Stroke = edgeBindingHelper.Pen.Brush;
            p.Data = path;
            this.view._hostCanvas.Children.Add(p);



            drawingContext.DrawGeometry(null, edgeBindingHelper.Pen, path);
            drawingContext.Close();
            visual.SetValue(FrameworkElement.TagProperty, edge);
            this.drawingVisual.Children.Add(visual);
        }
    }
}
