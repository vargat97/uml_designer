using System;
using System.Collections.Generic;
using System.Text;

namespace UML_Diagram_Designer.model
{
    //TODO
    public class BaseComponent
    {
        protected IMediator mediator;

        public BaseComponent(IMediator mediator)
        {
            this.mediator = mediator;
        }

       
    }
}
