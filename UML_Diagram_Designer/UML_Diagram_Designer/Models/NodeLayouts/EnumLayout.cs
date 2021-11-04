using MetaDslx.Languages.Uml.Model;
using MetaDslx.Modeling;
using System;
using System.Collections.Generic;
using System.Text;

namespace UML_Diagram_Designer.Models
{
    public class EnumLayout: NodeLayout
    {
        private List<ImmutableObject> _enumKinds;

        public List<ImmutableObject> EnumKinds
        {
            get { return this._enumKinds; }
            set { this._enumKinds = value; }
        }
        public EnumLayout(GraphLayout graphLayout, object nodeObject) : base(graphLayout,nodeObject)
        {
            _enumKinds = new List<ImmutableObject>();
            foreach(var enumkind in ((Enumeration)nodeObject).MChildren)
            {
                this._enumKinds.Add(enumkind);
            }
        }
    }
}
