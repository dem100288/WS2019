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
    /// Логика взаимодействия для DumpUI.xaml
    /// </summary>
    public partial class DumpUI : UserControl
    {
        private delegate void EventDelegate();
        public delegate void EventOnClickDump(Dump dump);
        public event EventOnClickDump OnClickDump;
        private Dump dump;
        public DumpUI(Dump _dump)
        {
            InitializeComponent();

            dump = _dump;
            textD.Text = Util.Localization.GetText("Text50")+" " + dump.Id;
            imageD.Source = new BitmapImage(new Uri(Settings.ImageDump, UriKind.Relative));
            Tools.OnChangeScale += Tools_OnChangeScale;
        }

        private void Tools_OnChangeScale()
        {
            Margin = new Thickness(dump.Point.ViewCoordinate.X - (Width / 2), dump.Point.ViewCoordinate.Y - (Height / 2), 0, 0);
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Settings.ControlCar)
                OnClickDump?.Invoke(dump);
        }
    }
}
