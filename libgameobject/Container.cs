using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using libpathgraph;
using Util;

namespace libgameobject
{
    public class Container
    {
        public delegate void OnChangePropertyHandle(Container cont);
        public event OnChangePropertyHandle OnChangeProperty;

        public delegate void OnFineHandle(Container cont, double fine);
        public event OnFineHandle OnFine;

        public ContainerStatusInfo StatusInfo;

        private Node point;
        public Node Point => point;

        private int id { set; get; }
        public int Id => id;

        private bool repletion { set; get; } = false;
        public bool Repletion
        {
            get { return repletion; }
            set
            {
                if (repletion != value)
                {
                    repletion = value;
                    ConnectionToServer.SendDataToServer("Контейнер изменил статус");
                    if (!repletion)
                    {
                        PayFine();
                    }
                    else
                    {
                        StatusInfo.t3++;
                        Tools.Message(MessageStatus.Warning, string.Format(Localization.GetText("Text11"),Id));
                    }
                }
            }
        }

        private double fine { set; get; }
        public double Fine => fine;

        private double FillRate { set; get; } 
        //private double porog { set; get; }

        private double capacity { set; get; } = 0;

        public double Capacity {
            get { return capacity; }
            set {
                capacity = value;
                if (value >= Settings.LimitCapacityContainer)
                {
                    Repletion = true;
                    capacity = Settings.LimitCapacityContainer;
                }
                else
                {
                    Repletion = false;
                }
                OnChangeProperty?.Invoke(this);
            }
        }

        public Container(int _id, Node _node)
        {
            StatusInfo = new ContainerStatusInfo();
            id = _id;
            point = _node;
            FillRate = Settings.KOccupancyContainer * (Tools.rand.Next(Settings.MinKContainer, Settings.MaxKContainer)/100d);
            //porog = Tools.rand.NextDouble();
        }

        public void PayFine()
        {
            if (Fine > 0)
            {
                StatusInfo.l2.Add(Fine);
                OnFine?.Invoke(this, Math.Round(Fine, 2));
                fine = 0;
            }
        }

        public void GameCicle(double delta)
        {
            if (Repletion) fine += delta * Settings.KFine;
            //double CapacityFill = Tools.rand.NextDouble();
            //double k = Tools.rand.NextDouble();
            //if (k > porog)
                Capacity += delta * FillRate;
        }

        internal void Reset()
        {
            Capacity = fine = 0;
            StatusInfo = new ContainerStatusInfo();
            //throw new NotImplementedException();
        }
    }
}
