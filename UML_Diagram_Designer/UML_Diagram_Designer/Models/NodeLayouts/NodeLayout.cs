using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace UML_Diagram_Designer.Models
{
    public class NodeLayout
    {
        private GraphLayout _graph;
        private object _nodeObject;
        private IntPtr _graphVizSubGraph;
        private IntPtr _graphVizNode;
        private Point2D? _preferredPosition;
        private Point2D? _preferredSize;

        public bool Selected { get; set; }
        public Point2D? PreferredPosition
        {
            get { return _preferredPosition; }
            set
            {
                if (value != _preferredPosition)
                {
                    _graph.SetDirty();
                    _preferredPosition = value;
                }
            }
        }
        public object NodeObject => _nodeObject;
        public Point2D? PreferredSize
        {
            get { return _preferredSize; }
            set
            {
                if (value != _preferredSize)
                {
                    _graph.SetDirty();
                    _preferredSize = value;
                }
            }
        }
        public GraphLayout Graph => _graph;
        public Point2D Position { get; internal set; }
        public Point2D Size { get; internal set; }
        public double Left => this.Position.X - this.Size.X / 2;
        public double Top => this.Position.Y - this.Size.Y / 2;
        public double Width => this.Size.X;
        public double Height => this.Size.Y;
        internal virtual IntPtr GraphVizNode => _graphVizNode;
        internal virtual IntPtr GraphVizSubGraph => _graphVizSubGraph;
        public bool IsSubGraph { get { return false; } private set { } }
        public NodeLayout(GraphLayout graph,object nodeObject)
        {
            Selected = false;
            _nodeObject = nodeObject;
            _graph = graph;
            Debug.Assert(graph.IsSubGraph);
            _graphVizSubGraph = graph.GraphVizSubGraph;
            _graphVizNode = CGraphLib.agnode(_graphVizSubGraph, null, true);
        }

    }
}
