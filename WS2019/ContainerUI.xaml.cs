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
    /// Логика взаимодействия для ContainerUI.xaml
    /// </summary>
    public partial class ContainerUI : UserControl
    {
        private delegate void EventDelegate();
        public delegate void EventOnClickConteiner(Container container);
        public event EventOnClickConteiner OnClickConteiner;
        private Container container;
        public ContainerUI(Container cont)
        {
            InitializeComponent();

            container = cont;
            container.OnChangeProperty += Container_OnChangeProperty;
            container.OnFine += Container_OnFine;
            textId.Text = container.Id.ToString();
            progressCapacity.Maximum = Settings.LimitCapacityContainer;
            Container_OnChangeProperty(container);
            Tools.OnChangeScale += Tools_OnChangeScale;
            image.Source = new BitmapImage(new Uri(Settings.ImageContainer, UriKind.Relative));
        }

        private void Container_OnFine(Container cont, double fine)
        {
            Simulation.ChangeCoinsDown(fine);
        }

        private void Tools_OnChangeScale()
        {
            Margin = new Thickness(container.Point.ViewCoordinate.X - (Width / 2), container.Point.ViewCoordinate.Y - (Height / 2), 0, 0);
        }

        private void Container_OnChangeProperty(Container cont)
        {
            if (!Dispatcher.HasShutdownStarted)
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new EventDelegate(delegate () {
                progressCapacity.Value = container.Capacity;
            }));
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OnClickConteiner?.Invoke(container);
        }
    }
}
