using DiagramDesigner.Model;
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
    /// Interaction logic for AddEnumKindsView.xaml
    /// </summary>
    public partial class AddEnumKindsView : Window
    {
        public string EnumKindName { get; set; }
        private readonly object sender;
        private ImmutableObject enumKindObject;
        public AddEnumKindsView(object senderView,ImmutableObject enumKindObject = null)
        {
            this.sender = senderView;
            InitializeComponent();
            this.NameEnumTextbox.DataContext = this;
           
            if(enumKindObject != null)
            {
                this.enumKindObject = enumKindObject;
                EnumKindName = this.enumKindObject.MName;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (EnumKindName == null) return;
            var view = (DetailsView)this.sender;
            var immutableObject = view.BindingHelper.GetImmutableObject();
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            var controller = mainWindow.GetController();
            if(this.enumKindObject == null)
            {
                var ek = controller.CreateEnumLiteral(EnumKindName);
                controller.SetEnumerationLiteralToEnumeration(controller.GetUMLModel().Model.GetObject(immutableObject), ek);
            }
            else
            {

                controller.ModifyObjectName(controller.GetUMLModel().Model.GetObject(enumKindObject), EnumKindName);
            }

            view.BindingHelper = new MyNodeBindingHelper(mainWindow.GetController().GetUMLModel().Model.GetObject(immutableObject));
            mainWindow.DiagramView.GraphLayout = null;
            mainWindow.DiagramView.GraphLayout = controller.GetGraphLayoutModel().GraphLayout;
            this.Close();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.enumKindObject != null)
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                var controller = mainWindow.GetController();

                controller.RemoveObject(this.enumKindObject);
                var view = (DetailsView)this.sender;
                var immutableObject = view.BindingHelper.GetImmutableObject();
                view.BindingHelper = new MyNodeBindingHelper(mainWindow.GetController().GetUMLModel().Model.GetObject(immutableObject.ToMutable()));
                mainWindow.DiagramView.GraphLayout = null;
                mainWindow.DiagramView.GraphLayout = controller.GetGraphLayoutModel().GraphLayout;
                this.Close();
            }
        }
    }
}
