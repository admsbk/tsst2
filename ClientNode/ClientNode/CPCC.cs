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
            string[] parts = e.message.Split('#');
            string response = parseMsgFromNCC(e.message);
            signalizationNetwork.sendMessage(nc + "@CallControll#ConnectionRequest#" + parts[2] +"#"+this.myId+"#"+ response);
        }

        public void sendMessage(string msg)
        {
            signalizationNetwork.sendMessage(msg);
        }

        public string parseMsgFromNCC(string query)
        {
            string[] parts = query.Split('#');
            DialogResult dialogResult = MessageBox.Show(parts[2]+ " is calling\nAccept?", myName+" CPCC", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
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
