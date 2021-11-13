using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using System.Windows.Media;
using System.Windows;
using Stylet;

namespace UML_Diagram_Designer.Models
{
    public abstract class EdgeLayout
    {
        private readonly double TriangleSideLenght = 20;
        private readonly GraphLayout _graph;
        private readonly object _edgeObject;
        private readonly IntPtr _graphVizEdge;
        private readonly NodeLayout _source;
        private readonly NodeLayout _target;
        protected PathGeometry _pathGeometry;
        protected EdgeLayout(GraphLayout graph,NodeLayout source, NodeLayout target, object edgeObject)
        {
            this._pathGeometry = new PathGeometry();
            _graph = graph;
            _edgeObject = edgeObject;
            _source = source;
            _target = target;
            _graphVizEdge = CGraphLib.agedge(_graph.GraphVizGraph, source.GraphVizNode, target.GraphVizNode, null, true);

        }


        public GraphLayout Graph => _graph;
        public object EdgeObject => _edgeObject;
        public NodeLayout Source => _source;
        public NodeLayout Target => _target;
        internal IntPtr GraphVizEdge => _graphVizEdge;


        public  PathGeometry PathGeometry
        {
            get { return this._pathGeometry; }
            set
            {
                this._pathGeometry = value;
                CreateTriangleGeometry();
            }
        }

        public PathGeometry TrianglePath { get; set; }

        protected virtual bool IsClosedTriangle()
        {
            return false;
        }

        internal void SetControlPoints(string positions)
        {
            var pointsBuilder = ImmutableArray.CreateBuilder<Point2D>();
            var splinesStr = positions.Trim().Split(';');
            foreach (var splineStr in splinesStr)
            {
                var pointsStr = splineStr.Trim().Split(' ');
                int index = 0;
                Point2D? startPoint = null;
                Point2D? endPoint = null;
                pointsBuilder.Clear();
                foreach (var pointStr in pointsStr)
                {
                    if (index == 0 && pointStr.StartsWith("s,"))
                    {
                        startPoint = new Point2D(pointStr.Substring(2), GraphLayout.DefaultDpi);
                    }
                    else if (index == 0 && pointStr.StartsWith("e,"))
                    {
                        endPoint = new Point2D(pointStr.Substring(2), GraphLayout.DefaultDpi);
                    }
                    else
                    {
                        pointsBuilder.Add(new Point2D(pointStr, GraphLayout.DefaultDpi));
                        ++index;
                    }
                }
                Debug.Assert(pointsBuilder.Count % 3 == 1);
                if (startPoint != null) pointsBuilder[0] = startPoint.Value;
                if (endPoint != null) pointsBuilder[pointsBuilder.Count - 1] = endPoint.Value;
              
            }
            CreateBezierSegment(pointsBuilder.ToImmutable());
        }
        private void CreateBezierSegment(ImmutableArray<Point2D> immutableArray)
        {
            var path = new PathGeometry();
            var pathFigure = new PathFigure();
            pathFigure.IsClosed = false;
            pathFigure.StartPoint = new Point(immutableArray[0].X, immutableArray[0].Y);

            for (int i = 1; i < immutableArray.Length; i += 3)
            {
                var segment = new BezierSegment(new Point(immutableArray[i].X, immutableArray[i].Y), new Point(immutableArray[i + 1].X, immutableArray[i + 1].Y), new Point(immutableArray[i + 2].X, immutableArray[i + 2].Y), true);
                pathFigure.Segments.Add(segment);
            }
            path.Figures.Add(pathFigure);

            PathGeometry = path;
        }

        private void CreateTriangleGeometry()
        {
            var pathfigure = new PathFigure();
            var pathfigure1 = new PathFigure();
            pathfigure1.IsClosed = false;
            pathfigure.IsClosed = false;

            var p1 = pathfigure.StartPoint = pathfigure1.StartPoint = ((BezierSegment)PathGeometry.Figures[0].Segments[PathGeometry.Figures[0].Segments.Count - 1]).Point3;
            Point p2;
            if (PathGeometry.Figures[0].Segments.Count > 1)
            {
                p2 = NormalizeSecondPoint(p1, ((BezierSegment)PathGeometry.Figures[0].Segments[PathGeometry.Figures[0].Segments.Count - 2]).Point3);
            }
            else
            {
                p2 = NormalizeSecondPoint(p1,(PathGeometry.Figures[0]).StartPoint) ;

            }

            var rotationLeft = Math.PI / 6;
            var rotationRight = -(Math.PI / 6);

            double x1 = p1.X + (p2.X - p1.X) * Math.Cos(rotationLeft) + (p2.Y - p1.Y) * Math.Sin(rotationLeft);
            double y1 = p1.Y - (p2.X - p1.X) * Math.Sin(rotationLeft) + (p2.Y - p1.Y) * Math.Cos(rotationLeft);


            double x2 = p1.X + (p2.X - p1.X) * Math.Cos(rotationRight) + (p2.Y - p1.Y) * Math.Sin(rotationRight);
            double y2 = p1.Y - (p2.X - p1.X) * Math.Sin(rotationRight) + (p2.Y - p1.Y) * Math.Cos(rotationRight);

            var segment = new LineSegment(new Point(x1, y1), true);
            var segment1 = new LineSegment(new Point(x2, y2), true);

            pathfigure.Segments.Add(segment);
            pathfigure1.Segments.Add(segment1);

            var pathFigureCollection = new PathFigureCollection();
            pathFigureCollection.Add(pathfigure);
            pathFigureCollection.Add(pathfigure1);

            if (IsClosedTriangle())
            {
                var pathfigure3 = new PathFigure();
                pathfigure3.IsClosed = false;
                pathfigure3.StartPoint = new Point(x1, y1);
                var segment3 = new LineSegment(new Point(x2, y2), true);
                pathfigure3.Segments.Add(segment3);
                pathFigureCollection.Add(pathfigure3);
            }

            var pathGeometry = new PathGeometry();
            pathGeometry.Figures = pathFigureCollection;

            TrianglePath = pathGeometry;
        }


        private Point NormalizeSecondPoint(Point p1, Point p2)
        {
            double angle = Math.Atan(Math.Abs(p1.X - p2.X) / Math.Abs(p1.Y - p2.Y));

            
            double xdiff = Math.Sin(angle) * TriangleSideLenght;
            double ydiff = Math.Cos(angle) * TriangleSideLenght;

            double p = p1.X > p2.X ?  p1.X - xdiff : p1.X + xdiff;
            double q = p1.Y > p2.Y ?  p1.Y - ydiff : p1.Y + ydiff;

            return new Point(p, q);
        } 
    }


}

