using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text;

namespace UML_Diagram_Designer.Models
{
    public class GraphLayout
    {
        public const double DefaultDpi = 72;

        private bool _isSubGraph;

        private string _engine;
        private object _graphObject;
        private Dictionary<object, NodeLayout> _objectToNodeMap;
        private Dictionary<object, EdgeLayout> _objectToEdgeMap;

        private IntPtr _graph;

        internal IntPtr _graphBbAttribute;
        internal IntPtr _graphRankSepAttribute;
        internal IntPtr _graphNodeSepAttribute;
        internal IntPtr _graphLenAttribute;
        internal IntPtr _graphMarginAttribute;

        internal IntPtr _nodeWidthAttribute;
        internal IntPtr _nodeHeightAttribute;
        internal IntPtr _nodeShapeAttribute;
        internal IntPtr _nodePosAttribute;
        internal IntPtr _nodeStyleAttribute;
        internal IntPtr _nodeMarginAttribute;

        internal IntPtr _edgePosAttribute;
        internal IntPtr _edgeDirAttribute;
        private string _dot;
        private bool _dirty;

        private IntPtr _graphVizNode;
        private IntPtr _graphVizSubGraph;
        public IEnumerable<NodeLayout> AllNodes => _objectToNodeMap.Values;
        public IEnumerable<EdgeLayout> AllEdges => _objectToEdgeMap.Values;

        public double NodeSeparation { get; set; }
        public double RankSeparation { get; set; }
        public double EdgeLength { get; set; }
        public double NodeMargin { get; set; }
        internal virtual IntPtr GraphVizNode => _graphVizNode;
        internal virtual IntPtr GraphVizSubGraph => _graphVizSubGraph;
        internal IntPtr GraphVizGraph => _graph;
        public object GraphObject => _graphObject;

        public bool IsSubGraph => _isSubGraph;
        public Point2D Position { get; internal set; }
        public Point2D Size { get; internal set; }


        public GraphLayout(string engine)
        {
            _isSubGraph = true;

            _graphVizSubGraph = CGraphLib.agopen(null, Agdesc_t.none, IntPtr.Zero);
            _graphVizNode = CGraphLib.agnode(_graphVizSubGraph, null, true);
            _graph = this._graphVizSubGraph;
            _dirty = true;
            _engine = engine;
            _graphObject = null;
            _objectToNodeMap = new Dictionary<object, NodeLayout>();
            _objectToEdgeMap = new Dictionary<object, EdgeLayout>();

            _graphBbAttribute = CGraphLib.agattr(_graph, CGraphLib.AGRAPH, "bb", "");
            _graphRankSepAttribute = CGraphLib.agattr(_graph, CGraphLib.AGRAPH, "ranksep", "2");
            _graphNodeSepAttribute = CGraphLib.agattr(_graph, CGraphLib.AGRAPH, "nodesep", "1");
            _graphLenAttribute = CGraphLib.agattr(_graph, CGraphLib.AGRAPH, "len", "3");
            _graphMarginAttribute = CGraphLib.agattr(_graph, CGraphLib.AGRAPH, "margin", "10");

            _nodeWidthAttribute = CGraphLib.agattr(_graph, CGraphLib.AGNODE, "width", "0");
            _nodeHeightAttribute = CGraphLib.agattr(_graph, CGraphLib.AGNODE, "height", "0");
            _nodeShapeAttribute = CGraphLib.agattr(_graph, CGraphLib.AGNODE, "shape", "rectangle");
            _nodePosAttribute = CGraphLib.agattr(_graph, CGraphLib.AGNODE, "pos", "");
            _nodeStyleAttribute = CGraphLib.agattr(_graph, CGraphLib.AGNODE, "style", "");
            _nodeMarginAttribute = CGraphLib.agattr(_graph, CGraphLib.AGNODE, "margin", "0");

            _edgePosAttribute = CGraphLib.agattr(_graph, CGraphLib.AGEDGE, "pos", "");
            _edgeDirAttribute = CGraphLib.agattr(_graph, CGraphLib.AGEDGE, "dir", "none");

        }
        internal void SetDirty()
        {
            _dirty = true;
        }
        public NodeLayout FindNodeLayout(object nodeObject)
        {
            if (_objectToNodeMap.TryGetValue(nodeObject, out var result)) return result;
            else return null;
        }

