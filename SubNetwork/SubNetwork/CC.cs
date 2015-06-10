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
                
                if (!AddRouting(connection.Target+"", label, value, connection.Id))
                {   // ...to także
                    Disconnect(connectionId);
                    return false;
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
                    label = "CP.";
                else if (value.Contains("CP"))
                    value = "CP.";
                Query(nodeId + "@CallControll#SET%" + label+"%"+value);
                Console.WriteLine(nodeId + "@CallControll#rtadd " + value + " "+label+" " + connectionId);
                Thread.Sleep(100);
                
                return true;
               
            }
            catch { return false; }
            /*
            string[] tokens = response.Split(' ');
            if (tokens.Length == 5 && tokens[4] == "ok")
                return true;
            else
                return false;*/
            
        }
        public void Query(string query)
        {
            //if (nodes.ContainsKey(id))
            //{
               
                    network.sendMessage(query);

                    //return response;
                
                    /*
                    Topology.RemoveVertex(nodes[id].tnode);
                    nodes.Remove(id);
                    List<int> disconnected = new List<int>();
                    foreach (int con in connections.Keys)
                    {
                        if (connections[con].Nodes.Contains(id))
                            disconnected.Add(con);
                    }
                    foreach (int con in disconnected)
                        CallController.CallTeardown(connections[con], "system");
                     * */
                
            //}
            //return "";
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


        public bool AddPath(STM vpath)
        {
            string label = "";
            string value = "";
            foreach (var link in vpath.Path)
            {
                if (label != "")
                {
                    value = link.SourceRouting;
                    //AddRouting(link.SourceId, label, value, vpath.Id);
                }
                label = link.TargetRouting;
            }
            virtualPaths.Add(vpath.Id, vpath);
            return true;
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
                /*
                if (RemoveRouting(link.SourceId, connection.Id))
                    link.Link.Capacity += connection.Capacity;*/
            }
            //RemoveRouting(connection.Target, connection.Id);
            connections.Remove(connectionId);
        }



        public List<string> GetVPaths()
        {
            List<string> ret = new List<string>();
            int[] keys = new int[virtualPaths.Count];
            virtualPaths.Keys.CopyTo(keys, 0);
            Array.Sort(keys);
            foreach (int key in keys)
            {
                string item = "[" + key + "] " + virtualPaths[key].Source.Name + " -{" + virtualPaths[key].Path.Count + "}-> " + virtualPaths[key].Target.Name;
                ret.Add(item);
            }
            return ret;
        }

        void PeerCoordination() { }

    }
}
