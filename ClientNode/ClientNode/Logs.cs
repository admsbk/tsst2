using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientNode
{
    public partial class Logs : Form
    {
        public Logs()
        {
            InitializeComponent();
        }


        public void addLog(string log, bool time, int flag, bool anotherThread = false)
        {
            ListViewItem item = new ListViewItem();
            switch (flag)
            {
                case 0:
                    item.ForeColor = Color.Blue;
                    break;
                case 1:
                    item.ForeColor = Color.Black;
                    break;
                case 2:
                    item.ForeColor = Color.Red;
                    break;
                case 3:
                    item.ForeColor = Color.Green;
                    break;
            }
            if (time)
                item.Text = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + log;
            else
                item.Text = log;
            if (!anotherThread)
            {
                logsListView.Items.Add(item);
                logsListView.Items[logsListView.Items.Count - 1].EnsureVisible();
            }
            else
            {
                logsListView.Invoke(
                    new MethodInvoker(delegate()
                    {
                        logsListView.Items.Add(item);
                        logsListView.Items[logsListView.Items.Count - 1].EnsureVisible();
                    })
                );
            }
        }

        public void addMsg(string msg, bool time, string name, bool isMe, bool anotherThread = false)
        {
            ListViewItem item = new ListViewItem();
            if (!isMe)
            {
                item.ForeColor = Color.Green;
            }
            else
            {
                item.ForeColor = Color.Black;
            }

            if (time)
                item.Text = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + "<" + name + "> " + msg;
            else
                item.Text = "<" + name + "> " + msg;

            if (!anotherThread)
            {
                //communicationListView.Items.Add(item);
                //communicationListView.Items[communicationListView.Items.Count - 1].EnsureVisible();
            }
            else
            {
                logsListView.Invoke(
                    new MethodInvoker(delegate()
                    {
                        //communicationListView.Items.Add(item);
                        //communicationListView.Items[communicationListView.Items.Count - 1].EnsureVisible();
                    })
                );
            }
        }
    }
}
