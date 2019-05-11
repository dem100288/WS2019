using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libpathgraph;

namespace libgameobject
{
    public class Station
    {
        private Node point;
        public Node Point => point;

        private int id { set; get; }
        public int Id => id;

        public Station(int _id, Node _node)
        {
            id = _id;
            point = _node;
        }

        public void Interact(Car car)
        {

        }
    }
}
