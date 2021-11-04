using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace UML_Diagram_Designer.Models
{
    public class EnumBindingSourceExtension: MarkupExtension
    {
        public Type EnumType { get; private set; }

        public EnumBindingSourceExtension(Type enumType)
        {
            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }
    }
}
