using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using networkLibrary;
using System.Net.Sockets;
using System.Net;

namespace SubNetwork
{
   
    public class NCC
    {
        private class DirectoryEntry
        {
            public int Id;
            public string Name;
            public Socket Socket;
            public byte[] Buffer = new byte[4086];
        }
        private Dictionary<string, DirectoryEntry> Directory = new Dictionary<string, DirectoryEntry>();
        private Dictionary<string, NetworkConnection> connBuffer = new Dictionary<string, NetworkConnection>();
        private Manager manager;
        private CC ConnectionController;
        private transportClient network;
        private LRM lrm;

        private RC rc { get; set; }
        private RC routingController { get; set; }

        public NCC(Manager manager, LRM linkResourceManager, networkLibrary.transportClient client, MainWindow window)
        {
            this.manager = manager;
            this.network = client;
            ConnectionController = new CC(this.network, window);
            this.lrm = linkResourceManager;
            this.rc = new RC(manager, linkResourceManager, ConnectionController);
            
        }

        public void DirectoryRegistration(string name, int id)
        {
            DirectoryEntry wpis = new DirectoryEntry();
            wpis.Id = id;
            wpis.Name = name;
            Directory.Add(name, wpis);
        }

        public int DirectoryRequest(string name)
        {
            try
            {
                return Directory[name].Id;
            }
            catch
            {
                return -1;
            }
        }

        public object[] CallRequest(string srcName, string dstName, int sourceId, int targetId, int cap)
        {
            object[] toReturn = new object[2];
            NetworkConnection connection = new NetworkConnection();

            if (targetId % 1000 == 0)
            {
                //e-nni
                string portsAvailable = rc.getExternalPorts(sourceId, targetId, ConnectionController.GetFreeId(), cap); 
                network.sendMessage("NCC"+targetId/1000+"@CallControll#CallCoordination#"+srcName+"#"+dstName+"#"+cap+"#"+portsAvailable);
     
                //connBuffer.Add("CallCoordination#"+srcName+"#"+dstName, connection);
                toReturn[0] = "Waiting for Call Coordination ok";
                toReturn[1] = null;
                return toReturn;
            }

            else
            {
                //i-nni
                connection = rc.assignRoute(sourceId, targetId, ConnectionController.GetFreeId(), cap);
                Establish(connection);
                toReturn[0] = "Setting up connection";
                toReturn[1] = connection;
                return toReturn;
            }
            
        }

        public object[] CallCoordination(string[] ports, string dstName, int dstId, int cap)
        {
            object[] toReturn = new object[2];
            NetworkConnection connection = new NetworkConnection();
            int i = 0;
            for ( ; i < ports.Length; i++)
            {
                connection = rc.ExternalRequest(ports[i], dstName, dstId, ConnectionController.GetFreeId(), cap);
                if (connection != null)
                    break;
            }

            //toReturn[0] = "";
            toReturn[0]=ports[i];
            toReturn[1] = connection;
            return toReturn;

        }

        public NetworkConnection CallCoordinationAck(string srdDstName, string connArgs, string choosedSlot)
        {
            NetworkConnection connection = new NetworkConnection();
            string[] names = srdDstName.Split('#');
            string[] args = connArgs.Split('#');
            string connectionId = "1";

            connection = rc.getPathViaExternalLink(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(connectionId), Convert.ToInt32(args[2]), choosedSlot);

            return connection;
        }

        public bool Establish(NetworkConnection connection)
        {
            if (connection != null)
            {
                ConnectionController.AddConnection(connection);
                ConnectionController.ConnectionRequest(connection.Id);
                return true;
            }
            return false;
        }



        public void CallTeardown(NetworkConnection connection, string reason)
        {
            /*
            string src = manager.Get(connection.Source, "Name");
            string trg = manager.Get(connection.Target, "Name");
            Directory[src].Socket.Send(Encoding.ASCII.GetBytes(
                "call_teardown " + connection.Id + " " + reason));
            Directory[trg].Socket.Send(Encoding.ASCII.GetBytes(
                "call_teardown " + connection.Id + " " + reason));
            manager.Disconnect(connection.Id);*/
        }
    }
}
