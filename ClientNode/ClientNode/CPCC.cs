using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using networkLibrary;
using System.Threading;
using System.Windows.Forms;

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

        public delegate void NewConnection(object myObject, MessageArgs myArgs);
        public event NewConnection OnNewConnectionEstablished;


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
            if (e.message.Contains("CallCoordination") && !e.message.Contains("ok"))
            {
                string[] parts = e.message.Split('#');
                string response = parseMsgFromNCC(e.message);
                signalizationNetwork.sendMessage(nc.Split('%')[0] + "@CallControll#CallCoordination#" + this.myId + "#" + parts[3] + "#" + response);
            }

            else if (e.message.Contains("CallCoordination") && e.message.Contains("ok")) 
            {
                MessageArgs arg = new MessageArgs(e.message);               
                OnNewConnectionEstablished(this, arg);
            }
        }

        public void sendMessage(string msg)
        {
            signalizationNetwork.sendMessage(msg);
        }

        public string parseMsgFromNCC(string query)
        {
            
                string[] parts = query.Split('#');
                DialogResult dialogResult = MessageBox.Show(parts[2] + " is calling\nAccept?", myName + " CPCC", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    
                    logs.addLog("Caller connected " + query, true, Constants.LOG_INFO, true);
                    MessageArgs arg = new MessageArgs(query);
                    OnNewConnectionEstablished(this, arg);
                    return "ok";
                    //do something
                }
                else if (dialogResult == DialogResult.No)
                {
                    return "fail";
                    //do something else
                }
                else
                {
                    return "fail";
                }
            
           
        }


        //CALL_TEARDOWN SRC_NAME DST_NAME
        public void callTeardown(string dstName)
        {
            string srcName = myId;
            sendMessage(nc+"@CallControll#" + Constants.PARSER_CALL_TEARDOWN + "#" + srcName + "#" + dstName + "#" + Constants.CALL_TEARDOWN_REASON);
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
