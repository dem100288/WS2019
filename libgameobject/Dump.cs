using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libpathgraph;
using Util;

namespace libgameobject
{
    public class Dump
    {
        private Node point;
        private double costTrash { set; get; } = 0.1;
        public double CostTrash => costTrash;
        public Node Point => point;

        private int id { set; get; }
        public int Id => id;

        public Dump(int _id, Node _node)
        {
            id = _id;
            point = _node;
        }

        public void Interact(Car car)
        {
            var d = car.Capacity;
            car.Capacity = 0;
            car.StatusInfo.l1.Add(d);
            Tools.Message(MessageStatus.Info, string.Format(Localization.GetText("Text12"), car.Id, d), true);
            Simulation.ChangeCoinsUp(d*CostTrash);
        }
    }
}
