using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using libgameobject;
using libpathgraph;
using Util;

namespace WS2019
{
    public static class Algorithm
    {
        public static readonly ConcurrentBag<ConcurrentBag<Container>> sectors = new ConcurrentBag<ConcurrentBag<Container>>();
        
        public static void Init()
        {
            Simulation.OnUpdate += () => Task.Run(OnUpdate);
            Simulation.OnSimulationStart += () => Task.Run(OnSimulationStart);
        }

        public static void OnUpdate()
        {
            ConcurrentBag<Container> containers = Simulation.Containers;
            ConcurrentBag<Car> cars = Simulation.Cars;
            
            if (cars.IsEmpty) return;

            for (int i = 0; i < cars.Count && i < sectors.Count; i++)
            {
                Car car = cars.ElementAt(i); 
                if (car.Status == CarStatus.Idle)
                {
                    Node currentPos = Simulation.Graph.FindingTheNearestPoint(car.Position);
                    if (Simulation.Stations.First().Point == currentPos)
                    {
                        if (car.Fuel < 199) car.Refueling();
                        if (car.Wearout > 1) car.Repair();
                    }
                    
                    Container c = GetMostValuedContainer(sectors.ElementAt(i));
                    Dump d = Simulation.Dumps.ElementAt(i);

                    Node n = c.Capacity + car.Capacity > car.Type.LimitCapacity ? d.Point : c.Point;

                    Station s = Simulation.Stations.First();
                    double distance = Path.FindPath(n, currentPos).Lenght + Path.FindPath(n, s.Point).Lenght;
                    if (distance * Settings.WearRate + car.Wearout < 80)
                    {
                        car.Run(n);
                    }
                    else
                    {
                        car.Run(s.Point);
                    }
                }
                else if (car.Status == CarStatus.Broken)
                {
                    car.Return();
                    car.Refueling();
                    car.Repair();
                    Console.WriteLine("Broken");
                }
            }
        }

        private static void OnSimulationStart()
        {
            CalculateSectors();
            for (int i = 0; i < 3; i++)
            {
                Simulation.BuyCar();
            }
        }

        private static void CalculateSectors()
        {
            foreach (Dump d in Simulation.Dumps)
            {
                sectors.Add(new ConcurrentBag<Container>());
            }

            foreach (Container container in Simulation.Containers)
            {
                double minDistance = double.PositiveInfinity;
                int minIndex = 0;
                for (int i = 0; i < Simulation.Dumps.Count; i++)
                {
                    double distance = Path.FindPath(container.Point, Simulation.Dumps.ElementAt(i).Point).Lenght;
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minIndex = i;
                    }
                }
                sectors.ElementAt(minIndex).Add(container);
            }
        }

        private static Container GetMostValuedContainer(ConcurrentBag<Container> sector)
        {
            Container result = sector.First();
            foreach (Container container in sector)
            {
                if (result.Capacity < container.Capacity)
                {
                    result = container;
                }
            }

            return result;
        }
    }
}