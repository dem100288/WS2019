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
    /// Логика взаимодействия для ContainerControlUI.xaml
    /// </summary>
    public partial class ContainerControlUI : UserControl
    {
        private delegate void EventDelegate();
        private Container container;
        public ContainerControlUI(Container _container)
        {
            InitializeComponent();

            container = _container;
            textId.Text = container.Id.ToString();
            textnum.Text = Util.Localization.GetText("Text51");
            textfine.Text = Util.Localization.GetText("Text52");
            container.OnChangeProperty += Container_OnChangeProperty;
            imageCap.Source = new BitmapImage(new Uri(Settings.ImageCap, UriKind.Relative));
            Container_OnChangeProperty(container);
        }

        private void Container_OnChangeProperty(Container cont)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new EventDelegate(delegate () { 
                textCap.Text = string.Format("{0}/{1}", Math.Round(container.Capacity), Settings.LimitCapacityContainer);
                textFine.Text = string.Format("{0}", Math.Round(container.Fine,2), 100);
            }));
        }
    }
}
