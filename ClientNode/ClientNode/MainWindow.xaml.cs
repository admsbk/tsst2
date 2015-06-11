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
using System.Windows.Forms;

namespace ClientNode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string pathToConfig;
        Client client;
        CPCC cpcc;
        Logs log;
        private int[] connections = {0, 0, 0, 0};


        public MainWindow()
        {
            InitializeComponent();
            log = new Logs();
            client = new Client(this.chat, this.txtBlock, this);
            setChat();

            string conf = ClientNode.App.partialPathToConfig;

            if (conf != null)
            {
                pathToConfig = @""+conf;
                client.readConfig(pathToConfig);
                startService();
            }

        }

        private void newConnection(object a, MessageArgs e)
        {
            int i=0;
            do
            {
                ++i;
            }
            while (connections[i] != 0);
            connections[i] = 1;

            switch (i)
            {
                case 1:
                    
                    this.call1.Dispatcher.Invoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(() =>
                        {
                            this.call1.IsEnabled = true;
                            this.call1.Header = e.message.Split('#')[2];
                            this.Button_1.IsEnabled = true;
                        })
                    );
                    break;
                case 2:
                    this.call2.Dispatcher.Invoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(() =>
                        {
                            this.call2.IsEnabled = true;
                            this.call2.Header = e.message.Split('#')[2];
                            this.Button_2.IsEnabled = true;
                        })
                    );
                    break;
                case 3:
                    this.call3.Dispatcher.Invoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(() =>
                        {
                            this.call3.IsEnabled = true;
                            this.call3.Header = e.message.Split('#')[2];
                            this.Button_3.IsEnabled = true;
                        })
                    );
                    break;
                case 4:
                    this.call4.Dispatcher.Invoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(() =>
                        {
                            this.call4.IsEnabled = true;
                            this.call4.Header = e.message.Split('#')[2];
                            this.Button_4.IsEnabled = true;
                        })
                    );
                    break;
            }
        }

        private void setChat()
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            startService();           
        }

        private void startService()
        {
            client.startService();
            cpcc = new CPCC(log, client.CloudIP, client.CloudPort, client.name, client.nodeName, client.networkController);
            cpcc.OnNewConnectionEstablished += new CPCC.NewConnection(newConnection);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //this.chat.TextAlignment = TextAlignment.Right;
            client.sendMessage(this.toSend.Text);
            //this.chat.TextAlignment = TextAlignment.Left;
        }
        private void Load_Conf_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".xml";
            dlg.Filter = "Text documents (.xml)|*.xml";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string pathToFile = dlg.FileName; 
                client.readConfig(pathToFile);
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
            client.stopService();
        }
        private void Logs_Click(object sender, EventArgs e)
        {
            log.ShowDialog();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            client.stopService();
        }

        private void CallingButton_Click(object sender, RoutedEventArgs e)
        {
            string callingName = this.CallName.Text;
            cpcc.sendMessage(cpcc.nc+"@CallControll#CallRequest#" + client.name +"#" + callingName+"#34");
            //Proba polaczenia (tylko do kogo? NM?)

            this.chatBox.IsEnabled = true;
            this.Button_1.IsEnabled = true;
        }

    }
}
