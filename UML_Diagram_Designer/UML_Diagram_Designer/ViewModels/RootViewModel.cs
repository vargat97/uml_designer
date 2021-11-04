using MetaDslx.Languages.Uml.Model;
using MetaDslx.Languages.Uml.Serialization;
using MetaDslx.Modeling;
using Microsoft.CodeAnalysis;
using Microsoft.Win32;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using UML_Diagram_Designer.Models;

namespace UML_Diagram_Designer.ViewModels
{
    public class RootViewModel: Conductor<object>.Collection.OneActive
    {
        private DiagramViewModel _diagramViewModel;
        private DiagramEditorViewModel _diagramEditorViewModel;

        public void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
            {

                var model = this.ReadUMLModelFromWhiteStarUmlFile(openFileDialog.FileName);
                EventAggregator eventAggregator = new EventAggregator();
                this._diagramEditorViewModel = new DiagramEditorViewModel(model, eventAggregator);
                this._diagramViewModel = new DiagramViewModel(model,this._diagramEditorViewModel,eventAggregator);

                this.Items.Add(this._diagramViewModel);
                this.Items.Add(this._diagramEditorViewModel);

            }
            
        }

        public void CreateNewModel()
        {

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

        private ImmutableModel ReadUMLModelFromWhiteStarUmlFile(string fileName)
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

    }

}
