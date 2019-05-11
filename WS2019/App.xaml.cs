using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using libgameobject;

namespace WS2019
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Simulation.LoadSettings();
            //if (!Simulation.LoadSettings())
            //{
            //    MessageBox.Show("Призагрузке данных произошла ошибка");
            //    this.Shutdown();
            //}
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
            this.Shutdown();
        }
    }
}
