using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libpathgraph;
using System.Xml.Linq;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using Util;

namespace libgameobject
{
    public static class Simulation
    {
        public delegate void OnCreateCarHandle(Car car);
        public static event OnCreateCarHandle OnCreateCar;
        public delegate void OnCreateContainerHandle(Container cont);
        public static event OnCreateContainerHandle OnCreateContainer;
        public delegate void OnCreateStationHandle(Station st);
        public static event OnCreateStationHandle OnCreateStation;
        public delegate void OnCreateDumpHandle(Dump d);
        public static event OnCreateDumpHandle OnCreateDump;
        public delegate void OnChangeCoinsHandle();
        public static event OnChangeCoinsHandle OnChangeCoins;
        public delegate void OnSimulationStartHandle();
        public static event OnSimulationStartHandle OnSimulationStart;
        public delegate void OnSimulationStopHandle();
        public static event OnSimulationStopHandle OnSimulationStop;
        public delegate void OnTimeChangeHandle();
        public static event OnTimeChangeHandle OnTimeChange;
        public delegate void OnEndMonthHandle();
        public static event OnEndMonthHandle OnEndMonth;
        public delegate void OnMessageStatHandle(string mes);
        public static event OnMessageStatHandle OnMessageStat;

        private static double coins { set; get; }
        public static double Coins
        {
            set
            {
                coins = value;
                OnChangeCoins?.Invoke();
            }
            get
            {
                return coins;
            }
        }
        public static ConcurrentBag<Car> Cars { set; get; } = new ConcurrentBag<Car>();
        public static ConcurrentBag<Container> Containers { set; get; } = new ConcurrentBag<Container>();
        public static ConcurrentBag<Station> Stations { set; get; } = new ConcurrentBag<Station>();
        public static ConcurrentBag<Dump> Dumps { set; get; } = new ConcurrentBag<Dump>();
        public static SimulationInfo Statistic { set; get; } = new SimulationInfo();
        public static double SpeedSimulation { set; get; } = 1;

        public static Graph Graph => graph;
        private static Graph graph { set; get; }

        private static Thread GameThread;

        private static double timeMonth;
        private static double timeDay;
        private static double timeHour;
        public static double TimeMonth
        {
            set
            {
                int d = (int)((value / 1d) - timeMonth);
                timeMonth = value;
                if (d > 0)
                {
                    foreach (Car c in Cars)
                        //ChangeCoinsDown(d * Cars.Count * Settings.MaintenanceCostsCar);
                        ChangeCoinsDown(c.Type.MaintenanceCostsCar);
                    foreach (Container c in Containers)
                        c.PayFine();
                    OnEndMonth?.Invoke();
                    if(timeMonth >= Settings.MonthTest)
                    {
                        SimulationRun = false;
                    }
                }
                
            }
            get
            {
                return timeMonth;
            }
        }
        public static double TimeDay
        {
            set
            {
                timeDay = value;
                int d = (int)(timeDay / 30d);
                if (d > 0)
                {
                    TimeMonth += d;
                    timeDay = timeDay % 30d;
                }
            }
            get
            {
                return timeDay;
            }
        }
        public static double TimeHour
        {
            set
            {
                timeHour = value;
                int d = (int)(timeHour / 24d);
                if (d > 0)
                {
                    TimeDay += d;
                    timeHour = timeHour % 24d;
                }
                OnTimeChange?.Invoke();
            }
            get
            {
                return timeHour;
            }
        }
        private static double pastTime;
        private static double PastTime
        {
            set
            {
                pastTime = value;
                int d = (int)(pastTime / Settings.SecondPerHour);
                if (d > 0)
                {
                    TimeHour += d;
                    pastTime = pastTime % Settings.SecondPerHour;
                }
            }
            get
            {
                return pastTime;
            }
        }

        private static bool simulationRun { set; get; } = false;
        public static bool SimulationRun {
            set
            {
                if (value != simulationRun)
                {
                    simulationRun = value;
                    if (value)
                        StartSimulation();
                    else
                        StopSimulation();
                    
                }
            }
            get
            {
                return simulationRun;
            }
        }

        static Simulation()
        {
            graph = new Graph();
            
        }

        private static void GameThreadFunc()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            double l;
            double d = 1;
            //while (GameThread.ThreadState == System.Threading.ThreadState.Running)
            while(SimulationRun)
            {
                try
                {
                    l = sw.ElapsedTicks;
                    {
                        sw.Restart();
                        d = l / Settings.TickPerSecond * SpeedSimulation;
                        GameCycle(d);
                    }

                    Thread.Sleep(50);
                }
                catch(Exception ex)
                {

                }
            }
        }

