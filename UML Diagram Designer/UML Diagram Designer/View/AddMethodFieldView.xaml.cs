using DiagramDesigner.Model;
using MetaDslx.Languages.Uml.Model;
using MetaDslx.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiagramDesigner.View
{
    /// <summary>
    /// Interaction logic for AddMethodFieldView.xaml
    /// </summary>
    public partial class AddMethodFieldView : Window
    {
        private ImmutableObject methodObject;
        private object senderView;
        public List<ImmutableObject> Types { get; set; }
        public string MethodName 
        { get; set; }
        public string NewParamaterName { get; set; }

        public static readonly DependencyProperty MethodParametersProperty = DependencyProperty.Register("MethodParameter", typeof(List<ImmutableObject>), typeof(AddMethodFieldView), new PropertyMetadata(MethodParameterChanged));

        public List<ImmutableObject> MethodParameters
        {
            get { return (List<ImmutableObject>)GetValue(MethodParametersProperty); }
            set { SetValue(MethodParametersProperty, value); }
        }

        private static void MethodParameterChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            AddMethodFieldView mfv = sender as AddMethodFieldView;
            if (mfv == null) return;
            mfv.OnItemsSourceChanged(e);
        }
        private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                this.AvailableParametersListBox.DataContext = (List<ImmutableObject>)e.NewValue;
            }
        }

        public AddMethodFieldView(List<ImmutableObject> types, object senderView, ImmutableObject methodObject = null)
        {
            this.senderView = senderView;
            this.Types = types;
            this.MethodName = "";
            InitializeComponent();
            if (methodObject != null){
                this.methodObject = methodObject;
                this.MethodName = this.methodObject.MName;

                this.MethodParameters = new List<ImmutableObject>();

                if(methodObject.MChildren != null)
                {
                    foreach(var child in methodObject.MChildren)
                    {
                        this.MethodParameters.Add(child);
                    }
                }
                ImmutableObject returnType = null;

                foreach(var param in ((Operation)methodObject).OwnedParameter)
                {
                    if (param.Direction == ParameterDirectionKind.Return)
                    {
                        returnType = param;
                        break;
                    }
                }
                if(returnType != null)
                {
                    foreach(var t in Types)
                    {
                        if (t.MName.Equals(returnType.MType.MName))
                        {
                            this.ReturnTypeBox.SelectedItem = t;
                        }
                    }
                }

            }
            else { this.MethodParameters = new List<ImmutableObject>(); }
           
            DataContext = this;
            this.AvailableParametersListBox.DataContext = MethodParameters;
            this.AvailableTypes.SelectedIndex = 0;

        }

        private void AddChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.methodObject == null)
            {
                this.Close();
                return;
            }
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            var controller = mainWindow.GetController();

           controller.ModifyObjectName(controller.GetUMLModel().Model.GetObject(methodObject), MethodName);
           var view = (DetailsView)this.senderView;

            if(methodObject.MParent == null) 
                controller.SetOperationToClass(((MyNodeBindingHelper)view.DataContext).GetImmutableObject(), methodObject);

            if(this.ReturnTypeBox.SelectedItem != null)
            {
                var p =controller.CreateParameter("");
                controller.SetObjectType(controller.GetUMLModel().Model.GetObject(p), controller.GetUMLModel().Model.GetObject((ImmutableObject)ReturnTypeBox.SelectedItem));
                controller.SetParameterToOperation(controller.GetUMLModel().Model.GetObject(methodObject), controller.GetUMLModel().Model.GetObject(p), ParameterDirectionKind.Return);
            }

            view.BindingHelper = new MyNodeBindingHelper(controller.GetUMLModel().Model.GetObject(controller.GetUMLModel().Model.GetObject(methodObject).MParent));
            mainWindow.DiagramView.GraphLayout = null;
            mainWindow.DiagramView.GraphLayout = controller.GetGraphLayoutModel().GraphLayout;
            this.Close();
        }

        private void AddParameterButton_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            var controller = mainWindow.GetController();

            var createdParameter = controller.CreateParameter(this.NewParamaterName);
            if(this.methodObject == null)
            {
                var operation = controller.CreateOperation(MethodName);
                this.methodObject = controller.GetUMLModel().Model.GetObject(operation);
            }

            controller.SetObjectType(controller.GetUMLModel().Model.GetObject(createdParameter), (ImmutableObject)AvailableTypes.SelectedItem);
            this.MethodParameters.Add(controller.GetUMLModel().Model.GetObject(createdParameter));
            controller.SetParameterToOperation(methodObject, createdParameter, MetaDslx.Languages.Uml.Model.ParameterDirectionKind.In);
            this.MethodParameters = new List<ImmutableObject>(MethodParameters);
            this.newParameterTextBox.Clear();
        }

        private void RemoveParameter_Click(object sender, RoutedEventArgs e)
        {

            if(AvailableParametersListBox.SelectedItem != null)
            {
                var param = (ImmutableObject)AvailableParametersListBox.SelectedItem;

                    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                    var controller = mainWindow.GetController();

                    controller.RemoveObject(controller.GetUMLModel().Model.GetObject(param));
                    var view = (DetailsView)this.senderView;
                    var immutableObject = view.BindingHelper.GetImmutableObject();
                
                view.BindingHelper = new MyNodeBindingHelper(mainWindow.GetController().GetUMLModel().Model.GetObject(immutableObject.ToMutable()));
                mainWindow.DiagramView.GraphLayout = null;
                mainWindow.DiagramView.GraphLayout = controller.GetGraphLayoutModel().GraphLayout;
                this.MethodParameters.Remove(param);
                this.MethodParameters = new List<ImmutableObject>(MethodParameters);
            }
        }

        private void RemoveMethod_Click(object sender, RoutedEventArgs e)
        {
            if(this.methodObject != null)
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                var controller = mainWindow.GetController();

                foreach(var child in methodObject.MChildren)
                {
                    controller.RemoveObject(child);
                }
                controller.RemoveObject(this.methodObject);
                var view = (DetailsView)this.senderView;
                var immutableObject = view.BindingHelper.GetImmutableObject();

                view.BindingHelper = new MyNodeBindingHelper(mainWindow.GetController().GetUMLModel().Model.GetObject(immutableObject.ToMutable()));
                mainWindow.DiagramView.GraphLayout = null;
                mainWindow.DiagramView.GraphLayout = controller.GetGraphLayoutModel().GraphLayout;
                this.Close();
            }
        }
    }
}
