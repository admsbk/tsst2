using networkLibrary;
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
using System.Windows.Forms;

namespace SubNetwork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        AdministrativeDomain AD;

        public MainWindow(string confPath, string topologyPath)
        {
            InitializeComponent();
            AD = new AdministrativeDomain(this);
            start(confPath, topologyPath);

        }

        private void start(string pathToConfig, string pathToTopology)
        {
            AD.readConfig(pathToConfig, pathToTopology);
            AD.startService();
        }
        public Topology getTopology()
        {
            return AD.manager.Topology;
        }

    }
}
