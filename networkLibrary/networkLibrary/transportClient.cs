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
    public class transportClient 
    {
        private TcpClient client;
        private Thread clientThread;
        protected ASCIIEncoding encoder;
        protected NetworkStream stream;

        public delegate void NewMsgHandler(object myObject, MessageArgs myArgs);
        public event NewMsgHandler OnNewMessageRecived;

        public delegate void NewSignalization(object myObject, MessageArgs myArgs);
        public event NewSignalization OnNewSignalization;

        public transportClient(string ip, string port)
        {
            this.encoder = new ASCIIEncoding();

            client = new TcpClient();
            IPAddress ipAddress;
            if (ip.Contains("localhost"))
            {
                ipAddress = IPAddress.Loopback;
            }
            else
            {
                ipAddress = IPAddress.Parse(ip);
            }

            int dstPort = Convert.ToInt32(port);
            client.Connect(new IPEndPoint(ipAddress, dstPort));

            if (client.Connected)
            {
                stream = client.GetStream();
                clientThread = new Thread(new ThreadStart(ListenForMessage));
                clientThread.Start();
                //sendMessage("say hello");

            }
            else
            {
                client = null;
            }
        }

        protected void ListenForMessage()
        {
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
                Console.WriteLine(message);
                MessageArgs myArgs = new MessageArgs(signal);
                OnNewMessageRecived(this, myArgs);
            }
            if (client != null)
            {
                try
                {
                    var args = new MessageArgs("Disconnected from cloud");
                    OnNewSignalization(this, args);

                    client.GetStream().Close();
                    client.Close();
                    client = null;
                    
                }
                catch (Exception e )
                {
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine("Exception in disconnecting from cloud");
                }
            }

        }

        public void sendMessage(string msg)
        {
                byte[] buffer = encoder.GetBytes(msg);
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
        
        }

        public bool isConnected()
        {
            if (this.client!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public void stopService()
        {
            client.GetStream().Close();
            client.Close();
            clientThread.Join();
        }

    }
}
