using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SubNetwork
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string partialPathToConfig;
        public static string partialPathToTopology;
        void App_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                partialPathToConfig = Environment.GetCommandLineArgs()[1];
                partialPathToTopology = Environment.GetCommandLineArgs()[2];
            }
            catch
            {

            }
        }
    }
}
