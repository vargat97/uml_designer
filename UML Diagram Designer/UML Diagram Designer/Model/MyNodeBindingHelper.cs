using DiagramDesigner.View;
using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using MetaDslx.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiagramDesigner.Model
{
    public class MyNodeBindingHelper
    {
        private readonly ImmutableObject immutableObject;
        private readonly Dictionary<ImmutableObject, List<ImmutableObject>> operationsParameters = new Dictionary<ImmutableObject, List<ImmutableObject>>();
        public string Name { get; set; }

        public VisibilityKind NodeVisibility { get; private set; }
        public List<ImmutableObject> EnumKinds { get; private set; } = new List<ImmutableObject>();
        public List<ImmutableObject> Operations { get; private set; } = new List<ImmutableObject>();
        public List<ImmutableObject> Properties{get; private set; } = new List<ImmutableObject>();

      
        public NodeTypeEnums NodeType { get; private set; }

        public ImmutableObject GetImmutableObject()
        {
            return this.immutableObject;
        }
        public List<ImmutableObject> GetParameters(ImmutableObject methodObject)
        {
            return this.operationsParameters.TryGetValue(methodObject, out var value) ? value : null;
        }
        public enum NodeTypeEnums
        {
            Class,
            Interface,
            Enumeration
        }

        public MyNodeBindingHelper(ImmutableObject obj)
        {
            this.immutableObject = obj;

            this.Name = this.immutableObject.MName;
            this.NodeVisibility = ((NamedElement)this.immutableObject).Visibility;
            this.AddProperties();
            this.AddOpertations();
            this.AddEnumerations();
        }
        public DataTemplate ChooseDataTemplateFromModel()
        {
            var metaClasName = immutableObject.MMetaClass.MName;
            var parentName = immutableObject.MMetaClass.MParent.MName;
            DataTemplate template = null;
            if(metaClasName.Equals("Class") || parentName.Equals("Logical View")){
                template = (DataTemplate)Application.Current.FindResource("ClassNodeTemplate");
                NodeType = NodeTypeEnums.Class;
            }
            else if (metaClasName.Equals("Interface"))
            {
                template = (DataTemplate)Application.Current.FindResource("InterfaceNodeTemplate");
                NodeType = NodeTypeEnums.Interface;

            }
            else if (metaClasName.Equals("Enumeration"))
            {
                template = (DataTemplate)Application.Current.FindResource("EnumNodeTemplate");

                NodeType = NodeTypeEnums.Enumeration;
            }

            return template;
        }

        private void AddProperties()
        {
            foreach(var child in this.immutableObject.MChildren)
            {
                if (child.MMetaClass.MName.Equals("Property")) this.Properties.Add((Property)child);
            }
        }
        private void AddOpertations()
        {
            foreach (var child in this.immutableObject.MChildren)
            {
                if (child.MMetaClass.MName.Equals("Operation")) 
                    this.Operations.Add((Operation)child);

                if (child.MChildren != null)
                {
                    List<ImmutableObject> parameters = new List<ImmutableObject>();
                    foreach(var cc in child.MChildren)
                    {
                        parameters.Add(cc);
                    }
                    this.operationsParameters.Add(child, parameters);
                }
            }
        }
        private void AddEnumerations()
        {
            foreach (var child in this.immutableObject.MChildren)
            {
                if (child.MMetaClass.MName.Equals("EnumerationLiteral"))
                    this.EnumKinds.Add((EnumerationLiteral)child);
            }
        }
    }
}
