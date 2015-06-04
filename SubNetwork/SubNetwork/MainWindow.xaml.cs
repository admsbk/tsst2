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
        string pathToConfig;
        string pathToTopology;
        AdministrativeDomain AD;

        public MainWindow()
        {
            InitializeComponent();
            AD = new AdministrativeDomain(this);

            string conf = SubNetwork.App.partialPathToConfig;
            string topology = SubNetwork.App.partialPathToTopology;

            if (conf != null && topology!=null)
            {
                pathToConfig = @"" + conf;
                pathToTopology = @"" + topology;
                start();
            }
        }

        private void start()
        {
            AD.readConfig(pathToConfig, pathToTopology);
            AD.startService();
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
            start();

        }

        private void netTopologyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopologyView topologyView = new TopologyView(AD.manager.Topology);
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
