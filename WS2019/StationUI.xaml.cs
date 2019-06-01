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
    /// Логика взаимодействия для StationUI.xaml
    /// </summary>
    public partial class StationUI : UserControl
    {
        private delegate void EventDelegate();
        public delegate void EventOnClickStation(Station station);
        public event EventOnClickStation OnClickStation;
        private Station station;
        public StationUI(Station _station)
        {
            InitializeComponent();

            station = _station;
            textSt.Text = Util.Localization.GetText("Text48") +" "+station.Id;
            imageSt.Source = new BitmapImage(new Uri(Settings.ImageStation, UriKind.Relative));
            Tools.OnChangeScale += Tools_OnChangeScale;
        }

        private void Tools_OnChangeScale()
        {
            Margin = new Thickness(station.Point.ViewCoordinate.X - (Width / 2), station.Point.ViewCoordinate.Y - (Height / 2), 0, 0);
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Settings.ControlCar)
                OnClickStation?.Invoke(station);
        }
    }
}