        private static void GameCycle(double delta)
        {
            PastTime += delta;
            foreach (var c in Cars)
            {
                c.GameCicle(delta);
            }
            foreach (var c in Containers)
            {
                c.GameCicle(delta);
            }
        }

        private static void StartSimulation()
        {
            //if (!SimulationRun)
            {

                //GameTimer = new Timer(GameTimerTick, null, new TimeSpan(0, 0, 0), GameCycleSpan);
                //coins = StartCoins;
                //Util.ConnectionToServer.SendDataToServer("Start simulation");
                Statistic = new SimulationInfo();
                Coins = 0;
                PastTime = TimeHour = TimeDay = TimeMonth = 0;
                GameThread?.Abort();
                GameThread = new Thread(GameThreadFunc);
                GameThread.Priority = ThreadPriority.Lowest;
                GameThread.Start();
                Util.ConnectionToServer.SendDataToServer("Start simulation");
                OnSimulationStart?.Invoke();
                Tools.Message(MessageStatus.Info, Localization.GetText("Text13"));
                ChangeCoinsUp(Settings.StartCoins);
                
                
            }
            
        }

        private static void StopSimulation()
        {
            Util.ConnectionToServer.SendDataToServer("Stop simulation");

            //GameTimer = null;

            //GameThread.Interrupt();
            //GameThread.Abort();
            DisplayInfo();
            Car someItem;
            while(graph.ListPath.Count > 0) graph.ListPath.First().RemovePath();
            while (!Cars.IsEmpty)
            {
                Cars.TryTake(out someItem);
            }
            foreach (var c in Containers) c.Reset();
            Tools.Message(MessageStatus.Info, Localization.GetText("Text14"));
            OnSimulationStop?.Invoke();
        }

