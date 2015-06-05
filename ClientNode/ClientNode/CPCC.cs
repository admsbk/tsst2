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
            sendMessage(nc+"@CallControll#MyParams"+myId+"#");
            logs.addLog("Service started correctly", true, Constants.LOG_INFO);
        }

        public void disconnect()
        {
            signalizationNetwork.stopService();
        }
        private void onNewMessage(object a, MessageArgs e)
        {
            logs.addLog(e.message, true, Constants.LOG_INFO, true);
            parseMsgFromNCC(e.message);
        }

        public void sendMessage(string msg)
        {
            signalizationNetwork.sendMessage(msg);
        }

        public void parseMsgFromNCC(string signal)
        {
            

            /*
            if (signal.Contains(' '))
            {
                string[] words = signal.Split(' ');
                Console.WriteLine(signal);
                switch (words[0])
                {
                    //CALL_ACCEPT SRC_NET_NAME DST_NET_NAME SRC_NAME
                    case Constants.PARSER_CALL_ACCEPT:
                        Console.WriteLine("WORDS2: " + words[2]);
                        Console.WriteLine("MYID: " + myId);
                        if (words[2].Equals(myId))
                        {
                            logs.addLog("<" + words[3] + "> " + Constants.CPCC_CALL_ACCEPT, true, Constants.LOG_INFO, true);
                            //addCall(words[3]);
                        }
                        break;
                    //CALL_TEARDOWN SRC_NET_NAME DST_NET_NAME CAUSE[0 - OK, 1 - LOST] SRC_NAME DST_NAME
                    case Constants.PARSER_CALL_TEARDOWN:
                        if (words[2].Equals(myId))
                        {
                            if (words[3].Equals("0"))
                            {
                                logs.addLog("<" + words[4] + "> " + Constants.CPCC_TEARDOWN_0, true, Constants.LOG_INFO, true);
                                //deleteCall(words[4]);
                            }
                            else
                            {
                                logs.addLog("<" + words[1] + "> " + Constants.CPCC_TEARDOWN_1, true, Constants.LOG_ERROR, true);
                                logs.addLog("<" + words[1] + "> " + Constants.CPCC_RECONNECTING, true, Constants.LOG_INFO, true);
                                //callRequest(words[1], peers[words[1]]);
                            }
                        }
                        break;
                    //CALL_REQUEST_RESPONSE SRC_NET_NAME DST_NET_NAME RESPONSE[0 - OK, 1-AUTH FAILED, 2-NET FULL, 3-USER NOT FOUND] DST_NAME 
                    case Constants.PARSER_CALL_REQUEST_RESPONSE:
                        if (words[1].Equals(myId))
                        {
                            switch (words[3])
                            {
                                case "0":
                                    logs.addLog("<" + words[4] + "> " + Constants.CPCC_CALL_REQUEST_0, true, Constants.LOG_INFO, true);
                                    //addCall(words[4]);
                                    break;
                                case "1":
                                    logs.addLog("<" + words[4] + "> " + Constants.CPCC_CALL_REQUEST_1, true, Constants.LOG_ERROR, true);
                                    break;
                                case "2":
                                    logs.addLog("<" + words[4] + "> " + Constants.CPCC_CALL_REQUEST_2, true, Constants.LOG_ERROR, true);
                                    break;
                                case "3":
                                    logs.addLog("<" + words[4] + "> " + Constants.CPCC_CALL_REQUEST_3, true, Constants.LOG_ERROR, true);
                                    break;
                            }
                        }
                        break;
                }
            }*/
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
