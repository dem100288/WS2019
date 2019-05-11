using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace libpathgraph
{
    public static class PointExtension
    {
        public static double Lenght(this Point point1, Point point2)
        {
            var dx = point1.X - point2.X;
            var dy = point1.Y - point2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
