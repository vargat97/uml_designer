using MetaDslx.Languages.Uml.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace UML_Diagram_Designer.Models
{
    public class InterfaceLayout: ClassifierLayout
    {
        private List<Property> _allAttributes;
        private List<Operation> _allOperations;

        public List<Operation> AllOperations
        {
            get { return this._allOperations; }
            set { this._allOperations = value; }
        }
        public List<Property> AllAttributes
        {
            get { return this._allAttributes; }
            set { this._allAttributes = value; }
        }

        public InterfaceLayout(GraphLayout graphLayout, object nodeObject) : base(graphLayout, nodeObject)
        {
            this._allAttributes = new List<Property>();
            this._allOperations = new List<Operation>();

            foreach (var feature in this._features)
            {
                if (feature.MMetaClass.MName.Equals("Property"))
                {
                    AllAttributes.Add((Property)feature);
                }
                else if (feature.MMetaClass.MName.Equals("Operation"))
                {
                    AllOperations.Add((Operation)feature);
                }
            }
        }


    }
}
