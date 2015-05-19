using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace NetworkManager
{
    class Logs
    {
        private ListView logList;

        public Logs(ListView logList)
        {
            this.logList = logList;
        }

        public void addLog(String log, bool time, int type)
        {
            ListViewItem item = new ListViewItem();
            switch (type)
            {
                case 0: item.ForeColor = Color.Green;
                    break;
                case 1: item.ForeColor = Color.Black;
                    break;
                case 2: item.ForeColor = Color.Blue;
                    break;
                case 3: item.ForeColor = Color.Red;
                    break;
            }
            if (time)
                item.Text = "<" + DateTime.Now.ToString("HH:mm:ss") + "> " + log;
            else
                item.Text = log;

            logList.Items.Add(item);
            logList.Items[logList.Items.Count - 1].EnsureVisible();
        }

        public void addLogFromOutside(String log, bool time, int type)
        {
            ListViewItem item = new ListViewItem();
            switch (type)
            {
                case 0: item.ForeColor = Color.Green;
                    break;
                case 1: item.ForeColor = Color.Black;
                    break;
                case 2: item.ForeColor = Color.Blue;
                    break;
                case 3: item.ForeColor = Color.Red;
                    break;
            }
            if (time)
                item.Text = "<" + DateTime.Now.ToString("HH:mm:ss") + "> " + log;
            else
                item.Text = log;

            logList.Invoke(
                new MethodInvoker(delegate()
                    {
                        logList.Items.Add(item);
                        logList.Items[logList.Items.Count - 1].EnsureVisible();
                    })
               );
             }
         }
}
