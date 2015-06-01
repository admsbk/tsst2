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
        private LRM linkResourceManager;
        private string CloudPort;
        private string CloudIp;
        private string domain;
        MainWindow window;
        

        public AdministrativeDomain(MainWindow window)
        {
            this.window = window;
        }

        public void readConfig(string xmlConfigPath, string xmlTopologyPath)
        {
            try
            {
                networkLibrary.Config conf = new Config(xmlConfigPath, Constants.AD);
                networkCallController = new NCC(conf.config[0]);
                linkResourceManager = new LRM(conf.config[3], window);
                linkResourceManager.loadTopology(xmlTopologyPath);
                CloudIp = conf.config[1];
                this.CloudPort = conf.config[2];
                this.domain = conf.config[3];
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
                signalization = new transportClient(CloudIp, CloudPort);
                msgHandler = new transportClient.NewMsgHandler(newMessageReceived);
                signalization.OnNewMessageRecived += msgHandler;
                signalization.sendMessage(networkCallController.getNccName()+"#");
            }
            catch 
            {
                addLog(window.logList, Constants.SERVICE_START_ERROR, Constants.LOG_ERROR);

            }
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

        #region frontend
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
        #endregion
    }
}
