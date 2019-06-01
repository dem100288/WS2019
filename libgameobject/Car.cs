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
    public enum CarStatus { Idle = 0, Run = 1, Broken = 2 };

    public class Car: BaseCar
    {
        public delegate void OnChangePositionHandle(Car car);
        public event OnChangePositionHandle OnChangePosition;

        public delegate void OnChangeStatusHandle(Car car);
        public event OnChangeStatusHandle OnChangeStatus;
        public delegate void OnChangePropertyHandle(Car car);
        public event OnChangePropertyHandle OnChangeProperty;

        public CarStatusInfo StatusInfo;
        public TypeCar Type { set; get; }

        private int id { set; get; }
        public int Id
        {
            set
            {
                id = value;
            }
            get
            {
                return id;
            }
        }

        public double Fuel
        {
            get { return fuel; }
            set
            {
                if (fuel != value)
                {
                    if (value <= 0)
                    {
                        fuel = 0;
                        Status = CarStatus.Broken;
                    }
                    else
                    {
                        fuel = value;
                    }
                    OnChangeProperty?.Invoke(this);
                }
            }
        }

        public Point Position
        {
            set {
                position = value;
                OnChangePosition?.Invoke(this);
                }
            get { return position; }
        }

        public Point ViewPosition
        {
            get { return new Point(position.X * Tools.ScaleX, position.Y * Tools.ScaleY); }
        }

        public double Wearout
        {
            get {
                return wearout;
            }

            set
            {
                //var probability = Tools.rand.NextDouble();
                //if (probability <= Settings.ProbabilityOfWear)
                //    wearout = value;
                if (wearout >= Settings.LimitWearout)
                {
                    wearout = Settings.LimitWearout;
                    Status = CarStatus.Broken;
                }
                OnChangeProperty?.Invoke(this);
            }
        }
        public double Capacity
        {
            get { return capacity; }
            set
            {
                if (value >= Type.LimitCapacity)
                {
                    capacity = Type.LimitCapacity;
                }
                else
                {
                    capacity = value;
                }
                OnChangeProperty?.Invoke(this);
            }
        }

        public Path currentPath { set; get; }

        public CarStatus Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    OnChangeStatus?.Invoke(this);
                    if (status == CarStatus.Broken)
                    {
                        StatusInfo.t11++;
                        currentPath?.RemovePath();
                    }
                    ConnectionToServer.SendDataToServer("TODO обновление данных объекта на сервере");
                }
            }
        }

        public Car(TypeCar type)
        {
            Type = type;
            fuel = Type.FuelOfBuying;
            StatusInfo = new CarStatusInfo(this);
            Position = Simulation.FindStationById(1).Point.Coordinate;
            Tools.OnChangeScale += Tools_OnChangeScale;
        }

        private void Tools_OnChangeScale()
        {
            OnChangePosition?.Invoke(this);
        }

        public void Reset()
        {
            if (!Simulation.SimulationRun)
            {
                Fuel = Type.FuelOfBuying;
                Wearout = Settings.WearoutOfBuying;
                Capacity = 0;
            }
        }

        public void Repair()
        {
            var st = Simulation.Stations.Where(x => x.Point.Coordinate == Position).FirstOrDefault();
            if (st != null)
            {
                double w = 0;
                if (Simulation.Coins > Wearout * Type.CostRepairs)
                {
                    Simulation.ChangeCoinsDown(Wearout * Type.CostRepairs);
                    w = wearout;
                    wearout = 0;
                    OnChangeProperty?.Invoke(this);
                }
                else
                {
                    if (Simulation.Coins > 0)
                    {
                        w = Simulation.Coins / Type.CostRepairs;
                        wearout -= Simulation.Coins / Type.CostRepairs;
                        OnChangeProperty?.Invoke(this);
                        Simulation.ChangeCoinsDown(Simulation.Coins);
                    }
                    Tools.Message(MessageStatus.Warning, string.Format(Util.Localization.GetText("Text1"), Id));
                }
                StatusInfo.l3.Add(w);
                Tools.Message(MessageStatus.Info, string.Format(Util.Localization.GetText("Text2"), Id, Math.Round(w, 2)));
                UpdateStatusAfterBroken();
            }
            else
            {
                Tools.Message(MessageStatus.Warning, string.Format(Util.Localization.GetText("Text3"), Id), true);
            }
        }

        private void UpdateStatusAfterBroken()
        {
            if (Status == CarStatus.Broken)
            {
                if ((Fuel > 0) && (wearout < Settings.LimitWearout))
                {
                    Status = CarStatus.Idle;
                }
            }
        }

        public void Refueling()
        {
            var st = Simulation.Stations.Where(x => x.Point.Coordinate == Position).FirstOrDefault();
            if (st != null)
            {
                double f = 0;
                if (Simulation.Coins > (Type.MaxFuel - Fuel) * Settings.CostFuel)
                {
                    Simulation.ChangeCoinsDown((Type.MaxFuel - Fuel) * Settings.CostFuel);
                    f = Type.MaxFuel - Fuel;
                    Fuel = Type.MaxFuel;
                    
                }
                else
                {
                    if (Simulation.Coins > 0)
                    {
                        Fuel += Simulation.Coins / Settings.CostFuel;
                        f = Simulation.Coins / Settings.CostFuel;
                        Simulation.ChangeCoinsDown(Simulation.Coins);
                    }
                    Tools.Message(MessageStatus.Warning, string.Format(Localization.GetText("Text4"), Id));
                }
                StatusInfo.l2.Add(f);
                Tools.Message(MessageStatus.Info, string.Format(Localization.GetText("Text5"), Id, Math.Round(f, 2)));
                UpdateStatusAfterBroken();
            }
            else
            {
                Tools.Message(MessageStatus.Warning, string.Format(Localization.GetText("Text6"), Id),true);
            }
        }

        public void Return()
        {
            if (Status == CarStatus.Broken || Status == CarStatus.Idle)
            {
                if (Simulation.Coins > Settings.CostReturn)
                {
                    Position = Simulation.Stations.Where(x => x.Id == 1).FirstOrDefault().Point.Coordinate;
                    Tools.Message(MessageStatus.Info, string.Format(Localization.GetText("Text7"), Id));
                    StatusInfo.t13++;
                    Simulation.ChangeCoinsDown(Settings.CostReturn);
                }
                else
                {
                    Tools.Message(MessageStatus.Warning, string.Format(Localization.GetText("Text8"), Id), true);
                }
            }
            else
            {
                Tools.Message(MessageStatus.Warning, string.Format(Localization.GetText("Text9"), Id), true);
            }
        }

        public void UnLoadContainer(Container container)
        {
            if (container.Point == Simulation.Graph.FindingTheNearestPoint(this.Position))
            {
                container.StatusInfo.l1.Add(container.Capacity);
                StatusInfo.t12++;
                if ((Type.LimitCapacity - Capacity) >= container.Capacity)
                {
                    Capacity += container.Capacity;
                    container.Capacity = 0;
                }
                else
                {
                    Tools.Message(MessageStatus.Warning, string.Format(Localization.GetText("Text10"), Id), true);
                    container.Capacity -= Type.LimitCapacity - Capacity;
                    Capacity = Type.LimitCapacity;
                }
            }
        }

        //public void Run(Path path)
        //{
        //    if (Status == CarStatus.Idle)
        //    {
                
        //    }
        //}

        private void CurrentPath_OnEndedPath(Path path)
        {
            
            //if (Simulation.Graph.FindingTheNearestPoint(Position) == path.NodeEnd)
            if(Position == path.NodeEnd.Coordinate)
            {
                Status = CarStatus.Idle;
                InteractObject(path.NodeEnd);
            }
            //currentPath = null;

        }

        private void InteractObject(Node node)
        {
            var cont = Simulation.Containers.Where(x => x.Point == node).FirstOrDefault();
            if (cont != null)
            {
                UnLoadContainer(cont);
            }
            var s = Simulation.Stations.Where(x => x.Point == node).FirstOrDefault();
            if (s != null)
            {
                s.Interact(this);
            }
            var d = Simulation.Dumps.Where(x => x.Point == node).FirstOrDefault();
            if (d != null)
            {
                d.Interact(this);
            }
        }

        public void Run(Node node)
        {
            if (Status == CarStatus.Idle)
            {
                var n = Simulation.Graph.FindingTheNearestPoint(Position);
                Position = n.Coordinate;
                var path = Simulation.Graph.NewPath(n, node);
                if (path != null)
                {
                    currentPath = path;
                    currentPath.OnEndedPath += CurrentPath_OnEndedPath;
                    Status = CarStatus.Run;
                }
                else
                {
                    if (n == node)
                    {
                        InteractObject(node);
                    }
                }
            }
            else
            {
                Tools.Message(MessageStatus.Warning, string.Format(Localization.GetText("Text59"),Id),true);
            }
        }

        public void GameCicle(double delta)
        {
            if ((Status == CarStatus.Run))
            {
                StatusInfo.t1 += delta;
                var pos = currentPath?.NextStep(Position, delta * Type.SpeedCar) ?? Position;
                double len = Position.Lenght(pos);
                StatusInfo.t2 += len;
                Position = pos;
                Fuel -= len * Type.FuelConsumption;
                Wearout += len * Settings.WearRate;
            }
        }
    }
}
