using MetaDslx.Languages.Uml.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace UML_Diagram_Designer.Models
{
    public abstract class ClassifierLayout: NodeLayout
    {
        protected List<Feature> _features;

        protected ClassifierLayout(GraphLayout graphLayout, object nodeObject) : base(graphLayout, nodeObject)
        {
            this._features = new List<Feature>(((Classifier)nodeObject).Feature);

        }

    }
}
