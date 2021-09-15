using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using MetaDslx.Languages.Uml.Serialization;
using MetaDslx.Modeling;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.Model
{
    public class UMLModel
    {
        public ImmutableModel Model { get; set; }
        private List<ImmutableObject> typesArray;

        public UMLModel(string path)
        {
            string extension = Path.GetExtension(path);
            if (extension.Equals(".uml"))
            {
                this.ReadUMLModelFromWhiteStarUmlFile(path);
            }
            else if (extension.Equals(".xmi"))
            {
                this.ReadUMLModelFromXMIExtension(path);
            }
        }

        public List<ImmutableObject> GetTypesFromModel()
        {
            if (this.typesArray != null)
            {
                return this.typesArray;
            }

            this.typesArray = new List<ImmutableObject>();
            foreach(var item in Model.Objects.OfType<Class>())
            {
                if(item.MMetaClass.MName.Equals("Class"))
                    this.typesArray.Add(item);
            }
            foreach (var item in Model.Objects.OfType<Interface>())
            {
                this.typesArray.Add(item);
            }
            foreach (var item in Model.Objects.OfType<Enumeration>())
            {
                this.typesArray.Add(item);
            }
            return this.typesArray;

        }


        /// <summary>
        /// Read the UML metamodel from a WhiteStarUml file. WhiteStarUml files have .uml extensions, so every .uml files should work.
        /// </summary>
        /// <param name="fileName">Defines the file path</param>
        private void ReadUMLModelFromWhiteStarUmlFile(string fileName)
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
            if (this.Model != null && this.Model.Name.Equals(fileName)) return;

            this.Model = model;
        }
        private void ReadUMLModelFromXMIExtension(string fileName)
        {
            UmlXmiSerializer serializer = new UmlXmiSerializer();
            var model = serializer.ReadModelFromFile(fileName);
            this.Model = model;
        }
    }
}
