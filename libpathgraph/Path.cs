using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace libpathgraph
{
    public class Path
    {
        public delegate void OnEndedPathHandle(Path path);
        public event OnEndedPathHandle OnEndedPath;

        private LinkNode currentLink;
        private Node nodeStart;
        public Node NodeStart => nodeStart;
        private Node nodeEnd;
        public Node NodeEnd => nodeEnd;
        private List<LinkNode> Links;
        public List<LinkNode> ListLinks => Links;
        public double Lenght => Links.Sum(x => x.Lenght);

        private Path()
        {

        }

        public void RemovePath()
        {
            OnEndedPath?.Invoke(this);
        }

        private Path(Node start, Node end, List<LinkNode> links)
        {
            nodeStart = start;
            nodeEnd = end;
            Links = links;
            currentLink = Links.FirstOrDefault(x => x.NodeStart == start);
        }

        public void Draw()
        {

        }

        private Point StepLink(Point point, double distance)
        {
            Vector v = new Vector(currentLink.vector.X * distance, currentLink.vector.Y * distance);
            return new Point(point.X + v.X, point.Y + v.Y);
        }

        public Point NextStep(Point point, double distance)
        {
            double LenghtToEndLink = point.Lenght(currentLink.NodeEnd.Coordinate);
            Point ret;
            if (LenghtToEndLink > distance)
            {
                ret = StepLink(point, distance);
            }
            else
            {
                if (currentLink.NodeEnd != NodeEnd)
                {
                    distance -= LenghtToEndLink;
                    currentLink = Links.FirstOrDefault(l => l.NodeStart == currentLink.NodeEnd);
                    ret = NextStep(currentLink.NodeStart.Coordinate, distance);
                }
                else
                {
                    ret = NodeEnd.Coordinate;
                    OnEndedPath?.Invoke(this);
                }
            }
            return ret;
        }
        

        public static Path FindPath(Node start, Node end)
        {
            Dictionary<Node, double> g = new Dictionary<Node, double>();
            Dictionary<Node, double> f = new Dictionary<Node, double>();
            Dictionary<Node, Node> parent = new Dictionary<Node, Node>();
            SortedList<double, Node> Q = new SortedList<double, Node>();
            List<Node> U = new List<Node>();

            g[start] = 0;
            f[start] = g[start] + Distance(start, end);
            Q.Add(f[start], start);

            while (Q.Count > 0)
            {
                Node current = Q.First().Value;
                if (current == end)
                {
                    List<LinkNode> result = new List<LinkNode>();
                    while (parent.ContainsKey(current))
                    {
                        result.Add(LinkNode.NewLink(parent[current], current));
                        current = parent[current];
                    }

                    return new Path(start, end, result);
                }

                Q.RemoveAt(0);
                U.Add(current);

                foreach (LinkNode outLink in current.OutLinks)
                {
                    double tentativeScore = g[current] + outLink.Lenght;
                    if (U.Contains(outLink.NodeEnd) 
                        && tentativeScore >= (g.ContainsKey(current) ? g[current] : double.PositiveInfinity)) continue;
                    if (!U.Contains(outLink.NodeEnd)
                        || tentativeScore < (g.ContainsKey(current) ? g[current] : double.PositiveInfinity))
                    {
                        parent[outLink.NodeEnd] = current;
                        g[outLink.NodeEnd] = tentativeScore;
                        f[outLink.NodeEnd] = g[outLink.NodeEnd] + Distance(outLink.NodeEnd, end);
                        if (!Q.ContainsValue(outLink.NodeEnd))
                        {
                            Q.Add(f[outLink.NodeEnd], outLink.NodeEnd);
                        }
                    }
                }
            }
            return null;
        }

        private static double Distance(Node start, Node end)
        {
            return Math.Sqrt(start.Coordinate.X * start.Coordinate.X + end.Coordinate.Y * end.Coordinate.Y);
        }
    }
}
