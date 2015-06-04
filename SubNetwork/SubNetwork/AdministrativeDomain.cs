using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using networkLibrary;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace SubNetwork
{
    class AdministrativeDomain
    {
        private int messageNumber = 0;
        private transportClient signalization;
        private networkLibrary.transportClient.NewMsgHandler msgHandler;
        private NCC networkCallController;
        private LRM linkResourceManager;
        private string CloudPort;
        private string CloudIp;
        private string domain;
        MainWindow window;
        ListView connection;
        public Manager manager { get; set; }


        

        public AdministrativeDomain(MainWindow window)
        {
            this.window = window;
            this.connection = window.links;
            manager = new Manager();
        }

        public void readConfig(string xmlConfigPath, string xmlTopologyPath)
        {
            try
            {
                networkLibrary.Config conf = new Config(xmlConfigPath, Constants.AD);
                //networkCallController = new NCC(conf.config[0]);
                linkResourceManager = new LRM(conf.config[3], window.links);
                linkResourceManager.loadTopology(xmlTopologyPath);
                CloudIp = conf.config[1];
                this.CloudPort = conf.config[2];
                this.domain = conf.config[3];
                addLog(window.logList, networkLibrary.Constants.CONFIG_OK, networkLibrary.Constants.LOG_INFO);
            }
            catch (Exception e)
            {
                addLog(window.logList, networkLibrary.Constants.CONFIG_ERROR, networkLibrary.Constants.LOG_ERROR);
                Console.WriteLine(e.StackTrace);
            }


        }

        public void startService()
        {
            try
            {
                manager.Init(linkResourceManager);
                signalization = new transportClient(CloudIp, CloudPort);
                msgHandler = new transportClient.NewMsgHandler(newMessageReceived);
                signalization.OnNewMessageRecived += msgHandler;
                signalization.sendMessage("NCC1@CallControll"+"#");
            }
            catch 
            {
                addLog(window.logList, Constants.SERVICE_START_ERROR, Constants.LOG_ERROR);

            }
        }

        private void Query(string node, string query)
        {
            try
            {
                signalization.sendMessage(node+"@CallControll#"+query);
            }
            catch (SocketException) { throw new SubNetwork.Manager.NodeUnaccessibleException(); }
        }

        /*
        public string Query(int id, string query)
        {
            if (nodes.ContainsKey(id))
            {
                try
                {
                    nodes[id].Socket.Send(Encoding.ASCII.GetBytes(query));
                    byte[] buffer = new byte[16 * 1024];
                    int received = nodes[id].Socket.Receive(buffer);
                    return Encoding.ASCII.GetString(buffer, 0, received);
                }
                catch (SocketException)
                {
                    Topology.RemoveVertex(nodes[id].tnode);
                    nodes.Remove(id);
                    List<int> disconnected = new List<int>();
                    foreach (int con in connections.Keys)
                    {
                        if (connections[con].Nodes.Contains(id))
                            disconnected.Add(con);
                    }
                    foreach (int con in disconnected)
                        CallController.CallTeardown(connections[con], "system");
                }
            }
            return "";
        }*/

        #region message received methods


        private void newMessageReceived(object a, MessageArgs e) 
        {
            //addLog(window.logList, e.message, Constants.LOG_INFO);
            if (e.message.Contains("NetworkNode"))
                nodeParser(e.message);
            else if (e.message.Contains("Client"))
                clientParser(e.message);
            
        }
        private void nodeParser(string message) 
        {
            addLog(window.logList, "Node says: " + message, Constants.LOG_INFO);
            if (message.Contains("MyParams"))
            {
                string[] msg = message.Split('#');
                string pattern = @"\d+";
                Match match = Regex.Match(msg[0], pattern);
                manager.newNodeConnected(match.Value, msg[0], "Type1");

            }
        }
        private void clientParser(string message) 
        {
            addLog(window.logList, "Client says: " + message, Constants.LOG_INFO);
            if (message.Contains("MyParams"))
            {
                string[] msg = message.Split('#');
                string pattern = @"\d+";
                Match match = Regex.Match(msg[0], pattern);
                manager.newNodeConnected("100"+match.Value, msg[0], "Type1");
            }
            try
            {
                string command = message.Split('#')[0].Split('%')[1];
                switch (command)
                {
                    case "CallRequest":
                        //string callingId = networkCallController.checkClientNetAddress(e.message.Split('#')[2]);
                        addLog(window.logList, "Route from " + message.Split('#')[1] + " to " +"/"+message.Split('#')[2], Constants.LOG_INFO);
                        break;
                }
            }
            catch { }
        }
        private void nameCastReceived(string message) { }
        private void callCoordinationReceived(string message) { }
        private void callRequestReceived(string message) { }
        #endregion

        #region frontend
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
                         t.Text = ("[" + DateTime.Now.ToString("HH:mm") + "]  " +
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
        #endregion
    }
}
