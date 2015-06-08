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
        public static string[] partialPathToConfigs = new string[0];
        public static string partialPathToTopology;
        void App_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                partialPathToConfigs = new string[Environment.GetCommandLineArgs().Length - 2];
                partialPathToTopology = Environment.GetCommandLineArgs()[1];
                for (int i = 2; i < partialPathToConfigs.Length+2; i++ )
                    partialPathToConfigs[i-2] = Environment.GetCommandLineArgs()[i];
            }
            catch
            {

            }
        }
    }
}
