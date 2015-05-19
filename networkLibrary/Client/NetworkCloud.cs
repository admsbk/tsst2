using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using networkLibrary;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Media;

namespace Cloud
{
    class NetworkCloud
    {
        transportServer server;
        private int messageNumber = 0;
        private ListView links;
        private ListView nodes;
        private Grid logs;
        private string CloudId { get; set; }
        private string CloudIP { get; set; }
        private string CloudPort { get; set; }
        private List<string> portsIn { get; set; }
        private List<string> portsOut { get; set; }
        private Config conf;
        private SwitchingBox switchBox;
        private Dictionary<string, TcpClient> clientSockets = new Dictionary<string, TcpClient>();
        private List<TcpClient> sockests;
        transportServer.NewClientHandler reqListener;
        transportServer.NewMsgHandler msgListener;

        public NetworkCloud(ListView links, ListView nodes, Grid logs)
        {
            this.links = links;
            this.logs = logs;
            this.nodes = nodes;
            this.switchBox = new SwitchingBox();

        }

        public void startService()
        {
            try
            {
                Console.WriteLine(Convert.ToInt32(this.CloudPort));
                server = new transportServer(Convert.ToInt32(this.CloudPort));
                sockests = new List<TcpClient>();
                reqListener = new transportServer.NewClientHandler(newClientRequest);
                msgListener = new transportServer.NewMsgHandler(newMessageRecived);
                server.OnNewClientRequest += reqListener;
                server.OnNewMessageRecived += msgListener;
                if (server.isStarted())
                {
                    addLog(this.logs, Constants.SERVICE_START_OK, Constants.LOG_INFO);
                }
                else
                {
                    addLog(this.logs, Constants.SERVICE_START_ERROR, Constants.LOG_ERROR);
                }
                

            }
            catch
            {
                addLog(this.logs, Constants.SERVICE_START_ERROR, Constants.LOG_ERROR);
            }
        }
        private void newClientRequest(object a, ClientArgs e)
        {

        }

        private void newMessageRecived(object a, MessageArgs e)
        {
            
            if(e.Message.Contains("COP"))
            {
                string getSenderId = clientSockets.FirstOrDefault(x => x.Value == e.ID).Key;
                addLog(this.logs, Constants.NEW_MSG_RECIVED + " from " + getSenderId + " " + e.Message, Constants.LOG_INFO);
                    try
                    {
                        string[] msg = e.Message.Split('^');
                        string forwarded = switchBox.forwardMessage(getSenderId +"%"+msg[0]+ "&" + e.Message);
                        string[] getNextNode = forwarded.Split('%');
                        server.sendMessage(clientSockets[getNextNode[0]], getSenderId + "%" + getNextNode[1] + msg[1].Split('&')[1]);
                        addLog(this.logs, Constants.FORWARD_MESSAGE + " " + forwarded, Constants.LOG_INFO);
                    }
                    catch
                    {
                        addLog(this.logs, Constants.UNREACHABLE_DST + " " + switchBox.forwardMessage(getSenderId + "%" + e.Message), Constants.LOG_ERROR);
                    }
            }
            
            else if (e.Message.Split('#').Length == 1 && e.Message.Split('/').Length != 2)
            {
                //string[] getSenderId = e.Message.Split('%');
                string getSenderId = clientSockets.FirstOrDefault(x => x.Value == e.ID).Key;
                if (e.Message.Split(':').Length > 1)
                {
                addLog(this.logs, Constants.NEW_MSG_RECIVED + " from " + getSenderId + " " + e.Message, Constants.LOG_INFO);
                    try
                    {
                        string forwarded = switchBox.forwardMessage(getSenderId + "%" + e.Message);
                        string[] getNextNode = forwarded.Split('%');
                        server.sendMessage(clientSockets[getNextNode[0]], getSenderId + "%" + getNextNode[1]);
                        addLog(this.logs, Constants.FORWARD_MESSAGE + " " + forwarded, Constants.LOG_INFO);
                    }
                    catch
                    {
                        addLog(this.logs, Constants.UNREACHABLE_DST + " " + switchBox.forwardMessage(getSenderId + "%" + e.Message), Constants.LOG_ERROR);
                    }
                }
                               
            }

            
            else if (e.Message.Split('#').Length == 1 && e.Message.Split('/').Length == 2)
            {
                string getSenderId = clientSockets.FirstOrDefault(x => x.Value == e.ID).Key;
                string[] receivedSlots = SynchronousTransportModule.getSlots(e.Message.Split('/')[0]);
                if (receivedSlots != null)
                {
                    addLog(this.logs, Constants.NEW_MSG_RECIVED + " from " + getSenderId + " " + e.Message, Constants.LOG_INFO);
                    try
                    {
                        string forwarded = switchBox.forwardMessage(getSenderId + "%" + e.Message);
                        string[] getNextNode = forwarded.Split('%');
                        server.sendMessage(clientSockets[getNextNode[0]], getSenderId + "%" + getNextNode[1]);
                        addLog(this.logs, Constants.FORWARD_MESSAGE + " " + forwarded, Constants.LOG_INFO);
                    }
                    catch
                    {
                        addLog(this.logs, Constants.UNREACHABLE_DST + " " + switchBox.forwardMessage(getSenderId + "%" + e.Message), Constants.LOG_ERROR);
                    }
                }
            }
            else
            {
                try
                {
                    if (clientSockets.Keys.Contains(e.Message.Split('#')[0]))
                    {
                        bool isConnected = clientSockets[(e.Message.Split('#')[0])].Connected;
                        if (!isConnected)
                        {
                            clientSockets.Remove(e.Message.Split('#')[0]);
                            clientSockets.Add(e.Message.Split('#')[0], e.ID);
                            addLog(this.logs, Constants.NEW_CLIENT_LOG + " " + e.Message.Split('#')[0], Constants.LOG_INFO);
                        }
                    }
                    else
                    {
                        clientSockets.Add(e.Message.Split('#')[0], e.ID);
                        addLog(this.logs, Constants.NEW_CLIENT_LOG + " " + e.Message.Split('#')[0], Constants.LOG_INFO);
                    }
                }
                catch
                {
                    addLog(this.logs, Constants.ALREADY_CONNECTED + " " + e.Message.Split('#')[0], Constants.LOG_ERROR);
                }

            }
        }

