using DiagramDesigner.Controller;
using MetaDslx.GraphViz;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using System.Windows.Input;
using System.Windows.Shapes;
using MetaDslx.Modeling;
using MetaDslx.Languages.Uml.Model;

namespace DiagramDesigner.View
{

    /// <summary>
    /// Interaction logic for DiagramView.xaml
    /// </summary>
    public partial class DiagramView : UserControl
    {
        private readonly VisualHost host;
        //public static readonly DependencyProperty EdgeTemplateProperty = DependencyProperty.Register("EdgeTemplate", typeof(EdgeTemplate), typeof(DiagramView), new UIPropertyMetadata(null));
        public static readonly DependencyProperty NodeTemplateCollectionProperty = DependencyProperty.Register("NodeTemplateCollection", typeof(ObservableCollection<DataTemplate>), typeof(DiagramView), new UIPropertyMetadata(null));
        public static readonly DependencyProperty GraphLayoutProperty = DependencyProperty.Register("GraphLayout", typeof(GraphLayout), typeof(DiagramView), new PropertyMetadata(GraphLayoutChanged));
        public ObservableCollection<DataTemplate> NodeTemplateCollection
        {
            get { return (ObservableCollection<DataTemplate>)GetValue(NodeTemplateCollectionProperty); }
            set { SetValue(NodeTemplateCollectionProperty, value); }
        }
        //public EdgeTemplate EdgeTemplate
        //{
        //    get { return (EdgeTemplate)GetValue(EdgeTemplateProperty); }
        //    set { SetValue(EdgeTemplateProperty, value); }
        //}
        public GraphLayout GraphLayout
        {
            get { return (GraphLayout)GetValue(GraphLayoutProperty); }
            set { SetValue(GraphLayoutProperty, value); }
        }
        public Line Line { get; set; }
        private Point drawLineStartPoint;
        private bool inDrawinngPocess = false;
        private ImmutableObject startObjectToEdge;
        public DiagramView()
        {
            NodeTemplateCollection = new ObservableCollection<DataTemplate>();
            this.AddDataTemplatesToNodeTemplateProperty();
            InitializeComponent();
            this.Line = new Line();
            //PreviewMouseWheel += DiagramView_PreviewMouseWheel;
            //Loaded += DiagramView_Loaded;
            host = new VisualHost(this);
            _hostCanvas.Children.Add(host);

        }
        public VisualHost GetVisualHost()
        {
            return this.host;
        }

        //private void DiagramView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    this.host.ZoomTo(RenderSize);
        //    UpdateLayout();
        //    _scrollViewer.ScrollToHorizontalOffset((this.host.RenderSize.Width - _scrollViewer.ViewportWidth) / 2);
        //    _scrollViewer.ScrollToVerticalOffset((this.host.RenderSize.Height - _scrollViewer.ViewportHeight) / 2);
        //}

        //private void DiagramView_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        //{
        //    e.Handled = true;
        //    // Scales and scrolls so, that the visual center keeps in the center
        //    double zoom = this.host.Zoom;
        //    double centerX = _scrollViewer.ViewportWidth / 2;
        //    double centerY = _scrollViewer.ViewportHeight / 2;
        //    double offsetX = (_scrollViewer.HorizontalOffset + centerX) / this.host.Zoom;
        //    double offsetY = (_scrollViewer.VerticalOffset + centerY) / this.host.Zoom;

        //    // zoom the content of the graph element
        //    zoom = zoom * (1 + e.Delta / 1200.0);
        //    this.host.Zoom = zoom;

        //    // Wait until the ScrollViewer has updated its layout because of the Graph's size changings
        //    UpdateLayout();

        //    _scrollViewer.ScrollToHorizontalOffset(offsetX * zoom - centerX);
        //    _scrollViewer.ScrollToVerticalOffset(offsetY * zoom - centerY);
        //}

        private static void GraphLayoutChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DiagramView dv = sender as DiagramView;
            if (dv == null) return;
            dv.OnItemsSourceChanged(e);
        }

