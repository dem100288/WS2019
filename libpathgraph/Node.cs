using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Util;

namespace libpathgraph
{
    public class     Node
    {
        
        public delegate void OnChangeCoordinateHandle(Node node);
        public event OnChangeCoordinateHandle OnChangePoint;

        private string Name;
        private Point coordinate { set; get; }
        public Point Coordinate => coordinate;
        public Point ViewCoordinate
        {
            get
            {
                return new Point(coordinate.X * Tools.ScaleX, coordinate.Y * Tools.ScaleY);
            }
        }
        public int id { set; get; }
        private List<LinkNode> Links;
        public List<LinkNode> OutLinks => Links.Where(x => x.NodeStart == this).ToList();
        public List<LinkNode> InLinks => Links.Where(x => x.NodeEnd == this).ToList();

        internal Node(Point _point, int _id)
        {
            coordinate = _point;
            id = _id;
            Links = new List<LinkNode>();
        }

        public void AddLink(LinkNode link)
        {
            Links.Add(link);
        }
    }
}
