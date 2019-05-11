using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Util
{
    public static class Settings
    {
        public static double CostRepairs { set; get; } = 0.1;
        public static double CostFuel { set; get; } = 0.1;
        public static double MaxFuel { set; get; } = 100;
        public static double CostReturn { set; get; } = 10;
        public static double LimitWearout { set; get; } = 100;
        public static string ImageMap { set; get; } = "";
        public static string ImageCar { set; get; } = "";
        public static string ImageContainer { set; get; } = "";
        public static string ImageBroken { set; get; } = "";
        public static string ImageIdle { set; get; } = "";
        public static string ImageRun { set; get; } = "";
        public static string ImageCan { set; get; } = "";
        public static string ImageBack { set; get; } = "";
        public static string ImageWrench { set; get; } = "";
        public static string ImageCap { set; get; } = "";
        public static string ImageCoins { set; get; } = "";
        public static string ImageStart { set; get; } = "";
        public static string ImageStop { set; get; } = "";
        public static string ImageStation { set; get; } = "";
        public static string ImageDump { set; get; } = "";
        public static double StartCoins { set; get; } = 100;
        public static double CostCar { set; get; } = 100;
        public static double MinSimulationSpeed { set; get; } = 1;
        public static double MaxSimulationSpeed { set; get; } = 100;
        public static double DefaultSimulationSpeed { set; get; } = 10;
        public static double TickPerSecond { set; get; } = 10000000;
        public static double SpeedCar { set; get; } = 50;
        public static double FuelOfBuying { set; get; } = 100;
        public static double FuelConsumption { set; get; } = 0.1;
        public static double WearoutOfBuying { set; get; } = 0;
        public static double ProbabilityOfWear { set; get; } = 0.5;
        public static double WearoutEffectOnSpeed { set; get; } = 0.1;
        public static double WearRate { set; get; } = 0.05;
        public static double LimitCapacityCar { set; get; } = 500;
        public static double LimitCapacityContainer { get; set; } = 100;
        public static double KFine { set; get; } = 0.01;
        public static double MaintenanceCostsCar { set; get; } = 50;
        public static double SecondPerHour { set; get; } = 1;
        public static bool ControlCar { set; get; } = true;
        public static double MonthTest { set; get; } = 10;
        

        public static void LoadVar(string name, string value)
        {
            //определение разделителя в числах
            //Regex expr = new Regex(@"\D");
            //var m = expr.Match(value);
            //string SeparatorInFile = m?.Value ?? ".";
            string SeparatorInFile = ".";
            var separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            switch(name)
            {
                case "CostRepairs": { CostRepairs = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "CostFuel": { CostFuel = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "MaxFuel": { MaxFuel = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "CostReturn": { CostReturn = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "LimitWearout": { LimitWearout = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "map": { ImageMap = value; break; }
                case "car": { ImageCar = value; break; }
                case "broken": { ImageBroken = value; break; }
                case "cont": { ImageContainer = value; break; }
                case "idle": { ImageIdle = value; break; }
                case "run": { ImageRun = value; break; }
                case "can": { ImageCan = value; break; }
                case "back": { ImageBack = value; break; }
                case "wrench": { ImageWrench = value; break; }
                case "cap": { ImageCap = value; break; }
                case "coins": { ImageCoins = value; break; }
                case "start": { ImageStart = value; break; }
                case "stop": { ImageStop = value; break; }
                case "station": { ImageStation = value; break; }
                case "dump": { ImageDump = value; break; }
                case "StartCoins": { StartCoins = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "CostCar": { CostCar = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "MinSimulationSpeed": { MinSimulationSpeed = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "MaxSimulationSpeed": { MaxSimulationSpeed = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "DefaultSimulationSpeed": { DefaultSimulationSpeed = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "TickPerSecond": { TickPerSecond = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "SpeedCar": { SpeedCar = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "FuelOfBuying": { FuelOfBuying = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "FuelConsumption": { FuelConsumption = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "WearoutOfBuying": { WearoutOfBuying = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "ProbabilityOfWear": { ProbabilityOfWear = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "WearoutEffectOnSpeed": { WearoutEffectOnSpeed = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "WearRate": { WearRate = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "LimitCapacityCar": { LimitCapacityCar = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "LimitCapacityContainer": { LimitCapacityContainer = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "KFine": { KFine = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "MaintenanceCostsCar": { MaintenanceCostsCar = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "SecondPerHour": { SecondPerHour = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
                case "ControlCar": { ControlCar = bool.Parse(value); break; }
                case "MonthTest": { MonthTest = double.Parse(value.Replace(SeparatorInFile, separator)); break; }
            }
        }

        
    }
}
