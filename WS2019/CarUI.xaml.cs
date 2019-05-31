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
    /// Логика взаимодействия для CarUI.xaml
    /// </summary>
    public partial class CarUI : UserControl
    {
        private delegate void EventDelegate();
        public Car car { set; get; }
        public CarUI(Car _car)
        {
            InitializeComponent();

            car = _car;
            progressCapacity.Maximum = car.Type.LimitCapacity;
            car.OnChangeStatus += Car_OnChangeStatus;
            car.OnChangeProperty += Car_OnChangeProperty;
            car.OnChangePosition += Car_OnChangePosition;
            textId.Text = car.Id.ToString();
            Car_OnChangePosition(car);
            Car_OnChangeStatus(car);
            Car_OnChangeProperty(car);
            image.Source = new BitmapImage(new Uri(car.Type.Image, UriKind.Relative));
            //ToolTip = new CarControlUI(car);
        }

        private void Car_OnChangePosition(Car car)
        {
            if (!Dispatcher.HasShutdownStarted)
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new EventDelegate(delegate() 
            {
                Margin = new Thickness(car.ViewPosition.X-(Width/2), car.ViewPosition.Y-(Height/2), 0, 0);
            }));
        }

        private void Car_OnChangeProperty(Car car)
        {
            if (!Dispatcher.HasShutdownStarted)
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new EventDelegate(delegate ()
                {
                progressCapacity.Value = car.Capacity;
                progressFuel.Value = car.Fuel;
                progressWear.Value = car.Wearout;
            }));
        }

        private void Car_OnChangeStatus(Car car)
        {
            if (!Dispatcher.HasShutdownStarted)
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new EventDelegate(delegate ()
                {
                switch (car.Status)
                {
                    case CarStatus.Idle:
                        {
                            imageStatus.Source = new BitmapImage(new Uri(Settings.ImageIdle, UriKind.Relative));
                            break;
                        }
                    case CarStatus.Broken:
                        {
                            imageStatus.Source = new BitmapImage(new Uri(Settings.ImageBroken, UriKind.Relative));
                            break;
                        }
                    case CarStatus.Run:
                        {
                            imageStatus.Source = new BitmapImage(new Uri(Settings.ImageRun, UriKind.Relative));
                            break;
                        }
                }
            }));
        }
    }
}
