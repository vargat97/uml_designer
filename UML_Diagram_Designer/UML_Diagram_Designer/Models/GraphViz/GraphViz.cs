using System;
using System.Collections.Generic;
using System.Text;

namespace UML_Diagram_Designer.Models
{
    public static class GraphViz
    {
        internal static string GraphVizLibPath;

        public static void Initialize(string graphVizLibPath)
        {
            GraphVizLibPath = graphVizLibPath;
        }
    }
}
