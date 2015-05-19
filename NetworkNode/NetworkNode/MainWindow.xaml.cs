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
using System.Windows.Threading;

namespace NetworkNode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string pathToConfig;
        Node node;


        public MainWindow()
        {
            InitializeComponent();
            string conf = NetworkNode.App.partialPathToConfig;
            node = new Node(this.log, this.links, this);
            setGraphics();

            if (conf != null)
            {
                pathToConfig = @"" + conf;
                node.readConfig(pathToConfig);
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                node.startService();
                this.startButton.IsEnabled = false;
            }

            catch
            {

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
               node.readConfig(pathToConfig);
            }
        }
        private void About_Click(object sender, EventArgs e)
        {
            /*
            AboutAuthors about = new AboutAuthors();
            about.ShowDialog();
            */
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
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {

            node.stopService();
        }

        private void links_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
