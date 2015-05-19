using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ClientNode
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string partialPathToConfig;
        void App_Startup(object sender, StartupEventArgs e)
        {
            
            try
            {
                partialPathToConfig = Environment.GetCommandLineArgs()[1];
            }
            catch
            {

            }
        }
    }
}
