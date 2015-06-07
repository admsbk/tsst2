using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using networkLibrary;
using System.Threading;

namespace ClientNode
{
    class CPCC
    {
        private transportClient signalizationNetwork;
        private transportClient.NewMsgHandler msgListener;
        private Logs logs;
        private Dictionary<string, string> peers;
        private string myName;
        private string myId;
        public string nc;

        public CPCC(Logs logWindow, string ip, string port, string myName, string myId, string networkController)
        {
            this.signalizationNetwork = new transportClient(ip, port);
            this.logs = logWindow;
            msgListener = new transportClient.NewMsgHandler(onNewMessage);
            signalizationNetwork.OnNewMessageRecived += msgListener;
            this.myName = myName;
            this.myId = myId;
            this.nc = networkController;
            sendMessage(myId + "@" + "CallControll"+"#");
            Thread.Sleep(100);
            sendMessage(nc+"@CallControll#MyParams#"+myId+"#");
            logs.addLog("Service started correctly", true, Constants.LOG_INFO);
        }

        public void disconnect()
        {
            signalizationNetwork.stopService();
        }
        private void onNewMessage(object a, MessageArgs e)
        {
            logs.addLog(e.message, true, Constants.LOG_INFO, true);
            string response = parseMsgFromNCC(e.message);
            //signalizationNetwork.sendMessage(nc+"@CallControll#"+response);
        }

        public void sendMessage(string msg)
        {
            signalizationNetwork.sendMessage(msg);
        }

        public string parseMsgFromNCC(string query)
        {
            string[] command = query.Split('#')[1].Split(' ');
            string response = "";
            if (command[0] == "ping")
            {
                response += "pong";
            }
            else if (command[0] == "get")
            {
                if (command[1] == "log" && command.Length == 3)
                {
                    int n;
                    try { n = Int32.Parse(command[2]); }
                    catch (FormatException) { n = 0; }
                    /*
                    if (n == 0)
                        return Serial.SerializeObject(node.Log);
                    else
                        return Serial.SerializeObject(new Log(node.Log, n));*/
                }
                if (command.Length != 2)
                    return response;
                /*
                if (command[1] == "config")
                   /return Serial.SerializeObject(config);
                if (command[1] == "routing")
                    return Serial.SerializeObject(GetRoutingTable());*
                response += "getresp " + command[1];
                string[] param = command[1].Split('.');
                if (param[0] == "type")
                    response += " Sink";
                if (param[0] == "ID")
                    response += " " + node.Id;
                else if (param[0] == "Name")
                    response += " " + node.Name;
                else if (param[0] == "PortsIn")
                {
                    if (param.Length != 3 && param.Length != 5)
                        return response;
                    if (param[2] == "Open")
                        response += " " + node.PortIn.Open;
                    else if (param[2] == "Connected")
                        response += " " + node.PortIn.Connected;
                    else if (param[2] == "_port")
                        response += " " + node.PortIn.TcpPort;
                    else if (param[2] == "Available")
                    {
                        try
                        {
                            int n = Int32.Parse(param[1]);
                            int vpi = Int32.Parse(param[3]);
                            int vci = Int32.Parse(param[4]);
                            response += " " + CheckPortIn(n, vpi, vci);
                        }
                        catch (FormatException) { return response; }
                    }
                }*/
            }
            else if (command[0] == "set")
            {/*
                if (command.Length != 3)
                    return response;
                response += "setresp " + command[1];
                string[] param = command[1].Split('.');
                if (param[0] == "ID")
                    response += " " + node.Id; // parametr niezmienny
                else if (param[0] == "Name")
                {
                    node.Name = command[2];
                    response += " " + node.Name;
                }
                else if (param[0] == "PortsIn")
                {
                    if (param.Length != 3)
                        return response;
                    if (param[2] == "Open")
                        response += " " + node.PortIn.Open; // póki co niezmienne
                    else if (param[2] == "Connected")
                        response += " " + node.PortIn.Connected; // niezmienne
                    else if (param[2] == "_port")
                        response += " " + node.PortIn.TcpPort; // niezmienne
                }*/
            }
            else if (command[0] == "rtadd")
            {
                response += "rtaddresp ";
                if (command.Length == 3)
                {
                    response += command[1] + " " + command[2];
                    try
                    {
                        //RoutingEntry incoming = new RoutingEntry(command[1]);
                        //if (1 > incoming.Port)
                        //{
                          //  node.Receiver.Sources.Add(incoming, command[2]);
                            response += " ok";
                        //}
                        //else
                          //  response += " fail";
                    }
                    catch (FormatException) { response += " fail"; }
                    catch (ArgumentException) { response += " fail"; }
                }
                    
                else if (command.Length == 4)
                {
                    response += command[1] + " " + command[2] + " " + command[3];
                    try
                    {
                       // RoutingEntry incoming = new RoutingEntry(command[1]);
                        //if (1 > incoming.Port)
                        //{
                          //  node.Receiver.Sources.Add(incoming, command[3]);
                            response += " ok";
                        //}
                        //else
                          //  response += " fail";
                    }
                    catch (ArgumentException)
                    {
                        response += " fail";
                    }
                }
            }
            else if (command[0] == "rtdel")
            {
                response += "rtdelresp ";
                if (command.Length != 2)
                    return response;
                response += command[1];
                try
                {
                    //if (node.Receiver.Sources.Remove(new RoutingEntry(command[1])))
                        response += " ok";
                    //else
                        //response += " fail";
                }
                catch (FormatException) { response += " fail"; }
            }
            else
                response = command[0] + "resp";
            return response;

           
        }

        public void callRequest(string name, string capacity)
        {

            if (!peers.ContainsKey(name))
            {
                sendMessage(Constants.PARSER_CALL_REQUEST + " " + myName + " " + name + " " + capacity);
                logs.addLog("<" + name + "> " + Constants.CPCC_CALL_REQUEST, true, Constants.LOG_INFO, true);
                peers.Add(name, capacity);
            }
            else
            {
                logs.addLog(Constants.CPCC_CALL_EXISTS, true, Constants.LOG_ERROR, true);
            }
        }

        //CALL_TEARDOWN SRC_NAME DST_NAME
        public void callTeardown(string srcName, string dstName)
        {
            sendMessage(Constants.PARSER_CALL_TEARDOWN + " " + srcName + " " + dstName + " " + Constants.CALL_TEARDOWN_REASON);
            logs.addLog("<" + dstName + "> " + Constants.CPCC_CALL_TEARDOWN, true, Constants.LOG_INFO, true);
        }

        public bool checkIfPeerExist(string name)
        {
            if (peers.ContainsKey(name))
                return true;
            else
                return false;
        }
    }
}
