using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace libpathgraph
{
    public class LinkNode
    {
        private Node nodeStart;
        public Node NodeStart => nodeStart;
        private Node nodeEnd;
        public Node NodeEnd => nodeEnd;
        private double lenght;
        public double Lenght => lenght;
        public Vector vector;
        private LinkNode()
        {

        }

        private void CalcVector()
        {
            if ((NodeStart != null) && (NodeEnd != null))
            {
                vector = new Vector(NodeEnd.Coordinate.X - NodeStart.Coordinate.X,
                    NodeEnd.Coordinate.Y - NodeStart.Coordinate.Y);
                vector.Normalize();
            }
        }

        public static LinkNode NewLink(Node start, Node end)
        {
            LinkNode link = null;
            if (start != end)
            {
                link = new LinkNode();
                link.nodeStart = start;
                link.nodeEnd = end;
                link.lenght = start.Coordinate.Lenght(end.Coordinate);
                link.NodeStart.OnChangePoint += link.NodeStart_OnChangePoint;
                link.NodeEnd.OnChangePoint += link.NodeEnd_OnChangePoint;
                link.CalcVector();
                start.AddLink(link);
                end.AddLink(link);
            }
            return link;
        }

        private void NodeEnd_OnChangePoint(Node node)
        {
            CalcVector();
        }

        private void NodeStart_OnChangePoint(Node node)
        {
            CalcVector();
        }
    }
}
