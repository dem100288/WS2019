﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace libpathgraph
{
    public class Graph
    {

        public delegate void OnCreatePathHandle();
        public event OnCreatePathHandle OnChangePath;

        public List<Node> ListNode => listNode;
        private List<Node> listNode { set; get; }

        public List<Path> ListPath => listPath;
        private List<Path> listPath { set; get; }

        public Graph()
        {
            listNode = new List<Node>();
            listPath = new List<Path>();
        }

        public void Draw()
        {

        }

        public Path NewPath(Node start, Node end)
        {
            Path path = null;
            if (start != end)
            {
                path = Path.FindPath(start, end);
                path.OnEndedPath += Path_OnEndedPath;
                listPath.Add(path);
                OnChangePath?.Invoke();
            }
            return path;
        }

        private void Path_OnEndedPath(Path path)
        {
            listPath.Remove(path);
            OnChangePath?.Invoke();
            //TODO
            //throw new NotImplementedException();
        }

        public void AddNode(Point _point, int _id)
        {

            listNode.Add(new Node(_point,_id));
        }

        public void AddLink(Node start, Node end)
        {
            LinkNode.NewLink(start, end);
        }

        public Node FindNodeById(int id)
        {
            return listNode.FirstOrDefault(n => n.id == id);
        }

        public Node FindingTheNearestPoint(Point point)
        {
            var l = new List<Node>();
            l.AddRange(ListNode);
            l.Sort(delegate (Node node1, Node node2)
            {
                if ((node1 == null) || (node2 == null)) return 0;
                var node1lenght = point.Lenght(node1.Coordinate);
                var node2lenght = point.Lenght(node2.Coordinate);
                if (node1lenght > node2lenght) return 1;
                else if (node1lenght < node2lenght) return -1;
                else return 0;

            });
            return l.FirstOrDefault();
        }
    }
}