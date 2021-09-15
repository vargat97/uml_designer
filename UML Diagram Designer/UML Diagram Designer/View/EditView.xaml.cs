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
using System.Windows.Shapes;

namespace DiagramDesigner.View
{
    /// <summary>
    /// Interaction logic for EditView.xaml
    /// </summary>
    public partial class EditView : Window
    {
        private ImmutableObject immutableObject;
        public EditView(ImmutableObject immutableObject)
        {
            this.immutableObject = immutableObject;
            var myNodeBindingHelper = new MyNodeBindingHelper(immutableObject);

            this.DataContext = myNodeBindingHelper;
            InitializeComponent();
            
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow.Controller.ModifyObjectName(MainWindow.Controller.GetUMLModel().Model.GetObject(this.immutableObject.ToMutable()), "ASD");
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
