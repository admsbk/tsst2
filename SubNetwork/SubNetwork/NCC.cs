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
        private Manager manager;
        private CC ConnectionController;
        private transportClient network;

        private RC rc { get; set; }
        private RC routingController { get; set; }
        /*
        public int Port
        {
            /*
            get
            {
                if (this.socket != null)
                    return ((IPEndPoint)this.socket.LocalEndPoint).Port;
                else
                    return 0;
            }
        }*/

        public NCC(Manager manager, LRM linkResourceManager, networkLibrary.transportClient client)
        {
            this.manager = manager;
            this.network = client;
            ConnectionController = new CC(this.network);
            this.rc = new RC(manager, linkResourceManager, ConnectionController);
            
        }

        /*
        private void ProcessQuery(string recv, DirectoryEntry client)
        {
            string[] query = recv.Split(' ');
            if (query[0] == "call_request")
            // format wiadomości: call_request {calling_name} {called_name} {capacity}
            {
                if (query.Length != 4)
                {
                    client.Socket.Send(Encoding.ASCII.GetBytes(
                        String.Format("call_rejected invalid_query {0}", query[2])));
                    return;
                }
                if (query[1] == client.Name)
                {
                    if (Directory.ContainsKey(query[2]))
                    {
                        var called = Directory[query[2]];
                        int callingId = client.Id;
                        int calledId = called.Id;
                        int cap;
                        try { cap = Int32.Parse(query[3]); }
                        catch (FormatException)
                        {
                            client.Socket.Send(Encoding.ASCII.GetBytes(
                                String.Format("call_rejected invalid_query {0}", query[2])));
                            return;
                        }

                        NetworkConnection connection = rc.setupConnection(callingId, calledId, manager.GetFreeId(), cap);
                        if (connection == null)
                            client.Socket.Send(Encoding.ASCII.GetBytes(
                                String.Format("call_rejected no_resources {0}", query[2])));
                        else
                        {
                            manager.AddConnection(connection);
                            called.Socket.Send(Encoding.ASCII.GetBytes(
                                String.Format("call_pending {0} {1}", connection.Id, client.Name)));
                        }
                    }
                    else
                    {
                        client.Socket.Send(Encoding.ASCII.GetBytes(
                            String.Format("call_rejected no_target {0}", query[2])));
                    }
                }
            }
            else if (query[0] == "call_accepted")
            // format wiadomości: call_accepted {call_id} {calling_name}
            {
                int id = Int32.Parse(query[1]);
                manager.Connect(id);
                var caller = Directory[query[2]];
                caller.Socket.Send(Encoding.ASCII.GetBytes(
                    String.Format("call_finished {0} {1}", id, client.Name)));
                client.Socket.Send(Encoding.ASCII.GetBytes(
                    String.Format("call_finished {0} {1}", id, caller.Name)));
            }
            else if (query[0] == "call_teardown")
            // format wiadomości: call_teardown {call_id}
            {
                int connectionId = Int32.Parse(query[1]);
                if (client.Id == manager.Connections[connectionId].Source     // połączenie może rozłączyć jedynie
                    || client.Id == manager.Connections[connectionId].Target) // węzeł korzystający z niego
                    CallTeardown(manager.Connections[connectionId], client.Name);
            }
        }*/

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

        public NetworkConnection CallRequest(int sourceId, int targetId, int cap)
        {
            NetworkConnection connection = rc.assignRoute(sourceId, targetId, ConnectionController.GetFreeId(), cap);
            if (connection != null)
            {
                ConnectionController.AddConnection(connection);
                ConnectionController.ConnectionRequest(connection.Id);
            }
            return connection;
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
