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
        private string nccname;
        private Dictionary<string, string> requestsToAnswer;
        private Dictionary<string, NetworkConnection> toEstablish;
        private List<string> neighbours;
        private List<string> parents;
        private List<string> children;


        

        public AdministrativeDomain(MainWindow window)
        {
            this.window = window;
            this.connection = window.links;
            manager = new Manager();
            neighbours = new List<string>();
            parents = new List<string>();
            children = new List<string>();
            requestsToAnswer = new Dictionary<string, string>();
            toEstablish = new Dictionary<string, NetworkConnection>();
        }

        public void readConfig(string xmlConfigPath, string xmlTopologyPath)
        {
            try
            {
                networkLibrary.Config conf = new Config(xmlConfigPath, Constants.AD);
                nccname = conf.config[0];
                linkResourceManager = new LRM(conf.config[3], window.links);
                linkResourceManager.loadTopology(xmlTopologyPath);
               
                CloudIp = conf.config[1];
                this.CloudPort = conf.config[2];
                this.domain = conf.config[3];
                neighbours = conf.neighbours;
                parents = conf.parent;
                children = conf.child;
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
                signalization.sendMessage(nccname+"@CallControll"+"#");
                networkCallController = new NCC(manager, linkResourceManager, signalization, window);
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

        #region message received methods


        private void newMessageReceived(object a, MessageArgs e) 
        {
            string[] temp = e.message.Split('#');
            addLog(window.logList, e.message, Constants.LOG_INFO);
            if (temp[0].Contains("NetworkNode"))
                nodeParser(e.message);
            else if (temp[0].Contains("Client"))
                clientParser(e.message);
            else if (temp[0].Contains("NCC"))
                nccParser(e.message);
        }

        private void nodeParser(string message) 
        {
            //addLog(window.logList, "Node says: " + message, Constants.LOG_INFO);
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
            addLog(window.logList, "Client: " + message, Constants.LOG_INFO);
            
            if (message.Contains("MyParams"))
            {
                ClientCastParametersRcv(message);
            }

            else if (message.Contains("CallRequest"))
            {
                ClientCallRequest(message);
            }

            else if (message.Contains("CallCoordination"))
            {
                if (message.Contains("ok"))
                    this.callCoordinationClientAck(message);
                else if (message.Contains("fail"))
                    this.callCoordinationNack(message);
                //else
                    //this.ClientCallRequest(message);
            }       
        }

        private void ClientCastParametersRcv(string message)
        {
            string[] msg = message.Split('#');
            string pattern = @"\d+";
            this.nameCastReceived(msg[0] + Regex.Match(domain, pattern).Value + "00" + Regex.Match(msg[0], pattern).Value);

            manager.newNodeConnected(Regex.Match(domain, pattern).Value + "00" + Regex.Match(msg[0], pattern).Value, msg[0], "Type1");
            foreach (string neighbour in neighbours)
                signalization.sendMessage(neighbour + "@CallControll#DirectoryCast#" + msg[0] + "#" + domain);
        }

        private void ClientCallRequest(string message)
        {
            string[] args = message.Split('#');

            int callingId = networkCallController.DirectoryRequest(args[3]);
            int callerId = networkCallController.DirectoryRequest(message.Split('@')[0]);
            if (callingId != -1)
            {

                object[] routeOutput = networkCallController.CallRequest(message.Split('@')[0], args[3], callerId, callingId, Convert.ToInt32(args[4]));
                NetworkConnection nc = (NetworkConnection)routeOutput[1];
                string syntax = (string)routeOutput[0];

                if (syntax == "Setting up connection")
                {
                    string traceroute = "";
                    try
                    {
                        addLog(window.logList, "Routing...", Constants.LOG_INFO);
                        for (int i = 0; i < nc.Path.Count; i++)
                            traceroute += nc.Path[i].SourceRouting + "##" + nc.Path[i].TargetRouting + "->";
                        addLog(window.logList, "Traceroute: " + traceroute, Constants.LOG_INFO);
                        signalization.sendMessage(nc.Source + "@CallControll#CallCoordination#" + nc.Target + "#ok");
                    }
                    catch
                    {
                        
                        addLog(window.logList, "Traceroute failed", Constants.LOG_ERROR);
                    }
                }
                else
                {
                    addLog(window.logList, syntax, Constants.LOG_INFO);
                    requestsToAnswer.Add(message.Split('@')[0] + "#" + args[3], callerId + "#" + callingId + "#" + args[4]);
                    
                }
            }

            else
                addLog(window.logList, "Unknown called client", Constants.LOG_ERROR);
        }

        private void nccParser(string message)
        {
            if (message.Contains("DirectoryCast"))
            {
                ExternalDirectoryCast(message);
            }

            else if (message.Contains("CallCoordination"))
            {
                NccCallCoordination(message);
            } 
        }

        private void ExternalDirectoryCast(string message)
        {
            string[] msg = message.Split('#');
            string pattern = @"\d+";
            this.nameCastReceived(msg[2].Split('@')[0] + "%" + Regex.Match(msg[3], pattern).Value + "000");
            try
            {
                manager.newNodeConnected(Regex.Match(msg[3], pattern).Value + "000", "Domain" + Regex.Match(msg[3], pattern).Value, "External");
                addLog(window.logList, "New neighbour " + "Domain" + Regex.Match(msg[3], pattern).Value, Constants.LOG_INFO);
            }
            catch
            {

            }
        }

        private void nameCastReceived(string message) 
        {
            string[] czesci = message.Split('%');
            addLog(window.logList, "New Dir registration " + czesci[0].Split('@')[0] + " " + czesci[1], Constants.LOG_INFO);
            networkCallController.DirectoryRegistration(czesci[0].Split('@')[0] , Convert.ToInt32(czesci[1]));
        }

        private void NccCallCoordination(string message)
        {
            if (message.Contains("ok"))
            {
                try
                {
                    //poinformowanie klienta o ack
                    string[] msg = message.Split('#');
                    signalization.sendMessage(msg[2] + "@CallControll" + "#CallCoordination#" + msg[3] + "#" + msg[4]);

                    //parsowanie rozkazu; ustawienie wezlow
                    callCoordinationAck(message);
                }
                catch
                {
                    Console.WriteLine("Fsa");

                }
            }
            else if (message.Contains("fail"))
                this.callCoordinationNack(message);
            //call coordination od innego ncc
            else
                this.callCoordinationReceived(message);
        }

        //od innego NCC
        private void callCoordinationReceived(string message) 
        {
            int j = 0;
            string[] subMessage = message.Split('#');
            string[] ports = new string[subMessage.Length - 6];
            string senderName = subMessage[2];
            string receiver = subMessage[3];
            int cap = Convert.ToInt32(subMessage[4]);
            int dstId = networkCallController.DirectoryRequest(receiver);
            for(int i=5; i<subMessage.Length-1;i++){
                ports[j] = subMessage[i];
                j++;
            }

            object[] arguments = new object[2];
            arguments = networkCallController.CallCoordination(ports, receiver, dstId, cap);
            NetworkConnection nc = (NetworkConnection)arguments[1];

            
            foreach (var link in nc.Path)
            {
                if (link.TargetId.Contains("Domain"))
                  link.SourceRouting = link.TargetRouting;
                //else if (link.SourceId.Contains("Domain"))
                  //  link.TargetRouting = link.SourceRouting;
            }
            
            //info do klienta
            signalization.sendMessage(subMessage[3] + "@CallControll#CallCoordination#" + subMessage[2]);

            //dodanie do "bufora", czyściec dla polaczen
            this.toEstablish.Add(subMessage[3] + "@CallControll#CallCoordination#" + subMessage[2], nc);
            this.requestsToAnswer.Add(subMessage[3] + "@CallControll#CallCoordination#" + subMessage[2], subMessage[0].Split('%')[0]);
            //signalization.sendMessage(message.Split('#')[0].Split('%')[0]+"#CallCoordinationOK");
        }

        //od klienta do ncc
        private void callCoordinationClientAck(string message)
        {
            try
            {
                //przyszlo ack
                string[] msg = message.Split('#');
                string key = msg[0].Split('%')[0] + "#CallCoordination#" + msg[2];
                //wziecie z czyscca i zestawienie
                NetworkConnection nc = this.toEstablish[key];
                foreach (var link in nc.Path)
                {
                    if (link.TargetId.Contains("Domain"))
                        link.SourceRouting = link.TargetRouting;
                    //else if (link.SourceId.Contains("Domain"))
                    //  link.TargetRouting = link.SourceRouting;
                }

                if (networkCallController.Establish(nc))
                {
                    
                    addLog(window.logList, "Connected ", Constants.LOG_INFO);
                    string choosedSlot = this.toEstablish[key].Path[0].Link.Name +":"+ this.toEstablish[key].Path[0].SourceRouting.Split('.')[1];
                    string query = this.requestsToAnswer[key] + "#CallCoordination#" + msg[2] + "#" + msg[3] + "#" + msg[4] + "#" + choosedSlot;
                    signalization.sendMessage(query);
                    //mozna wyrzucic z czyscca
                    this.toEstablish.Remove(key);
                }

            }
            catch 
            {
                Console.WriteLine("Fsa");

            }
        }
        
        //od ncc do ncc
        private void callCoordinationAck(string message)
        {
            try
            {
                //przyszlo ack
                string[] msg = message.Split('#');
                //string key = msg[2] + "#CallCoordination#" + msg[2];
                //wziecie z czyscca i zestawienie
                //string choosedSlot = this.toEstablish[key].Path[0].SourceRouting;
                string choosedSlot = msg[5];

                int callingId = networkCallController.DirectoryRequest(msg[3]);
                int callerId = networkCallController.DirectoryRequest(msg[2]);
                string key = msg[2] + "#" + msg[3];
                string ConnectionArguments = this.requestsToAnswer[key];
                NetworkConnection nc = networkCallController.CallCoordinationAck(key, ConnectionArguments, choosedSlot);
                foreach (var link in nc.Path)
                {
                    if (link.TargetId.Contains("Domain"))
                        link.SourceRouting = link.TargetRouting;
                    //else if (link.SourceId.Contains("Domain"))
                       // link.TargetRouting = link.SourceRouting;
                }
                if (networkCallController.Establish(nc))
                    addLog(window.logList, "sub network connection done", Constants.LOG_INFO);
                else
                    addLog(window.logList, "subnetwork connection establish failed", Constants.LOG_ERROR);
                /*
                object[] routeOutput = networkCallController.CallRequest(msg[3], msg[2], callerId, callingId, Convert.ToInt32(msg[4]));
                NetworkConnection nc = (NetworkConnection)routeOutput[1];
                string syntax = (string)routeOutput[0];
                */
                    

            }
            catch
            {
                Console.WriteLine("powrot fail");

            }
        }

        private void callCoordinationNack(string message)
        {

        }
        private void callRequestReceived(string message) 
        {
            
        }
        #endregion

        #region frontend
        public void addLog(Grid log, string message, int logType)
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
