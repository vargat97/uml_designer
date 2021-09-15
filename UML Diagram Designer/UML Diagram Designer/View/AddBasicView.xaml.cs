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
    /// Interaction logic for AddBasicView.xaml
    /// </summary>
    public partial class AddBasicView : Window
    {
        public string NodeName { get; set; }
        private string name;
        public AddBasicView(string name)
        {
            InitializeComponent();
            this.NameTextbox.DataContext = this;
            this.name = name;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (NodeName == null) return;
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if(this.name == "Class") mainWindow.GetController().CreateClass(NodeName);
            if (this.name == "Interface") mainWindow.GetController().CreateInterface(NodeName);
            if (this.name == "Enumeration") mainWindow.GetController().CreateEnum(NodeName);
            mainWindow.DiagramView.GraphLayout = null;
            mainWindow.DiagramView.GraphLayout = mainWindow.GetController().GetGraphLayoutModel().GraphLayout;

            this.Close();
        }
    }
}
