using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using networkLibrary;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace SubNetwork
{
    class AdministrativeDomain
    {
        private int messageNumber = 0;
        private transportClient signalization;
        private networkLibrary.transportClient.NewMsgHandler msgHandler;
        private NCC networkCallController;
        MainWindow window;

        public AdministrativeDomain(MainWindow window)
        {
            this.window = window;
            signalization = new transportClient("localhost", "3333");
            networkCallController = new NCC();
            msgHandler = new transportClient.NewMsgHandler(newMessageReceived);
            signalization.OnNewMessageRecived += msgHandler;
            signalization.sendMessage("NCC1#");
        }

        #region message received methods
        private void newMessageReceived(object a, MessageArgs e) 
        {
            addLog(window.logList, e.message, Constants.LOG_INFO);
            try
            {
                string command = e.message.Split('#')[0].Split('%')[1];
                switch (command)
                {
                    case "CallRequest":
                        string callingId = networkCallController.checkClientNetAddress(e.message.Split('#')[2]);
                        addLog(window.logList, "Route from " + e.message.Split('#')[1] + " to " + callingId+"/"+e.message.Split('#')[2], Constants.LOG_INFO);
                        break;
                }
            }
            catch { }
        }
        private void nameCastReceived(string message) { }
        private void callCoordinationReceived(string message) { }
        private void callRequestReceived(string message) { }
        #endregion

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
