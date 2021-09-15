using DiagramDesigner.Model;
using MetaDslx.Modeling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace DiagramDesigner.View
{
    /// <summary>
    /// Interaction logic for DetailsView.xaml
    /// </summary>
    public partial class DetailsView : UserControl
    {
        public bool DrawLine { get; set; }
        public Button EdgeButton { get; set; }
        public static readonly DependencyProperty BindingHelperProperty = DependencyProperty.Register("BindingHelper", typeof(MyNodeBindingHelper), typeof(DetailsView), new PropertyMetadata(BindingHelperChanged));
        public MyNodeBindingHelper BindingHelper
        {
            get { return (MyNodeBindingHelper)GetValue(BindingHelperProperty); }
            set { SetValue(BindingHelperProperty, value); }
        }

        public DetailsView()
        {
            InitializeComponent();
            this.DrawLine = false;
        }

        private  static void BindingHelperChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DetailsView dv = sender as DetailsView;
            if (dv == null) return;
            dv.OnItemsSourceChanged(e);
        }
        private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                this.DataContext = (MyNodeBindingHelper)e.NewValue;
            }
        }
        private void AddInterfaceButton_Click(object sender, RoutedEventArgs e)
        {
            AddBasicView view = new AddBasicView("Interface");
            view.ShowDialog();

        }

        private void AddClassButton_Click(object sender, RoutedEventArgs e)
        {
            AddBasicView view = new AddBasicView("Class");
            view.ShowDialog();
        }

        private void AddEnumButton_Click(object sender, RoutedEventArgs e)
        {

            AddBasicView view = new AddBasicView("Enumeration");
            view.ShowDialog();
        }

        private void FieldListBox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            var controller = mainWindow.GetController();
            var model = controller.GetUMLModel();
            var source = e.Source as ListBox;

            AddFieldItemView view;

            if(source == null)
            {
                view = new AddFieldItemView(model.GetTypesFromModel(), this, null);
            }
            else
            {
                var value = source.SelectedItem;
                view = new AddFieldItemView(model.GetTypesFromModel(), this, (ImmutableObject)value);
            }
            
            view.ShowDialog();

        }

        private void NameSetButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.NameTextBox.Text != null)
            {
                var n = this.NameTextBox.Text;
                MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                var controller = mainWindow.GetController();
                var immutableObject = ((MyNodeBindingHelper)this.DataContext).GetImmutableObject();

                controller.ModifyObjectName(controller.GetUMLModel().Model.GetObject(immutableObject), n);
                this.BindingHelper = new MyNodeBindingHelper(controller.GetUMLModel().Model.GetObject(immutableObject));
                mainWindow.DiagramView.GraphLayout = controller.GetGraphLayoutModel().GraphLayout;

            }


        }

        private void EnumButton_Click(object sender, RoutedEventArgs e)
        {
            var source = e.Source as ListBox;
            AddEnumKindsView view;

            if (source == null)
            {
                view = new AddEnumKindsView(this, null);
            }
            else
            {
                var value = source.SelectedItem;
                view = new AddEnumKindsView(this, (ImmutableObject)value);
            }

            view.ShowDialog();

        }

        private void MethodAddButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            var controller = mainWindow.GetController();
            var model = controller.GetUMLModel();
            var source = e.Source as ListBox;
            AddMethodFieldView view;

            if (source == null)
            {
                view = new AddMethodFieldView(model.GetTypesFromModel(),this,null);
            }
            else
            {
                var value = controller.GetUMLModel().Model.GetObject((ImmutableObject)source.SelectedItem);
                view = new AddMethodFieldView(model.GetTypesFromModel(),this,(ImmutableObject)value);
            }

            view.ShowDialog();
        }

        private void Remove_Node_Click(object sender, RoutedEventArgs e)
        {
            if(this.BindingHelper != null)
            {

                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                var controller = mainWindow.GetController();

                foreach (var child in BindingHelper.GetImmutableObject().MChildren)
                {
                    controller.RemoveObject(child);
                }

                controller.RemoveObject(this.BindingHelper.GetImmutableObject());
                BindingHelper = null;
                mainWindow.DiagramView.GraphLayout = null;
                mainWindow.DiagramView.GraphLayout = controller.GetGraphLayoutModel().GraphLayout;
            }
        }
        private void EdgeAddButton_Click(object sender, RoutedEventArgs e)
        {
            this.DrawLine = !this.DrawLine;
            Button btn = sender as Button;
            this.EdgeButton = btn;
            btn.Background = btn.Background == Brushes.Aqua ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF")) : Brushes.Aqua;
        }
    }
}
