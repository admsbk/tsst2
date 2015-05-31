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
        private networkLibrary.transportServer.NewMsgHandler msgHandler;
        private Dictionary<string, string> directory;
        MainWindow window;

        public AdministrativeDomain(MainWindow window)
        {
            this.window = window;
            signalization = new transportClient("localhost", "3333");
            msgHandler = new transportServer.NewMsgHandler(newMessageReceived);
            directory = new Dictionary<string, string>();
            signalization.sendMessage("NCC1#");
        }

        #region message received methods
        private void newMessageReceived(object a, MessageArgs e) 
        {
            addLog(window.logList, e.message, Constants.LOG_INFO);
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
