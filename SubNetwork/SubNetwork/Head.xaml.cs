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
using System.Windows.Shapes;

namespace SubNetwork
{
    /// <summary>
    /// Interaction logic for Head.xaml
    /// </summary>
    public partial class Head : Window
    {
        string pathToConfig;
        string pathToTopology;
        List<MainWindow> domains;

        public Head()
        {
            InitializeComponent();
            domains = new List<MainWindow>();
            
            string topology = SubNetwork.App.partialPathToTopology;
            for (int i = 0; i < SubNetwork.App.partialPathToConfigs.Length; i++)
            {
                string conf = SubNetwork.App.partialPathToConfigs[i];
                if (conf != null && topology != null)
                {
                    
                    pathToConfig = @"" + conf;
                    pathToTopology = @"" + topology;
                    MainWindow wind = new MainWindow(pathToConfig, pathToTopology);
                    domains.Add(wind);
                    wind.Show();
                }
            }
        }

        private void Load_Conf_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".xml";
            dlg.Filter = "Text documents (.xml)|*.xml";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                pathToConfig = dlg.FileName;
            }
        }
        private void Load_Topology_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".xml";
            dlg.Filter = "Text documents (.xml)|*.xml";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                pathToTopology = dlg.FileName;

            }
        }

        private void Start_Click(object sender, EventArgs e)
        {
            //start();

        }

        private void netTopologyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Topology> topologies = new List<Topology>();
            for (int i = 0; i < domains.Count; i++ )
                topologies.Add(domains[i].getTopology());
            TopologyView topologyView = new TopologyView(pathToTopology);
            topologyView.Show();
        }
        private void About_Click(object sender, EventArgs e)
        {
            AboutAuthors about = new AboutAuthors();
            about.ShowDialog();

        }
        private void Exit_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("load conf clicked");

        }
    }
}
