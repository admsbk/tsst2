using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SubNetwork
{
    class CC
    {
        private Dictionary<int, NetworkConnection> connections = new Dictionary<int, NetworkConnection>();
        public Dictionary<int, NetworkConnection> Connections
        { get { return connections; } }
        private Dictionary<int, STM> virtualPaths = new Dictionary<int, STM>();
        public Dictionary<int, STM> VirtualPaths
        { get { return virtualPaths; } }
        private networkLibrary.transportClient network;
        MainWindow wind;

        public CC(networkLibrary.transportClient networkC, MainWindow wind)
        {
            this.network = networkC;
            this.wind = wind;
        }

        public bool ConnectionRequest(int connectionId)
        {
            Dictionary<string, string[]> routingEntry = new Dictionary<string, string[]>();
            if (!connections.ContainsKey(connectionId))
                return false;
            NetworkConnection connection = connections[connectionId];
            List<string> ports = new List<string>();
            if (!connections[connectionId].Active)
            {
                string label = connection.Id.ToString(); // pierwszy wpis: Id -> re
                string value = "";
                
                foreach (var link in connection.Path)
                {
                    
                    if (!routingEntry.ContainsKey(link.SourceId))
                    {
                        string[] table = new string[2];
                        table[0] = link.SourceRouting;
                        routingEntry.Add(link.SourceId, table);
                    }
                    else
                    {
                        routingEntry[link.SourceId][1] = link.SourceRouting;
                    }
                    if (!routingEntry.ContainsKey(link.TargetId))
                    {
                        string[] table = new string[2];
                        table[0] = link.TargetRouting;
                        routingEntry.Add(link.TargetId, table);
                    }
                    else
                    {
                        routingEntry[link.TargetId][1] = link.TargetRouting;
                    }

                    link.Link.Capacity -= connection.Capacity;
                    foreach (var linkDouble in connection.Path)
                    {
                        if (linkDouble.SourceId == link.TargetId && linkDouble.TargetId == link.SourceId)
                            linkDouble.Link.Capacity -= connection.Capacity;
                    }

                }

                    /*
                    value = link.SourceRouting;
                    
                    if (!AddRouting(link.SourceId, label, value, connection.Id))
                    {   // to nie powinno się zdarzyć...
                        Disconnect(connectionId);
                        return false;
                    }
                    link.Link.Capacity -= connection.Capacity;
                    label = link.TargetRouting;
                    */
                
                /*
                value = connection.Id.ToString();
                
                if (!AddRouting(connection.Target+"", label, value, connection.Id))
                {   // ...to także
                    Disconnect(connectionId);
                    return false;
                }*/

                foreach (var nn in routingEntry)
                {
                    if(nn.Value[1]!=null)
                        if (!AddRouting(nn.Key, nn.Value[0], nn.Value[1], connection.Id))
                        {   // to nie powinno się zdarzyć...
                            Disconnect(connectionId);
                            return false;
                        }
                    
                }

                connection.Active = true;
            }
            return connection.Active;
        }

        public bool Connect(int connectionId)
        {
            if (!connections.ContainsKey(connectionId))
                return false;
            NetworkConnection connection = connections[connectionId];
            if (!connections[connectionId].Active)
            {
                string label = connection.Id.ToString(); // pierwszy wpis: Id -> re
                string value = "";
                foreach (var link in connection.Path)
                {
                    value = link.SourceRouting;
                    if (!AddRouting(link.SourceId, label, value, connection.Id))
                    {   // to nie powinno się zdarzyć...
                        Disconnect(connectionId);
                        return false;
                    }
                    link.Link.Capacity -= connection.Capacity;
                    label = link.TargetRouting;
                }
                value = connection.Id.ToString();
                if (!AddRouting(connection.Target, label, value, connection.Id))
                {   // ...to także
                    Disconnect(connectionId);
                    return false;
                }
                connection.Active = true;
            }
            return connection.Active;
        }

        public bool AddRouting(int id, string label, string value)
        {
            try
            {
                Query(id + "rtadd " + label + " " + value);

                return true;
            }
            catch { return false; }
        }

        public bool AddRouting(string nodeId, string label, string value, int connectionId)
        {
            string command="";
            try
            {

                if (label.Contains("CP"))
                    label = label.Split('.')[0] + ".";
                else if (value.Contains("CP"))
                    value = value.Split('.')[0] + ".";
                Query(nodeId + "@CallControll#SET%" + label+"%"+value + "#"+connectionId);
                Console.WriteLine(nodeId + "@CallControll#rtadd " + value + " "+label+" " + connectionId);
                Thread.Sleep(100);
                
                return true;
               
            }
            catch { return false; }

            
        }
        public void Query(string query)
        { 
            network.sendMessage(query);
        }
        /*
        public List<string> GetConnections()
        {
            List<string> ret = new List<string>();
            int[] keys = new int[connections.Count];
            connections.Keys.CopyTo(keys, 0);
            Array.Sort(keys);
            foreach (int key in keys)
            {
                string item = "[" + key + "] " + nodes[connections[key].Source].Name + " --{" + connections[key].Path.Count + "}--> " + nodes[connections[key].Target].Name;
                ret.Add(item);
            }
            return ret;
        }*/

        public string GetDetails(int id)
        {
            string ret = "";
            if (virtualPaths.ContainsKey(id))
            {
                foreach (var link in virtualPaths[id].Path)
                {
                    string item = "[" + link.SourceId + "]:" + link.SourceRouting + " -> [" + link.TargetId + "]:" + link.TargetRouting;
                    ret += item + Environment.NewLine;
                }
            }
            else if (connections.ContainsKey(id))
            {
                foreach (var link in connections[id].Path)
                {
                    string item = "[" + link.SourceId + "]:" + link.SourceRouting + " -> [" + link.TargetId + "]:" + link.TargetRouting;
                    ret += item + Environment.NewLine;
                }
            }
            return ret;
        }

        public int GetFreeId()
        {
            int i = 1;
            while (connections.ContainsKey(i) || virtualPaths.ContainsKey(i))
                i++;
            return i;
        }

        public void AddConnection(NetworkConnection connection)
        {
            connections.Add(connection.Id, connection);
        }



        public void Disconnect(int connectionId)
        {
            NetworkConnection connection = connections[connectionId];
            foreach (var link in connection.Path)
            {        
                if (RemoveRouting(link.SourceId, connection.Id))
                    link.Link.Capacity += connection.Capacity;
            }
            RemoveRouting(connection.Target, connection.Id);
            connections.Remove(connectionId);
        }

        private bool RemoveRouting(string src, int connection)
        {
            //NetworkConnection c = connections[connection];
            Query(src + "@CallControll#DELETE%" + connection);
            Thread.Sleep(100);
            return true;
        }


        void PeerCoordination() { }

    }
}