        public void readConfig(string pathToConfig)
        {
            try
            {
                int linkNum = 1;
                conf = new Config(pathToConfig, Constants.Cloud);
                this.CloudId = conf.config[0];
                this.CloudIP = conf.config[1];
                this.CloudPort = conf.config[2];
                foreach (KeyValuePair<string, string> entry in conf.switchTable)
                {
                    this.switchBox.addLink(entry.Key, entry.Value);
                    string[] keyItem = entry.Key.Split('%');
                    string[] valueItem = entry.Value.Split('%');
                    links.Items.Add(new Link(Convert.ToString(linkNum), keyItem[0], keyItem[1],
                                                valueItem[0], valueItem[1]));
                    linkNum++;
                }
                
                addLog(logs, networkLibrary.Constants.CONFIG_OK, networkLibrary.Constants.LOG_INFO);
            }
            catch(Exception e)
            {
                addLog(logs, networkLibrary.Constants.CONFIG_ERROR, networkLibrary.Constants.LOG_ERROR);
                Console.WriteLine(e.StackTrace);
            }
        }

        public void stopServer(){
            foreach(KeyValuePair<string, TcpClient> entry in clientSockets)
            {
                server.endConnection(entry.Value);
            }

            server.OnNewClientRequest -= reqListener;
            server.OnNewMessageRecived -= msgListener;
            reqListener=null;
            msgListener=null;
            server.stopServer();
            server = null;
        }

        private void addLog(Grid log, string message, int logType)
        {
            var color = Brushes.Black;

            switch (logType)
            {
                case networkLibrary.Constants.LOG_ERROR:
                    color = Brushes.Red;
                    break;
                case networkLibrary.Constants.LOG_INFO:
                    color = Brushes.Blue;
                    break;
            }

            log.Dispatcher.Invoke(
                 System.Windows.Threading.DispatcherPriority.Normal,
                     new Action(() =>
                     {
                         var t = new TextBlock();
                         t.Text = ("[" + DateTime.Now.ToString("HH:mm:ss") + "]  " +
                             message + Environment.NewLine);
                         t.Foreground = color;
                         RowDefinition gridRow = new RowDefinition();
                         gridRow.Height = new GridLength(15);
                         log.RowDefinitions.Add(gridRow);
                         Grid.SetRow(t, messageNumber);
                         messageNumber++;
                         log.Children.Add(t);

                     })
                 );
        }

        public bool isStarted()
        {
            if (server != null)
            {
                return server.isStarted();
            }
            else
            {
                return false;
            }
        }
    }
}