        public EdgeLayout FindEdgeLayout(object edgeObject)
        {
            if (_objectToEdgeMap.TryGetValue(edgeObject, out var result)) return result;
            else return null;
        }
        public void ComputeLayout()
        {
            if (!_dirty) return;
            foreach (var node in _objectToNodeMap.Values)
            {
                if (!node.IsSubGraph)
                {
                    if (node.PreferredPosition != null)
                    {
                        CGraphLib.agxset(node.GraphVizNode, _nodePosAttribute, node.PreferredPosition.ToString());
                    }
                    else
                    {
                        CGraphLib.agxset(node.GraphVizNode, _nodePosAttribute, "");
                    }
                    if (node.PreferredSize != null)
                    {
                        CGraphLib.agxset(node.GraphVizNode, _nodeWidthAttribute, node.PreferredSize?.X.ToString());
                        CGraphLib.agxset(node.GraphVizNode, _nodeHeightAttribute, node.PreferredSize?.Y.ToString());
                    }
                    else
                    {
                        CGraphLib.agxset(node.GraphVizNode, _nodeWidthAttribute, "");
                        CGraphLib.agxset(node.GraphVizNode, _nodeHeightAttribute, "");
                    }
                }
                else
                {
                    CGraphLib.agxset(node.GraphVizSubGraph, _graphMarginAttribute, (this.NodeMargin * DefaultDpi).ToString());
                }
            }
            CGraphLib.agxset(_graph, _graphNodeSepAttribute, this.NodeSeparation.ToString());
            CGraphLib.agxset(_graph, _graphRankSepAttribute, this.RankSeparation.ToString());
            CGraphLib.agxset(_graph, _graphLenAttribute, this.EdgeLength.ToString());
            _dot = GraphVizLib.Instance.Layout(_graph, _engine);
            var graphBb = Marshal.PtrToStringAnsi(CGraphLib.agxget(_graph, _graphBbAttribute));
            if (!string.IsNullOrEmpty(graphBb))
            {
                var parts = graphBb.Trim().Split(',');
                if (parts.Length == 4)
                {
                    double x1, y1, x2, y2;
                    for (int i = 0; i < parts.Length; i++)
                    {
                        var charSubParts = parts[i].Split('.');
                        if (charSubParts.Length > 1)
                        {
                            var subPart = charSubParts[0] + ',' + charSubParts[1];
                            parts[i] = subPart;
                        }
                    }


                    if (Double.TryParse(parts[0], out x1) && Double.TryParse(parts[1], out y1) &&
                        Double.TryParse(parts[2], out x2) && Double.TryParse(parts[3], out y2))
                    {
                        
                        x1 /= DefaultDpi;
                        y1 /= DefaultDpi;
                        x2 /= DefaultDpi;
                        y2 /= DefaultDpi;

                        this.Position = new Point2D((x1 + x2) / 2, (y1 + y2) / 2);
                        this.Size = new Point2D(x2 - x1, y2 - y1);
                    }



                    CGraphLib.agxset(_graph, _graphBbAttribute, "");
                }
            }
            foreach (var node in _objectToNodeMap.Values)
            {
                if (node.IsSubGraph)
                {
                    var gvBb = Marshal.PtrToStringAnsi(CGraphLib.agxget(node.GraphVizSubGraph, _graphBbAttribute));
                    if (!string.IsNullOrEmpty(gvBb))
                    {
                        var parts = gvBb.Trim().Split(',');
                        if (parts.Length == 4)
                        {
                            double x1, y1, x2, y2;
                            for (int i = 0; i < parts.Length; i++)
                            {
                                var charSubParts = parts[i].Split('.');
                                if (charSubParts.Length > 1)
                                {
                                    var subPart = charSubParts[0] + ',' + charSubParts[1];
                                    parts[i] = subPart;
                                }
                            }
                            if (double.TryParse(parts[0], out x1) && double.TryParse(parts[1], out y1) &&
                                double.TryParse(parts[2], out x2) && double.TryParse(parts[3], out y2))
                            {
                                x1 /= DefaultDpi;
                                y1 /= DefaultDpi;
                                x2 /= DefaultDpi;
                                y2 /= DefaultDpi;
                                node.Position = new Point2D((x1 + x2) / 2, (y1 + y2) / 2);
                                node.Size = new Point2D(x2 - x1, y2 - y1);
                            }
                        }
                        CGraphLib.agxset(node.GraphVizSubGraph, _graphBbAttribute, "");
                    }
                    else
                    {
                        if (node.PreferredSize != null) node.Size = node.PreferredSize.Value;
                    }
                }
                else
                {
                    var gvPos = Marshal.PtrToStringAnsi(CGraphLib.agxget(node.GraphVizNode, _nodePosAttribute));
                    if (!string.IsNullOrEmpty(gvPos))
                    {
                        node.Position = new Point2D(gvPos, GraphLayout.DefaultDpi);
                        CGraphLib.agxset(node.GraphVizNode, _nodePosAttribute, "");
                    }
                    var gvWidth = Marshal.PtrToStringAnsi(CGraphLib.agxget(node.GraphVizNode, _nodeWidthAttribute));
                    var gvHeight = Marshal.PtrToStringAnsi(CGraphLib.agxget(node.GraphVizNode, _nodeHeightAttribute));
                    if (!string.IsNullOrEmpty(gvWidth) && !string.IsNullOrEmpty(gvHeight))
                    {
                        if (double.TryParse(gvWidth, out var width) && double.TryParse(gvHeight, out var height))
                        {
                            node.Size = new Point2D(width, height);
                        }
                    }
                    else
                    {
                        if (node.PreferredSize != null) node.Size = node.PreferredSize.Value;
                    }
                }
            }
            foreach (var edge in _objectToEdgeMap.Values)
            {
                var gvPos = Marshal.PtrToStringAnsi(CGraphLib.agxget(edge.GraphVizEdge, _edgePosAttribute));
                if (!string.IsNullOrEmpty(gvPos))
                {
                    edge.SetControlPoints(gvPos);
                    CGraphLib.agxset(edge.GraphVizEdge, _edgePosAttribute, "");
                }
            }
            _dirty = false;
        }

