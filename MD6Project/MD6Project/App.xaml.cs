using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace MD6Project
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Startup += App_Startup;
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            MD6Project.MainWindow.Instance.Show();
        }
    }
}
