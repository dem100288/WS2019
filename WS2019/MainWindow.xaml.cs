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
using libpathgraph;
using libgameobject;
using Util;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace WS2019
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double IX = 1, IY = 1;
        private delegate void EventDelegate();
        public MainWindow()
        {
            InitializeComponent();
            Algorithm.Init();

            Title = Util.Localization.GetText("Title");
            tabConsole.Header = Util.Localization.GetText("Text53");
            tabStat.Header = Util.Localization.GetText("Text54");
            btnSave.Content = Util.Localization.GetText("Text55");
            textScale.Text = Util.Localization.GetText("Text56");
            textSpeed.Text = Util.Localization.GetText("Text57");
            if (Settings.ControlCar)
                BtnAddCar.Visibility = Visibility.Visible;
            else
                BtnAddCar.Visibility = Visibility.Collapsed;

            Simulation.OnCreateCar += Simulation_OnCreateCar;
            Simulation.OnCreateContainer += Simulation_OnCreateContainer;
            Simulation.OnCreateStation += Simulation_OnCreateStation;
            Simulation.OnCreateDump += Simulation_OnCreateDump;
            Simulation.OnChangeCoins += Simulation_OnChangeCoins;
            Simulation.OnSimulationStart += Simulation_OnSimulationStart;
            Simulation.OnSimulationStop += Simulation_OnSimulationStop;
            Simulation.OnTimeChange += Simulation_OnTimeChange;
            Simulation.OnMessageStat += Simulation_OnMessageStat;
            Tools.OnChangeScale += Tools_OnChangeScale;
            Tools.OnNewMessage += Tools_OnNewMessage;
            textCoins.Text = "0.00";
            var bmp = new BitmapImage(new Uri(Settings.ImageMap, UriKind.Relative));
            imageMap.Source = bmp;
            imageMap.UpdateLayout();

            IX = bmp.Width;
            IY = bmp.Height;
            Tools.ScaleX = imageMap.ActualWidth / IX;
            Tools.ScaleY = imageMap.ActualHeight / IY;
            foreach(var c in Simulation.Containers)
            {
                AddContainer(c);
                ContainerControlUI cont = new ContainerControlUI(c);
                
                panelCont.Children.Add(cont);
            }
            foreach (var s in Simulation.Stations)
            {
                AddStation(s);
            }
            foreach (var d in Simulation.Dumps)
            {
                AddDump(d);
            }
            Simulation.Graph.OnChangePath += Graph_OnChangePath;
            imageCoins.Source = new BitmapImage(new Uri(Settings.ImageCoins, UriKind.Relative));
            imageStartStop.Source = new BitmapImage(new Uri(Settings.ImageStart, UriKind.Relative));
            Speed.Minimum = Settings.MinSimulationSpeed;
            Speed.Maximum = Settings.MaxSimulationSpeed;
            Speed.Value = Settings.DefaultSimulationSpeed;
            Simulation_OnTimeChange();
            if (ConnectionToServer.TestConnectToServer())
                Tools.Message(MessageStatus.Info, Util.Localization.GetText("Text60"));
            else
                Tools.Message(MessageStatus.Error, Util.Localization.GetText("Text61"));
            comboType.ItemsSource = Settings.ListTypeCar;
            comboType.DisplayMemberPath = "Name";
        }

        private void Simulation_OnMessageStat(string mes)
        {
            if (!Dispatcher.HasShutdownStarted)
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new EventDelegate(delegate ()
                {
                    stat.Items.Add(mes);
                }));
        }

        private void Simulation_OnTimeChange()
        {
            if (!Dispatcher.HasShutdownStarted)
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new EventDelegate(delegate ()
                {
                    textTime.Text = string.Format(Util.Localization.GetText("Text58"), Simulation.TimeMonth, Simulation.TimeDay, Simulation.TimeHour);
                }));
        }

        private void Simulation_OnSimulationStop()
        {
            if (!Dispatcher.HasShutdownStarted)
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new EventDelegate(delegate ()
                {
                    panelCar.Items.Clear();
                    canCar.Children.Clear();
                    tabs.SelectedIndex = 1;
                    imageStartStop.Source = new BitmapImage(new Uri(Settings.ImageStart, UriKind.Relative));
                }));
        }

        private void Simulation_OnSimulationStart()
        {
            if (!Dispatcher.HasShutdownStarted)
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new EventDelegate(delegate ()
                {
                    stat.Items.Clear();
                    console.Items.Clear();
                    tabs.SelectedIndex = 0;
                    imageStartStop.Source = new BitmapImage(new Uri(Settings.ImageStop, UriKind.Relative));
                }));
        }

        private void Tools_OnNewMessage(MessageStatus status, string message)
        {
            if (!Dispatcher.HasShutdownStarted)
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new EventDelegate(delegate ()
                {
                    var t = new TextBlock() { Text = string.Format("{0}: {1}", Tools.DescriptinoMessageStatus(status), message) };
                    if (status == MessageStatus.Warning) t.Background = new SolidColorBrush(Colors.LightYellow);
                    if (status == MessageStatus.Error) t.Background = new SolidColorBrush(Colors.MediumVioletRed);
                    console.Items.Insert(0, t);
                }));

        }

        private void Simulation_OnCreateDump(Dump d)
        {
            AddDump(d);
        }

        private void Simulation_OnCreateStation(Station st)
        {
            AddStation(st);
        }

        private void Simulation_OnChangeCoins()
        {
            if (!Dispatcher.HasShutdownStarted)
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new EventDelegate(delegate ()
                {
                    textCoins.Text = Math.Round(Simulation.Coins,2).ToString();
                }));
        }

        private void Tools_OnChangeScale()
        {
            ReDrawPath();
        }

        private void ReDrawPath()
        {
            if (!Dispatcher.HasShutdownStarted)
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new EventDelegate(delegate ()
                {
                    canpath.Children.Clear();
                    foreach (var p in Simulation.Graph.ListPath)
                    {
                        foreach (var ln in p.ListLinks)
                        {
                            Line l = new Line();
                            l.Stroke = Brushes.LawnGreen;
                            l.StrokeThickness = 5;
                            l.X1 = ln.NodeStart.ViewCoordinate.X;
                            l.Y1 = ln.NodeStart.ViewCoordinate.Y;
                            l.X2 = ln.NodeEnd.ViewCoordinate.X;
                            l.Y2 = ln.NodeEnd.ViewCoordinate.Y;
                            canpath.Children.Add(l);
                        }
                    }
                }));
        }

        private void Graph_OnChangePath()
        {
            ReDrawPath();
        }

        private void AddStation(Station st)
        {
            if (!Dispatcher.HasShutdownStarted)
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new EventDelegate(delegate ()
                {
                    StationUI s = new StationUI(st);
                    s.OnClickStation += S_OnClickStation;
                    s.RenderTransform = Tools.ScaleObject;
                    cangraph.Children.Add(s);
                }));
        }

        private void S_OnClickStation(Station station)
        {
            SendCarToObject(station.Point);
            //throw new NotImplementedException();
        }

        private void AddContainer(Container cont)
        {
            if (!Dispatcher.HasShutdownStarted)
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new EventDelegate(delegate ()
                {
                    ContainerUI c = new ContainerUI(cont);
                    c.OnClickConteiner += C_OnClickConteiner;
                    c.RenderTransform = Tools.ScaleObject;
                    canobject.Children.Add(c);
                }));
        }

        private void SendCarToObject(Node node)
        {
            var c = (CarControlUI)panelCar.SelectedItem;
            c?.Car?.Run(node);
        }

        private void C_OnClickConteiner(Container container)
        {
            SendCarToObject(container.Point);
            //throw new NotImplementedException();
        }

        private void AddDump(Dump d)
        {
            if (!Dispatcher.HasShutdownStarted)
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new EventDelegate(delegate ()
                {
                    DumpUI c = new DumpUI(d);
                    c.OnClickDump += C_OnClickDump;
                    c.RenderTransform = Tools.ScaleObject;
                    cangraph.Children.Add(c);
                }));
        }

        private void C_OnClickDump(Dump dump)
        {
            SendCarToObject(dump.Point);
            //throw new NotImplementedException();
        }

        private void Simulation_OnCreateContainer(Container cont)
        {
            AddContainer(cont);
        }

        private void Simulation_OnCreateCar(Car car)
        {
            if (!Dispatcher.HasShutdownStarted)
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new EventDelegate(delegate ()
                {
                    CarUI c = new CarUI(car);
                    c.RenderTransform = Tools.ScaleObject;
                    canCar.Children.Add(c);
                    CarControlUI ctrl = new CarControlUI(car);
                    panelCar.Items.Add(ctrl);
                }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (comboType.SelectedItem != null)
            {
                Simulation.BuyCar((comboType.SelectedItem as TypeCar).Id);
            }
            else
            {
                Simulation.BuyCar();
            }
        }

        private void ScaleObject_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Tools.ScaleObject.ScaleX = Tools.ScaleObject.ScaleY = ScaleObject.Value;
        }

        private void Speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Simulation.SpeedSimulation = Speed.Value;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Simulation.SimulationRun = false;
            
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Simulation.SimulationRun = !Simulation.SimulationRun;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "csv file |*.csv";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (var fs = File.CreateText(sfd.FileName))
                {
                    foreach (var s in stat.Items)
                    {
                        fs.WriteLine((s as string).Replace(" - ", ";"));
                    }
                }
            }
            sfd.Dispose();
        }

        private void imageMap_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Tools.ScaleX = imageMap.ActualWidth / IX;
            Tools.ScaleY = imageMap.ActualHeight / IY;
        }
    }
}
