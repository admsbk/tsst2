using networkLibrary;
using Cloud.Properties;
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
using System.Windows.Threading;
using System.Threading;

namespace Cloud
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NetworkCloud cloud;
        string pathToConfig;

        public MainWindow()
        {
            InitializeComponent();
            setGraphics();
            cloud = new NetworkCloud(this.links, this.nodes, this.logList);
            var path = @"Config/NetworkTopology.xml";


            string conf = Cloud.App.partialPathToConfig;

            if (conf != null)
            {
                pathToConfig = @"" + conf;
                cloud.readConfig(pathToConfig);
                startService();
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            startService();

        }
        private void startService()
        {

            try
            {
                cloud.startService();
                this.StartButton.IsEnabled = false;
            }
            catch
            {
                Console.WriteLine("unable to start cloud");
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                this.StartButton.IsEnabled = true;
                cloud.stopServer();
            }
            catch
            {
                Console.WriteLine("Unable to stop server");
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (cloud != null && cloud.isStarted())
            {
                cloud.stopServer(); 
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
                cloud.readConfig(pathToConfig);

            }
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
        private void setGraphics()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 2);
            timer.Tick += ((sender, e) =>
            {
                if (scroll.VerticalOffset == scroll.ScrollableHeight)
                {
                    scroll.ScrollToEnd();
                }
            });
            timer.Start();
        }
    }
}
