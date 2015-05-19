using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using networkLibrary;
using System.Xml;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace NetworkNode
{
    class Node
    {
        private MainWindow mainWindow;
        private Grid logs;
        private ListView links;
        private Config conf;

        private int messageNumber = 0;
        private int rIndex;
        private transportClient cloud;
        private transportClient manager;
        private networkLibrary.SwitchingBoxNode switchTable;
        private networkLibrary.SynchronousTransportModule[] STM;
        private List<Link> linkList;
        public transportClient.NewMsgHandler newMessageHandler { get; set; }
        public transportClient.NewMsgHandler newOrderHandler { get; set; }
        
        string ManagerIP { get; set; }
        string ManagerPort { get; set; }
        string CloudIP { get; set; }
        string CloudPort { get; set; }
        string NodeId { get; set; }
        public List<string> portsInTemp { get; set; }
        public List<string> portsOutTemp { get; set; }
        public List<Port> portsIn = new List<Port>();
        public List<Port> portsOut = new List<Port>();

        public Node(Grid logs, ListView links, MainWindow mainWindow)
        {
            this.logs = logs;
            this.links = links;
            this.mainWindow = mainWindow;
            rIndex = Grid.GetRow(logs);
            switchTable = new SwitchingBoxNode();
            linkList = new List<Link>();
        }

        private void startSending()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += ((sender, e) =>
            {
                int i=0;
                foreach (SynchronousTransportModule stm in STM)
                {
                    string tos = stm.prepareToSend().Split(':')[1];
                    if (SynchronousTransportModule.getSlots(tos) != null)
                    {
                        cloud.sendMessage(portsOutTemp[i] + "&" + stm.prepareToSend());
                        addLog(logs, stm.prepareToSend(), Constants.LOG_INFO);
                    }
                    stm.clearSTM();
                    i++;
                }
            });
            timer.Start();
        }

        private void newOrderRecived(object myObject, MessageArgs myArgs)
        {
            string[] check = myArgs.Message.Split('%');
            if (check[0] == NodeId)
            {
                addLog(logs, Constants.RECIVED_FROM_MANAGER + " " + myArgs.Message, Constants.LOG_INFO);
                if (check[1] == Constants.SET_LINK)
                    parseOrder(check[1] + "%" + check[2] + "%" + check[3]);
                else if (check[1] == Constants.DELETE_LINK)
                    parseOrder(check[1] + "%" + check[2]);
                else if(check[1]==Constants.SHOW_LINK)
                    parseOrder(check[1] + "%" + check[2]);   
            }            
        }

        private void newMessageRecived(object myObject, MessageArgs myArgs)
        {
            addLog(logs, Constants.NEW_MSG_RECIVED + " " + myArgs.Message, Constants.LOG_INFO);

            string[] fromWho = myArgs.Message.Split('%');
            if (fromWho[0].Contains("C"))
            {
                string forwarded = switchTable.forwardMessage(myArgs.Message);

                try
                {
                    string port = forwarded.Split('^')[0];
                    string slot = forwarded.Split('^')[1].Split('&')[0];
                    STM.ElementAt(portsOutTemp.IndexOf(port)).reserveSlot(Convert.ToInt32(slot), forwarded.Split('^')[1].Split('&')[1]);
                    addLog(logs, "slot reserved ", Constants.LOG_INFO);
                    addLog(logs, Constants.FORWARD_MESSAGE + " " + forwarded, Constants.LOG_INFO);
                }
                catch(Exception e)
                {
                    //addLog(logs, "slot reserved before/not empty", Constants.LOG_ERROR);
                    addLog(logs, Constants.INVALID_PORT, Constants.LOG_ERROR);
                }
            }

            else if (fromWho[1].Split('/').Length>1)
            {
                string[] slots = SynchronousTransportModule.getSlots(fromWho[1].Split(':')[1]);
                try
                {
                    int i = 0;
                    foreach (string s in slots)
                    {
                        if (s.Length != 0)
                        {
                            string forwarded = switchTable.forwardMessage(fromWho[0]+"%"+fromWho[1].Split('&')[0]+"."+i+"&"+s);
                            addLog(logs, forwarded, Constants.LOG_INFO);
                            string port = forwarded.Split('^')[0];
                            string slot = forwarded.Split('^')[1].Split('&')[0];
                            if (!port.Contains("C"))
                            {
                                STM.ElementAt(portsOutTemp.IndexOf(port)).reserveSlot(Convert.ToInt32(slot), forwarded.Split('^')[1].Split('&')[1]);
                                addLog(logs, "slot reserved ", Constants.LOG_INFO);
                                addLog(logs, Constants.FORWARD_MESSAGE + " " + forwarded, Constants.LOG_INFO);
                            }
                            else
                            {
                                cloud.sendMessage(forwarded);
                            }
                        }
                        i++;
                    }

                }
                catch
                {
                    addLog(logs, Constants.INVALID_PORT, Constants.LOG_ERROR);
                }
            }

            /*
            string[] forwarded = switchTable.forwardMessage(myArgs.Message);
            if (forwarded != null) //czyli nie przyszla pusta stmka
            { 
                foreach (string s in forwarded)
                {
                    if (s != null)
                    {
                        string[] msg = s.Split('&');
                        string[] msg1 = msg[1].Split('^');

                        if (s.Contains("CO"))
                        {
                            cloud.sendMessage(s);
                            addLog(logs, Constants.FORWARD_MESSAGE + " " + s, Constants.LOG_INFO);
                        }
                        else
                        {
                            try
                            {
                                STM.ElementAt(portsOutTemp.IndexOf(msg[0])).reserveSlot(Convert.ToInt32(msg1[0]), msg1[1]);
                                addLog(logs, "slot reserved ", Constants.LOG_INFO);
                                addLog(logs, Constants.FORWARD_MESSAGE + " " + s, Constants.LOG_INFO);
                            }
                            catch
                            {
                                //addLog(logs, "slot reserved before/not empty", Constants.LOG_ERROR);
                            }
                        }
                        
                    }
                }
            }
            else
            {
                addLog(logs, Constants.INVALID_PORT, Constants.LOG_ERROR);
            }*/
            
        }

        public void readConfig(string pathToConfig)
        {
            addLog(logs, pathToConfig, Constants.LOG_INFO);
            try
            {
                conf = new Config(pathToConfig, Constants.node);
                this.NodeId = conf.config[0];
                this.CloudIP = conf.config[1];
                this.CloudPort = conf.config[2];
                this.ManagerIP = conf.config[3];
                this.ManagerPort = conf.config[4];
                this.portsInTemp = conf.portsIn;
                this.portsOutTemp = conf.portsOut;

               
                foreach (string portIn in portsInTemp)
                {

                        Port tempPort = new Port(portIn);
                        this.portsIn.Add(tempPort);

                    
                }

                foreach (string portOut in portsOutTemp)
                {

                        Port tempPort = new Port(portOut);
                        this.portsIn.Add(tempPort);

                }
                
                this.mainWindow.Title = this.NodeId; 
                addLog(logs, networkLibrary.Constants.CONFIG_OK, networkLibrary.Constants.LOG_INFO);

                foreach (Port portIn in portsIn)
                {
                    addLog(logs,portIn.portID,Constants.TEXT);
                }
                foreach (Port portOut in portsOut)
                {
                    addLog(logs, portOut.portID, Constants.TEXT);
                }
            }

            catch(Exception e)
            {
                addLog(logs, networkLibrary.Constants.CONFIG_ERROR, networkLibrary.Constants.LOG_ERROR);
                System.Console.WriteLine(e);
            }
        }

        public void startService()
        {
            try
            {
                cloud = new transportClient(CloudIP, CloudPort);
                newMessageHandler = new transportClient.NewMsgHandler(newMessageRecived);
                cloud.OnNewMessageRecived += newMessageHandler;

                manager = new transportClient(ManagerIP, ManagerPort);
                newOrderHandler = new transportClient.NewMsgHandler(newOrderRecived);
                manager.OnNewMessageRecived += newOrderHandler;

                cloud.sendMessage(this.NodeId+"#");

                addLog(logs, Constants.SERVICE_START_OK, Constants.LOG_INFO);

                this.STM = new SynchronousTransportModule[portsOutTemp.Count];
                for (int i = 0; i < STM.Length; i++)
                {
                    this.STM[i] = new SynchronousTransportModule(3);// TUTAJ DODAC JESZCZE PARAMETR Z XMLA
                }

                startSending();
                
            }
            catch
            {
                addLog(logs, Constants.SERVICE_START_ERROR, Constants.LOG_ERROR);
                addLog(logs, Constants.CANNOT_CONNECT_TO_CLOUD, Constants.LOG_ERROR);
                addLog(logs, Constants.CANNOT_CONNECT_TO_MANAGER, Constants.LOG_ERROR);

                throw new Exception("zly start networknode");
            }
        }

        private void parseOrder(string order)
        {
            //WZOR WIADOMOSCI PRZEROBIC NA WPIS DO SWITCHING TABLE

            string[] parsed = order.Split('%');
            

            switch (parsed[0])
            {
                case Constants.SET_LINK:
                    string[] parsed1 = parsed[1].Split('.');
                    string[] parsed2 = parsed[2].Split('.');


                    if ((ifContains(parsed1[0], parsed1[1], portsIn)))
                    {
                       
                            if (switchTable.addLink(parsed1[0], parsed1[1], parsed2[0], parsed2[1]))
                            {
                                Link newLink = new Link(Convert.ToString(linkList.Count() + 1), parsed1[0], parsed1[1], parsed2[0], parsed2[1]);
                                linkList.Add(newLink);

                                Application.Current.Dispatcher.Invoke((Action)(() =>
                                {
                                    this.links.Items.Add(newLink);


                                }));
                            }
                            break;
                        
                    }
                    
                    else
                    {
                        addLog(logs, Constants.NONEXISTENT_PORT, Constants.ERROR);
                        break;
                    }
                case Constants.DELETE_LINK:
                    
                    if (parsed[1] == "*")
                    {
                        for (int i = links.Items.Count - 1; i >= 0; i--)
                        {

                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                links.Items.Remove(links.Items[i]);
                                linkList.RemoveAt(i);
                            }));
                        }
                        
                        switchTable.removeAllLinks();
                    }
                    else
                    {
                        string[] parsedX = parsed[1].Split('.');
                        switchTable.removeLink(parsedX[0],parsedX[1]);
                        for (int i = links.Items.Count - 1; i >= 0; i--)
                        {
                            if (parsed[1] == linkList[i].src)
                            {
                                links.Items.Remove(i);
                                linkList.RemoveAt(i);
                            }
                            
                        }
                    }
                    break;
            }

        }
        private void addLog(Grid log, string message, int logType)
        {
            var color = Brushes.Black;

            switch(logType){
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
        public void stopService()
        {
            if (cloud != null)
            {
                cloud.OnNewMessageRecived -= newMessageHandler;
                manager.OnNewMessageRecived -= newOrderHandler;
                newMessageHandler = null;
                newOrderHandler = null;
                cloud.stopService();
                cloud = null;
                manager.stopService();
                manager = null;
            }
        }

        public bool ifContains(string portID, string slot,List<Port> list)
        {
            foreach (Port portTemp in list)
            {
                if (portTemp.portID == portID)
                {
                   if (portTemp.portID.Contains("C"))
                       return true;
                   else if (portTemp.slots.Contains(slot))
                   {
                        return true;
                   }
                }
            }

            return false;
        }

    }
}
