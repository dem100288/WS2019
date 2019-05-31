using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using libgameobject;

namespace WS2019
{
    public static class Algorithm
    {
        public static void Init()
        {
            Simulation.OnUpdate += OnUpdate;
        }

        public static void OnUpdate()
        {
            ConcurrentBag<Container> containers = Simulation.Containers;
            containers.First();
            Console.WriteLine("Test");
        }
    }
}