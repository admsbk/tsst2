using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace networkLibrary
{
    public class transportServer
    {
        public ASCIIEncoding encoder;
        public NetworkStream stream;
        public TcpListener serverSocket;
        public Thread serverThread;
        //private Dictionary<TcpClient, string> clientSockets = new Dictionary<TcpClient, string>();
        public List<TcpClient> clientSocket;



        public transportServer(int port)
        {
            this.encoder = new ASCIIEncoding();
            this.clientSocket = new List<TcpClient>();
            if (serverSocket == null && serverThread == null)
            {      
                    this.serverSocket = new TcpListener(IPAddress.Any, port);
                    this.serverThread = new Thread(new ThreadStart(ListenForClients));
                    this.serverThread.Start();
                    //logs.addLog(Constants.CLOUD_STARTED_CORRECTLY, true, Constants.LOG_INFO, true);        
            }
            else
            {
                //logs.addLog(Constants.CLOUD_STARTED_ERROR, true, Constants.LOG_ERROR, true);
                //return false;
                throw new Exception("server has been started");
            }
        }
        public void ListenForClients()
        {
            
            this.serverSocket.Start();
            while (true)
            {
         
                try
                {
                    TcpClient clientSocket = this.serverSocket.AcceptTcpClient();
                    ClientArgs args = new ClientArgs();

                   // args.NodeName = networkLibrary.Constants.NEW_CLIENT_LOG;
                    
                    args.ID = clientSocket;

                    this.clientSocket.Add(clientSocket);
                    OnNewClientRequest(this, args);

                    Thread clientThread = new Thread(new ParameterizedThreadStart(ListenForMessage));
                    clientThread.Start(clientSocket);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    break;
                }
            }
        }

        protected void ListenForMessage(object client)
        {

            TcpClient clientSocket = (TcpClient)client;
            NetworkStream stream = clientSocket.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (stream.CanRead)
            {
                bytesRead = 0;
                try
                {
                    bytesRead = stream.Read(message, 0, 4096);
                    
                }
                catch
                {
                    break;
                }

                if (bytesRead == 0)
                {
                    break;
                }

                string signal = encoder.GetString(message, 0, bytesRead);
                
                MessageArgs myArgs = new MessageArgs(signal);
                myArgs.ID = clientSocket;
                OnNewMessageRecived(this, myArgs);
               
            }
            if (serverSocket != null)
            {
                try
                {
                    clientSocket.GetStream().Close();
                    clientSocket.Close();
                }
                catch
                {
                }
            } 
        }

        public delegate void NewMsgHandler(object myObject, MessageArgs myArgs);
        public event NewMsgHandler OnNewMessageRecived;

        public delegate void NewClientHandler(object myObject, ClientArgs myArgs);
        public event NewClientHandler OnNewClientRequest;

        public bool isStarted()
        {
            if (serverSocket != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void endConnection(TcpClient client)
        {
            try
            {
                client.GetStream().Close();
                client.Close();
                clientSocket.Remove(client);
            }
            catch
            {
                Console.WriteLine("Problems with disconnecting clients from cloud");
            }
        }

        public void stopServer()
        {          
            foreach (TcpClient client in clientSocket)
            {
                try
                {
                    client.GetStream().Close();
                    client.Close();
                    clientSocket.Remove(client);
                }
                catch
                {
                    Console.WriteLine("Problems with disconnecting clients from cloud");
                }
            }

            if (serverSocket != null)
            {
                try
                {
                    serverSocket.Stop();
                    
                    if (serverThread.IsAlive)
                    { serverThread.Abort(); }
                }
                catch
                {
                    Console.WriteLine("Unable to stop cloud");
                }
            }
                    
            serverSocket = null;
            serverThread = null;
        }

        public void sendMessage(TcpClient client, string msg)
        {
            if (serverSocket != null)
            {
                stream = null;                
                if (client != null)
                {
                    
                    try
                    {
                        stream = client.GetStream();
                        byte[] buffer = encoder.GetBytes(msg);
                        stream.Write(buffer, 0, buffer.Length);
                        stream.Flush();
                    }
                    catch
                    {
                        //stream.Close();

                    }
                }
            }
        }
    }
}
