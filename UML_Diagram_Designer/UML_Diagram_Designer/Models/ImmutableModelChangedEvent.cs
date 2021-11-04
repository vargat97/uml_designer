using MetaDslx.Modeling;
using System;
using System.Collections.Generic;
using System.Text;

namespace UML_Diagram_Designer.Models
{
    public class ImmutableModelChangedEvent
    {
        private ImmutableModel _immutableModel;

        public ImmutableModel ImmutableModel
        {
            get { return this._immutableModel; }
            private set { this._immutableModel = value; }
        }

        public ImmutableModelChangedEvent(ImmutableModel immutableModel)
        {
            this._immutableModel = immutableModel;
        }

    }
}
