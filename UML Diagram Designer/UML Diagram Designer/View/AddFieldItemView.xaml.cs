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
    /// Interaction logic for AddItemView.xaml
    /// </summary>
    public partial class AddFieldItemView : Window
    {
        private List<ImmutableObject> displayArray;
        private ImmutableObject field;
        public string FName { get; set; }
        private object sender;
        public AddFieldItemView(List<ImmutableObject> list, object sender, ImmutableObject immutable = null)
        {
            displayArray = list;
            this.sender = sender;
            InitializeComponent();
            this.TypeListBox.DataContext = displayArray;
            this.NameTextbox.DataContext = this;
            if (immutable != null)
            {
                this.field = immutable;
                FName = immutable.MName;
                

                foreach (var obj in (List<ImmutableObject>)TypeListBox.DataContext)
                {
                    if (obj.MName.Equals(immutable.MType.MName))
                    {
                        this.TypeListBox.SelectedItem = obj;
                    }
            }
            }

            this.TypeListBox.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (FName == null|| e.Source == null) return;
            var view = (DetailsView)this.sender;
            var immutableObject = view.BindingHelper.GetImmutableObject();
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            var controller = mainWindow.GetController();
            if(this.field == null)
            {
                var f = controller.CreateProperty(FName);
                controller.SetObjectType(controller.GetUMLModel().Model.GetObject(f), (ImmutableObject)TypeListBox.SelectedItem);
                controller.SetPropertyToClass(controller.GetUMLModel().Model.GetObject(immutableObject), controller.GetUMLModel().Model.GetObject(f));
            }
            else
            {
                
                controller.ModifyObjectName(controller.GetUMLModel().Model.GetObject(field), FName);
                controller.SetObjectType(controller.GetUMLModel().Model.GetObject(field), (ImmutableObject)TypeListBox.SelectedItem);
            }

            view.BindingHelper = new MyNodeBindingHelper(mainWindow.GetController().GetUMLModel().Model.GetObject(immutableObject.ToMutable()));
            mainWindow.DiagramView.GraphLayout = null;
            mainWindow.DiagramView.GraphLayout = controller.GetGraphLayoutModel().GraphLayout;
            this.Close();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.field != null)
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                var controller = mainWindow.GetController();

                controller.RemoveObject(this.field);
                var view = (DetailsView)this.sender;
                var immutableObject = view.BindingHelper.GetImmutableObject();
                view.BindingHelper = new MyNodeBindingHelper(mainWindow.GetController().GetUMLModel().Model.GetObject(immutableObject.ToMutable()));
                mainWindow.DiagramView.GraphLayout = null;
                mainWindow.DiagramView.GraphLayout = controller.GetGraphLayoutModel().GraphLayout;
                this.Close();
            }
        }



        //private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    var value = e.Source as TextBox;
        //    this.Name = value.Text;
        //}
    }
}
