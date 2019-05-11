using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Util;

namespace libgameobject
{
    public class BaseCar
    {
        protected Point position;
        protected double fuel = Settings.FuelOfBuying; 
        protected double wearout = Settings.WearoutOfBuying;
        protected double capacity = 0;
        protected CarStatus status = CarStatus.Idle;
    }
}
