using System;

using System.Windows;
using System.Windows.Controls;

using MetaDslx.Modeling;

namespace UML_Diagram_Designer.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        public MainWindow()
        {
        }
        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //TODOO
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {/*
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".uml";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                this.DiagramView.GraphLayout = controller.OpenFileClickControl(dlg.FileName);
            }
            */
        }
        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            /*
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".xmi";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string fileName = dlg.FileName;
                controller.SaveFileClickControl(fileName);

            }
            */
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (controller.GetUMLModel() != null)
            {
                MessageBoxResult result = MessageBox.Show("There are unsaved changes. Would you like to save it?", "Save changes", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        this.SaveMenuItem_Click(this, new RoutedEventArgs());

                        break;
                    case MessageBoxResult.No:
                        Application.Current.Shutdown();
                        break;
                }

            }
            */
            Application.Current.Shutdown();
        }

        private void DetailsGrid_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            /*
            var diagramView = (DiagramView)e.Source;
            var point = e.GetPosition(diagramView);
            var diagramHost = diagramView.GetVisualHost();
            var node = diagramHost.GetNodeLayoutFromNodeContents(new MetaDslx.GraphViz.Point2D(point.X, point.Y));
            if (node != null)
            {
                DetailsView.BindingHelper = new MyNodeBindingHelper((ImmutableObject)node.NodeObject);
                if (!this.DetailsView.DrawLine) e.Handled = true;
            }
            else { e.Handled = true; }
           */
        }

    }
}