        private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                this.host.BindGraphLayout((GraphLayout)e.NewValue);
            }
        }
        private void AddDataTemplatesToNodeTemplateProperty()
        {
            NodeTemplateCollection.Add((DataTemplate)Application.Current.Resources["ClassNodeTemplate"]);
            NodeTemplateCollection.Add((DataTemplate)Application.Current.Resources["InterfaceNodeTemplate"]);
            NodeTemplateCollection.Add((DataTemplate)Application.Current.Resources["ClassNodeTemplate"]);
        }

        public void HostCanvasChildrenClear()
        {
            _hostCanvas.Children.Clear();
        }

        private void _hostCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            this.inDrawinngPocess = true;
            this.drawLineStartPoint = e.GetPosition(_hostCanvas);
            this.startObjectToEdge = (ImmutableObject)host.GetNodeLayoutFromNodeContents(new Point2D(drawLineStartPoint.X, drawLineStartPoint.Y)).NodeObject;
        }

        private void _hostCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.inDrawinngPocess)
            {
                _hostCanvas.Children.Remove(Line);
                Line.X1 = this.drawLineStartPoint.X;
                Line.Y1 = this.drawLineStartPoint.Y;

                Line.X2 = e.GetPosition(_hostCanvas).X;
                Line.Y2 = e.GetPosition(_hostCanvas).Y;
                Line.Stroke = Brushes.Pink;


                _hostCanvas.Children.Add(Line);
            }
        }

        private void _hostCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if (this.inDrawinngPocess)
            {
                Point2D p = new Point2D(e.GetPosition(_hostCanvas).X, e.GetPosition(_hostCanvas).Y);
                var node = host.GetNodeLayoutFromNodeContents(p);
                if (node != null)
                {
                    this.CreateEdge((ImmutableObject)node.NodeObject);
                }
            }

           
            _hostCanvas.Children.Remove(Line);
            this.inDrawinngPocess = false;
        }


        private void CreateEdge(ImmutableObject targetObject)
        {
            if (targetObject.Equals(this.startObjectToEdge)) return;
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            var button = mainWindow.DetailsView.EdgeButton;
            if(button != null)
            {
                var content = (string)button.Content;
                var controller = mainWindow.GetController();
                
                if (content.Equals("Realization"))
                {
                    // XOR
                    if (!(targetObject.MMetaClass.MName != "Interface") != (startObjectToEdge.MMetaClass.MName != "Interface")) return;

                    var interfaceObject = (targetObject.MMetaClass.MName == "Interface") ? targetObject : this.startObjectToEdge;
                    var classifierObject = interfaceObject.MName == targetObject.MName ? this.startObjectToEdge : targetObject;

                    if (classifierObject.MMetaClass.MName == "Enumeration") return;
                    //If his general's have an interfacerealization??
                    var classImplOfClassifier = (Classifier)classifierObject;

                    if(classImplOfClassifier.Generalization.Count != 0)
                    {
                        var general = classImplOfClassifier.Generalization.FirstOrDefault().General;
                        var allInterfaceRealizations = general.AllRealizedInterfaces();
                        foreach (var realization in allInterfaceRealizations)
                        {
                            if (realization.MName == interfaceObject.MName)
                                return;
                        }
                    }
                    if(classImplOfClassifier.AllRealizedInterfaces().Count != 0)
                    {
                        foreach (var realization in classImplOfClassifier.AllRealizedInterfaces())
                        {
                            if (realization.MName == interfaceObject.MName)
                                return;
                        }
                    }

                    var interfaceRealization = controller.CreateInterfaceRealization();
                    var immutableModel = controller.GetUMLModel().Model;
                    controller.SetInterfaceRealization(immutableModel.GetObject(interfaceObject), immutableModel.GetObject(classifierObject), immutableModel.GetObject(interfaceRealization));
                }
                if (content.Equals("Dependency"))
                {

                    
                }

                if (content.Equals("Generalization"))
                {
                    var generalObject = targetObject;
                    var specificObject = this.startObjectToEdge;

                    if (generalObject.MMetaClass.Name != "Class" && specificObject.MMetaClass.MName != "Class") return;

                    var specificClassifierObject = (Classifier)specificObject;
                    if (specificClassifierObject.Generalization.Count > 0) return;

                    var generalizationObject = controller.CreateGeneralization();
                    var immutableModel = controller.GetUMLModel().Model;
                    controller.SetGeneralization(immutableModel.GetObject(targetObject), immutableModel.GetObject(this.startObjectToEdge), immutableModel.GetObject(generalizationObject));
                }

                if (content.Equals("Composition"))
                {

                }
                if (content.Equals("Aggregation"))
                {

                }
                if(content.Equals("Directed Associoation"))
                {

                }
                if (content.Equals("Association"))
                {

                }

                GraphLayout = null;

                GraphLayout = controller.GetGraphLayoutModel().GraphLayout;
            }
                
        }
    }
}
