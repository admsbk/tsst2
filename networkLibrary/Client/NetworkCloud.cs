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
        private MainWindow window;
        private Grid logs;
        private string CloudId { get; set; }
        private string CloudIP { get; set; }
        private string CloudPort { get; set; }
        private List<string> portsIn { get; set; }
        private List<string> portsOut { get; set; }
        private Config conf;
        private SwitchingBox switchBox;
        private SwitchingBox signalizationBox;
        private Dictionary<string, TcpClient> clientSockets = new Dictionary<string, TcpClient>();
        private List<TcpClient> sockests;
        transportServer.NewClientHandler reqListener;
        transportServer.NewMsgHandler msgListener;

        public NetworkCloud(ListView links, ListView nodes, Grid logs, MainWindow window)
        {
            this.links = links;
            this.logs = logs;
            this.nodes = nodes;
            this.switchBox = new SwitchingBox();
            this.signalizationBox = new SwitchingBox();
            this.window = window;
        }

        #region CloudServer
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
        public void stopServer()
        {
            foreach (KeyValuePair<string, TcpClient> entry in clientSockets)
            {
                server.endConnection(entry.Value);
            }

            server.OnNewClientRequest -= reqListener;
            server.OnNewMessageRecived -= msgListener;
            reqListener = null;
            msgListener = null;
            server.stopServer();
            server = null;
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
#endregion

        private void newClientRequest(object a, ClientArgs e)
        {

        }

        //mozna to przerobic na jakies bardziej obiektowe
        private void newMessageRecived(object a, MessageArgs e)
        {
            string getSenderId = null ;
            try
            {
                getSenderId = clientSockets.FirstOrDefault(x => x.Value == e.ID).Key;
            }
            catch { }
            if (e.message.Contains("CP") && !getSenderId.Contains("CallControl"))
            {   
                addLog(this.logs, Constants.NEW_MSG_RECEIVED + " from " + getSenderId + " " + e.message, Constants.LOG_INFO);
                    try
                    {
                        string[] msg = e.message.Split('^');
                        string forwarded = switchBox.forwardMessage(getSenderId +"%"+msg[0]+ "&" + e.message);
                        string[] getNextNode = forwarded.Split('%');
                        server.sendMessage(clientSockets[getNextNode[0]], getSenderId + "%" + getNextNode[1] + msg[1].Split('&')[1]);
                        addLog(this.logs, Constants.FORWARD_MESSAGE + " " + forwarded, Constants.LOG_INFO);
                    }
                    catch
                    {
                        addLog(this.logs, Constants.UNREACHABLE_DST + " " + switchBox.forwardMessage(getSenderId + "%" + e.message), Constants.LOG_ERROR);
                    }
            }

            else if (e.message.Split('#').Length == 1 && e.message.Split('/').Length != 2 && e.message.Split(':').Length > 1 && !getSenderId.Contains("CallControl"))
            {      
                addLog(this.logs, Constants.NEW_MSG_RECEIVED + " from " + getSenderId + " " + e.message, Constants.LOG_INFO);
                try
                {
                    string forwarded = switchBox.forwardMessage(getSenderId + "%" + e.message);
                    string[] getNextNode = forwarded.Split('%');
                    server.sendMessage(clientSockets[getNextNode[0]], getSenderId + "%" + getNextNode[1]);
                    addLog(this.logs, Constants.FORWARD_MESSAGE + " " + forwarded, Constants.LOG_INFO);
                }
                catch
                {
                    addLog(this.logs, Constants.UNREACHABLE_DST + " " + switchBox.forwardMessage(getSenderId + "%" + e.message), Constants.LOG_ERROR);
                }                    
            }

            else if (e.message.Split('#').Length == 1 && e.message.Split('/').Length == 2 && !getSenderId.Contains("CallControl"))
            {
                string[] receivedSlots = SynchronousTransportModule.getSlots(e.message.Split('/')[0]);
                if (receivedSlots != null)
                {
                    addLog(this.logs, Constants.NEW_MSG_RECEIVED + " from " + getSenderId + " " + e.message, Constants.LOG_INFO);
                    try
                    {
                        string forwarded = switchBox.forwardMessage(getSenderId + "%" + e.message);
                        addLog(this.logs, Constants.FORWARD_MESSAGE + " " + forwarded, Constants.LOG_INFO);
                        string[] getNextNode = forwarded.Split('%');
                        server.sendMessage(clientSockets[getNextNode[0]], getSenderId + "%" + getNextNode[1]);
                        addLog(this.logs, Constants.FORWARD_MESSAGE + " " + forwarded, Constants.LOG_INFO);
                    }
                    catch
                    {
                        addLog(this.logs, Constants.UNREACHABLE_DST + " " + switchBox.forwardMessage(getSenderId + "%" + e.message), Constants.LOG_ERROR);
                    }
                }
            }

            else if(getSenderId!=null)
            {
                if (getSenderId.Contains("CallControl"))
                {
                    addLog(this.logs, Constants.NEW_MSG_RECEIVED + " from " + getSenderId + " " + e.message, Constants.LOG_INFO);
                    if (getSenderId.Contains("NetworkNode") && getSenderId.Contains("rtadd"))
                    {
                        try
                        {
                            string[] getNextNode = e.message.Split('#');
                            string pdu = "";
                            for (int i = 1; i < getNextNode.Length; i++)
                                pdu += "#" + getNextNode[i];
                            server.sendMessage(clientSockets[getNextNode[0]], getSenderId + "%" + pdu);
                            addLog(this.logs, Constants.FORWARD_MESSAGE + " " + getSenderId + "%" + pdu, Constants.LOG_INFO);
                        }
                        catch
                        {
                            addLog(this.logs, Constants.UNREACHABLE_DST + " " , Constants.LOG_ERROR);
                        }
                    }
                    else
                    {
                        try
                        {
                            string[] getNextNode = e.message.Split('#');
                            string pdu = "";
                            for (int i = 1; i < getNextNode.Length; i++)
                                pdu += "#" + getNextNode[i];
                            server.sendMessage(clientSockets[getNextNode[0]], getSenderId + "%" + pdu);
                            addLog(this.logs, Constants.FORWARD_MESSAGE + " " + getSenderId + "%" + pdu, Constants.LOG_INFO);
                        }
                        catch
                        {
                            addLog(this.logs, Constants.UNREACHABLE_DST + " ", Constants.LOG_ERROR);
                        }
                    }
                }
            }
            else
            {
                addNewClient(e.message, e);
            }
        }

        private void addNewClient(string m, MessageArgs e)
        {
            try
            {
                if (clientSockets.Keys.Contains(m.Split('#')[0]))
                {
                    bool isConnected = clientSockets[(m.Split('#')[0])].Connected;
                    if (!isConnected)
                    {
                        clientSockets.Remove(m.Split('#')[0]);
                        clientSockets.Add(m.Split('#')[0], e.ID);
                        addLog(this.logs, Constants.NEW_CLIENT_LOG + " " + m.Split('#')[0], Constants.LOG_INFO);
                    }
                }
                else
                {
                    clientSockets.Add(m.Split('#')[0], e.ID);
                    addLog(this.logs, Constants.NEW_CLIENT_LOG + " " + m.Split('#')[0], Constants.LOG_INFO);
                }
            }
            catch
            {
                addLog(this.logs, Constants.ALREADY_CONNECTED + " " + e.message.Split('#')[0], Constants.LOG_ERROR);
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
                    links.Items.Add(new CrossConnection(Convert.ToString(linkNum), keyItem[0], keyItem[1],
                                                valueItem[0], valueItem[1]));
                    linkNum++;
                }

                foreach (KeyValuePair<string, string> entry in conf.controlConnection)
                {
                    this.signalizationBox.addLink(entry.Key, entry.Value);
                    window.signalizationNetwork.Items.Add(new CallControlConnection(entry.Key, entry.Value));
                }
                
                addLog(logs, networkLibrary.Constants.CONFIG_OK, networkLibrary.Constants.LOG_INFO);
            }
            catch(Exception e)
            {
                addLog(logs, networkLibrary.Constants.CONFIG_ERROR, networkLibrary.Constants.LOG_ERROR);
                Console.WriteLine(e.StackTrace);
            }
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
    }
}