        private static void DisplayInfo()
        {
            double temp = 0;
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text15"), Math.Round(Coins, 2)));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text16"), Math.Round(Statistic.t1, 2)));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text17"), Math.Round(Statistic.t2, 2)));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text18"), TimeMonth));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text19"), TimeDay));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text20"), TimeHour));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text21"), Containers.Select(x => x.StatusInfo.l2.Sum()).Sum()));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text22"), Containers.Where(x => x.StatusInfo.l1.Count == 0).Count()));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text23"), Containers.Select(x => x.StatusInfo.t5).Average()));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text24"), Containers.Select(x => x.StatusInfo.t4).Max()));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text25"), Containers.Select(x => x.StatusInfo.t3).Sum()));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text26"), Containers.Select(x => x.StatusInfo.t1).Average()));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text27"), Cars.Count));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text28"), Cars.Count > 0 ? Cars.Select(x => x.StatusInfo.t1).Average() : 0));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text29"), Cars.Count > 0 ? Cars.Select(x => x.StatusInfo.t2).Average() : 0));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text30"), Cars.Count > 0 ? Cars.Select(x => x.StatusInfo.t3).Sum() : 0));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text31"), Cars.Count > 0 ? Cars.Select(x => x.StatusInfo.t8).Average() : 0));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text32"), Cars.Count > 0 ? Cars.Select(x => x.StatusInfo.t6).Sum() : 0));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text33"), Cars.Count > 0 ? Cars.Select(x => x.StatusInfo.t10).Average() : 0));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text34"), Cars.Count > 0 ? Cars.Select(x => x.Wearout).Average() : 0));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text35"), Cars.Count > 0 ? Cars.Select(x => x.StatusInfo.t5).Sum() : 0));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text36"), Cars.Count > 0 ? Cars.Select(x => x.StatusInfo.t7).Average() : 0));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text37"), Cars.Count > 0 ? Cars.Select(x => x.StatusInfo.t9).Sum() : 0));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text38"), Cars.Count > 0 ? Cars.Select(x => x.StatusInfo.t11).Sum() : 0));
            OnMessageStat?.Invoke(string.Format(Localization.GetText("Text39"), Cars.Count > 0 ? Cars.Select(x => x.StatusInfo.t13).Sum() : 0));

        }

        public static void AddContainer(int _id, int idNode)
        {
            Container cont = new Container(_id, Graph.FindNodeById(idNode));
            Containers.Add(cont);
            OnCreateContainer?.Invoke(cont);
        }

        public static void AddStation(int _id, int idNode)
        {
            Station st = new Station(_id, Graph.FindNodeById(idNode));
            Stations.Add(st);
            OnCreateStation?.Invoke(st);
        }

        public static void AddDump(int _id, int idNode)
        {
            Dump d = new Dump(_id, Graph.FindNodeById(idNode));
            Dumps.Add(d);
            OnCreateDump?.Invoke(d);
        }

        public static void ChangeCoinsUp(double _coins)
        {
            if (_coins > 0)
            {
                Coins += _coins;
                Statistic.t1 += _coins;
                Tools.Message(MessageStatus.Info, string.Format(Localization.GetText("Text40"), Math.Round(_coins, 2)));
            }
        }

        public static void ChangeCoinsDown(double _coins)
        {
            if (_coins > 0)
            {
                Coins -= _coins;
                Statistic.t2 += _coins;
                Tools.Message(MessageStatus.Info, string.Format(Localization.GetText("Text41"), Math.Round(_coins, 2)));
            }
        }

        public static void BuyCar()
        {
            BuyCar(Settings.DefaultTypeByCar);
        }

        public static void BuyCar(int idType)
        {
            var type = Settings.FindTypeById(idType);
            if (type == null) { Tools.Message(MessageStatus.Info, string.Format(Localization.GetText("Text62"), idType), true); return; }
            if (!SimulationRun) { Tools.Message(MessageStatus.Info, Localization.GetText("Text42"), true); return; }
            if (Coins < type.CostCar) { Tools.Message(MessageStatus.Info, Localization.GetText("Text43"), true); return; }
            Car car = new Car(type);
            
            int maxId = Cars.Count > 0 ? Cars.Max(c => c.Id) : 0;
            car.Id = maxId + 1;
            ChangeCoinsDown(type.CostCar);
            Cars.Add(car);
            OnCreateCar?.Invoke(car);
            Util.ConnectionToServer.SendDataToServer("Add car");
        }

        public static Station FindStationById(int id)
        {
            return Stations.FirstOrDefault(x => x.Id == id);
        }
        public static Dump FindDumpById(int id)
        {
            return Dumps.FirstOrDefault(x => x.Id == id);
        }
        public static Car FindCarById(int id)
        {
            return Cars.FirstOrDefault(x => x.Id == id);
        }
        public static Container FindContainerById(int id)
        {
            foreach (Container x in Containers)
            {
                if (x.Id == id) return x;
            }

            return null;
        }

        public static void LoadSettings()
        {
            XDocument settings = XDocument.Load("Settings.xml");
            foreach (var set in settings.Root.Elements())
            {
                switch (set.Name.LocalName)
                {
                    case "graph":
                        {
                            foreach (var n in set.Element("nodes").Elements())
                            {
                                graph.AddNode(new Point(double.Parse(n.Attribute("x").Value), double.Parse(n.Attribute("y").Value)), int.Parse(n.Attribute("id").Value));
                            }
                            foreach (var l in set.Element("links").Elements())
                            {
                                graph.AddLink(graph.FindNodeById(int.Parse(l.Attribute("node1").Value)), graph.FindNodeById(int.Parse(l.Attribute("node2").Value)));
                            }
                            break;
                        }
                    case "objects":
                        {
                            foreach (var o in set.Elements())
                            {
                                if (o.Name.LocalName == "container")
                                {
                                    AddContainer(int.Parse(o.Attribute("id").Value), int.Parse(o.Attribute("node").Value));
                                }
                                if (o.Name.LocalName == "station")
                                {
                                    AddStation(int.Parse(o.Attribute("id").Value), int.Parse(o.Attribute("node").Value));
                                }
                                if (o.Name.LocalName == "dump")
                                {
                                    AddDump(int.Parse(o.Attribute("id").Value), int.Parse(o.Attribute("node").Value));
                                }
                            }
                            break;
                        }
                    case "variables":
                        {
                            foreach (var o in set.Elements())
                            {
                                Settings.LoadVar(o.Attribute("name").Value, o.Attribute("value").Value);
                            }
                            break;
                        }
                    case "local":
                        {
                            Localization.SetDitionary(set);
                            break;
                        }
                    case "typecar":
                        {
                            Settings.LoadTypeCar(set);
                            break;
                        }
                }
            }
            dfs(Graph.ListNode[0]);
            if (Used.Count != Graph.ListNode.Count) throw new Exception(Localization.GetText("Text44"));
            foreach (Node node in Graph.ListNode)
            {
                if (node.OutLinks.Count == 0 || node.InLinks.Count == 0)
                {
                    throw new Exception(Localization.GetText("Text44"));
                }
            }
            ConnectionToServer.SendJson(Graph.GetListNodes(),"Simulation","LoadNodes");
        }

        private static void dfs(Node node) {
            Used.Add(node);
            foreach (LinkNode outLink in node.OutLinks)
            {
                if (!Used.Contains(outLink.NodeEnd))
                {
                    dfs(outLink.NodeEnd);
                }
            }
        }

        private static readonly List<Node> Used = new List<Node>();
    }
}
