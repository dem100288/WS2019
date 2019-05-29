using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using libgameobject;
using Util;

namespace WS2019
{
    /// <summary>
    /// Логика взаимодействия для CarControlUI.xaml
    /// </summary>
    public partial class CarControlUI : UserControl
    {
        private Car car;
        public Car Car => car;

        public CarControlUI(Car _car)
        {
            InitializeComponent();

            car = _car;
            textId.Text = car.Id.ToString();
            textnum.Text = Util.Localization.GetText("Text51");
            car.OnChangeProperty += Car_OnChangeProperty;
            car.OnChangeStatus += Car_OnChangeStatus;
            Car_OnChangeProperty(car);
            Car_OnChangeStatus(car);
            imageCan.Source = new BitmapImage(new Uri(Settings.ImageCan, UriKind.Relative));
            //imageBack.Source = new BitmapImage(new Uri(Settings.ImageBack, UriKind.Relative));
            imageWrench.Source = new BitmapImage(new Uri(Settings.ImageWrench, UriKind.Relative));
            imageCap.Source = new BitmapImage(new Uri(Settings.ImageCarCap, UriKind.Relative));
            if (Settings.ControlCar)
            {
                ControlCar.Visibility = Visibility.Visible;
                //combObj.Items.Clear();
                //foreach (var i in Simulation.Stations)
                //    combObj.Items.Add(new TextBlock() { Text = string.Format(Util.Localization.GetText("Text48") + " {0}", i.Id), Tag = i.Point.id });
                //foreach (var i in Simulation.Containers)
                //    combObj.Items.Add(new TextBlock() { Text = string.Format(Util.Localization.GetText("Text49") + " {0}", i.Id), Tag = i.Point.id });
                //foreach (var i in Simulation.Dumps)
                //    combObj.Items.Add(new TextBlock() { Text = string.Format(Util.Localization.GetText("Text50") + " {0}", i.Id), Tag = i.Point.id });
            }
            else
            {
                ControlCar.Visibility = Visibility.Collapsed;
            }
        }

        private void Car_OnChangeStatus(Car car)
        {
            Dispatcher.Invoke(() =>
            {
                switch (car.Status)
                {
                    case CarStatus.Idle:
                        {
                            imStatus.Source = new BitmapImage(new Uri(Settings.ImageIdle, UriKind.Relative));
                            break;
                        }
                    case CarStatus.Broken:
                        {
                            imStatus.Source = new BitmapImage(new Uri(Settings.ImageBroken, UriKind.Relative));
                            break;
                        }
                    case CarStatus.Run:
                        {
                            imStatus.Source = new BitmapImage(new Uri(Settings.ImageRun, UriKind.Relative));
                            break;
                        }
                }
            });
        }

        private void Car_OnChangeProperty(Car car)
        {
            Dispatcher.Invoke(() =>
            {
                textFuel.Text = string.Format("{0}/{1}", Math.Round(car.Fuel), Settings.MaxFuel);
                textWear.Text = string.Format("{0}/{1}", Math.Round(car.Wearout), Settings.LimitWearout);
                textCap.Text = string.Format("{0}/{1}", Math.Round(car.Capacity), Settings.LimitCapacityCar);
            });
        }

        private void combObj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                int idNode = (int)(e.AddedItems[0] as TextBlock).Tag;
                car.Run(Simulation.Graph.FindNodeById(idNode));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            car.Repair();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            car.Refueling();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            car.Return();
        }
    }
}
