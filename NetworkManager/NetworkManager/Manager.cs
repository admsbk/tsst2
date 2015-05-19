using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using networkLibrary;



namespace NetworkManager
{
    class Manager
    {
        public transportServer server;
        //private ListView nodes;
        private transportServer.NewClientHandler clientHandler { get; set; }
        private CommandVerifier commandVerifier;
        private Logs logs;

        public Manager (Logs logs)
        {
            this.commandVerifier = new CommandVerifier();
            this.logs = logs;

        }
        public bool startManager(int port)
        {
            try
            {
                server = new transportServer(port);
                clientHandler = new transportServer.NewClientHandler(newClientRequest);
                server.OnNewClientRequest += clientHandler;
                logs.addLogFromOutside(Constants.MANAGER_STARTED, true, Constants.INFO);
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        private void newClientRequest(object a, ClientArgs e)
        {
            logs.addLogFromOutside(Constants.NEW_CLIENT_LOG + " NetworkNode", true, Constants.LOG_INFO);
        }

        public void stopManager()
        {
            try
            {
                foreach (TcpClient client in server.clientSocket)
                {
                    server.endConnection(client);
                }

                server.OnNewClientRequest -= clientHandler;
                clientHandler = null;
                server.stopServer();
                server = null;
            }
            catch
            {
                logs.addLogFromOutside("Problems with disconnecting", true, Constants.LOG_ERROR);
            }
        }
        public bool sendCommandToAll(string command)
        {
            bool returned = false;
            if (command != null)
            {
               if (this.server != null)
               {
                   bool validCommand = commandVerifier.verifyCommand(command);
                   if (validCommand == false)
                   {
                       logs.addLogFromOutside(networkLibrary.Constants.COMMAND+command, true, 3);
                       logs.addLogFromOutside(commandVerifier.getErrorMessage(), false, 3);

                   }

                   logs.addLogFromOutside("Sending message: " + command, true, Constants.LOG_INFO);
                   foreach (TcpClient client in server.clientSocket)
                   {
                      // try
                      // {
                           server.sendMessage(client, command);
                      // }
                       //catch
                      // {

                      // }
                   }
                   returned = true;
               }
            }
            return returned;
        }


    }
}