        public GraphLayout AddNode(NodeLayout nodeLayout)
        {
            nodeLayout.PreferredSize = new Point2D(100, 100);
            _dirty = true;
            if (this.FindNodeLayout(nodeLayout.NodeObject) != null)
            {
                return this;
            }
            _objectToNodeMap.Add(nodeLayout.NodeObject, nodeLayout);
            return this;
        }

        public bool AddEdge(object sourceNodeObject, object targetNodeObject, EdgeLayout edgeLayout, bool autoAddNodes = false)
        {
            if (sourceNodeObject == null) throw new ArgumentNullException(nameof(sourceNodeObject));
            if (targetNodeObject == null) throw new ArgumentNullException(nameof(targetNodeObject));

            var source = this.FindNodeLayout(sourceNodeObject);
            var target = this.FindNodeLayout(targetNodeObject);

            if (source.IsSubGraph) throw new ArgumentException("The source of the edge must be a simple node, it cannot be a subgraph.", nameof(sourceNodeObject));
            if (target.IsSubGraph) throw new ArgumentException("The target of the edge must be a simple node, it cannot be a subgraph.", nameof(targetNodeObject));

            if (_objectToEdgeMap.ContainsKey(edgeLayout.EdgeObject))
            {
                return false;
            }
            _dirty = true;
            CGraphLib.agxset(edgeLayout.GraphVizEdge, _edgeDirAttribute, "none");
            _objectToEdgeMap.Add(edgeLayout.EdgeObject, edgeLayout);

            return true;
        }

    }
}
