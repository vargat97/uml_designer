using MetaDslx.Languages.Uml.Model;
using MetaDslx.Languages.Uml.Serialization;
using MetaDslx.Modeling;
using Microsoft.CodeAnalysis;
using Microsoft.Win32;
using Stylet;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using UML_Diagram_Designer.Models;

namespace UML_Diagram_Designer.ViewModels
{
    public class RootViewModel : Conductor<object>.Collection.OneActive
    {
        private DiagramViewModel _diagramViewModel;
        private DiagramEditorViewModel _diagramEditorViewModel;

        private bool _isDirty = false;
        public void OpenFile()
        {
            if (_isDirty)
            {
                this.SaveDialogOpen();
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
            {

                var model = this.ReadUMLModelFromWhiteStarUmlFile(openFileDialog);
                EventAggregator eventAggregator = new EventAggregator();
                this._diagramEditorViewModel = new DiagramEditorViewModel(model, eventAggregator);
                this._diagramViewModel = new DiagramViewModel(model,this._diagramEditorViewModel,eventAggregator);

                this.Items.Add(this._diagramViewModel);
                this.Items.Add(this._diagramEditorViewModel);

                this._isDirty = true;

            }
            
        }

        public void CreateNewModel()
        {
            if (_isDirty)
            {
                this.SaveDialogOpen();
            }


            var mutablemodel = new MutableModel();
            var model = mutablemodel.ToImmutable();
            EventAggregator eventAggregator = new EventAggregator();
            this._diagramEditorViewModel = new DiagramEditorViewModel(model, eventAggregator);
            this._diagramViewModel = new DiagramViewModel(model, this._diagramEditorViewModel, eventAggregator);

            this.Items.Add(this._diagramViewModel);
            this.Items.Add(this._diagramEditorViewModel);

        }
        public void ExitProgram()
        {
            System.Windows.Application.Current.Shutdown();
        }
        public void SaveModel()
        {
            this.SaveDialogOpen();
        }
        private void SaveDialogOpen()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".xmi";
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                UmlXmiSerializer serializer = new UmlXmiSerializer();
                serializer.WriteModelToFile(fileName, _diagramViewModel.ImmutableModel);
            }

            this.Items.Remove(this._diagramViewModel);
            this.Items.Remove(this._diagramEditorViewModel);
        }
        private ImmutableModel ReadUMLModelFromWhiteStarUmlFile(OpenFileDialog dialog)
        {

            if (Path.GetExtension(dialog.FileName) == ".uml") return this.ReadFromUMLFile(dialog.FileName);
            else if (Path.GetExtension(dialog.FileName) == ".xmi") return this.ReadFromXMIFile(dialog.FileName);
            return null;
        }

        private ImmutableModel ReadFromUMLFile(string fileName)
        {
            UmlDescriptor.Initialize();

            var umlSerializer = new WhiteStarUmlSerializer();
            var model = umlSerializer.ReadModelFromFile(fileName, out var diagnostics);

            DiagnosticFormatter df = new DiagnosticFormatter();
            if (diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
            {
                diagnostics = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToImmutableArray();
            }
            for (int i = 0; i < 10 && i < diagnostics.Length; i++)
            {
                Console.WriteLine(df.Format(diagnostics[i]));
            }

            return model;
        }

        private ImmutableModel ReadFromXMIFile(string fileName)
        {
            UmlDescriptor.Initialize();
            var xmiSerializer = new UmlXmiSerializer();
            var model = xmiSerializer.ReadModelFromFile(fileName);

            return model;
        }

    }

}
