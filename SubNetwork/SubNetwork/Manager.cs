using networkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SubNetwork
{
    // Ta klasa jest przeznaczona do przechowywania danych o elementach sieci na potrzeby zarzadzania.
    public class Manager
    {
        public class Node
        {
            public int Id
            {
                get { return tnode.Id; }
                set { tnode.Id = value; }
            }
            public string Name
            {
                get { return tnode.Name; }
                set { tnode.Name = value; }
            }
            public string Type
            {
                get { return tnode.Type; }
                set { tnode.Type = value; }
            }
            public Socket Socket;
            public Topology.Node tnode;
        }

        public class NodeUnaccessibleException : Exception
        {
            public int Id = -1;
            public NodeUnaccessibleException()
            { }
            public NodeUnaccessibleException(int id)
            {
                Id = id;
            }
        }

        private Dictionary<int, Node> nodes = new Dictionary<int, Node>(); // mapa numerów id na metadane węzłów
        private Topology topology = new Topology(); // topologia sieci
        public Topology Topology
        { get { return topology; } }


        public NCC CallController { get; set; }
        private LRM lrm;

        public void Init(LRM linkResourceManager)
        {
            lrm = linkResourceManager;
            //this.CallController = new NCC(this, linkResourceManager);
        }

        public void newNodeConnected(string id, string name, string type)
        {
            Node node = new Node();
            node.tnode = new Topology.Node();
            node.Id = Int32.Parse(id);
            node.Name = name.Split('@')[0];
            node.Type = type;
            
            nodes.Add(node.Id, node);
            topology.Clear();
            foreach (KeyValuePair<int, Node> n in nodes)
            {
                //AddLink(n.Value);
                topology.AddVertex(n.Value.tnode);
            }
            foreach (KeyValuePair<int, Node> n in nodes)
            {
                AddLink(n.Value);
                //topology.AddVertex(n.Value.tnode);
            }   
        }

        public void AddLink(Node sourceNode)
        {
            if (nodes.ContainsKey(sourceNode.Id))
            {
                Dictionary<string, SNPLink> polaczenia = new Dictionary<string, SNPLink>();
                polaczenia = lrm.getLinks();
                
                foreach (KeyValuePair<string, SNPLink> entry in polaczenia)
                {
                    string pattern = @"\d+";
                    Match match = Regex.Match(entry.Value.nodeDst, pattern);
                    if (entry.Value.nodeSrc == sourceNode.Name)
                    {
                        if (nodes.ContainsKey(Convert.ToInt32(match.Value)))
                        {
                            topology.AddEdge(new Topology.Link(nodes[sourceNode.Id].tnode,
                                nodes[Convert.ToInt32(match.Value)].tnode, entry.Value.portSrc,
                                entry.Value.portDst, 140));
                            
                            topology.AddEdge(new Topology.Link(nodes[Convert.ToInt32(match.Value)].tnode, 
                                nodes[sourceNode.Id].tnode, entry.Value.portSrc,
                                entry.Value.portDst, 140));

                        }
                    }
                }
            }
        }


        /*
        private Socket socket;

        public int Port
        {
            get
            {
                if (this.socket != null)
                    return ((IPEndPoint)this.socket.LocalEndPoint).Port;
                else
                    return 0;
            }
        }
        public int CCPort { get { return CallController.Port; } }
        */
        public void Reset()
        {
            foreach (Node node in nodes.Values)
            {
                node.Socket.Close();
            }
            nodes.Clear();
            topology.Clear();
        }

        // pobranie listy dostepnych elementow
        public List<string> GetElements()
        {
            List<string> ret = new List<string>();
            int[] keys = new int[nodes.Count];
            nodes.Keys.CopyTo(keys, 0);
            Array.Sort(keys);
            foreach (int key in keys)
            {
                /*
                if (Ping(key))
                {
                    string item = "[" + key + "] " + nodes[key].Name;
                    ret.Add(item);
                }*/
            }
            return ret;
        }

        public List<string> GetLinks()
        {
            List<string> ret = new List<string>();
            foreach (var link in topology.Edges)
            {
                string item = "{[" + link.Source.Id + "]" + link.Source.Name + "/" + link.SourcePort +
                    "} -> {[" + link.Target.Id + "]" + link.Target.Name + "/" + link.TargetPort + "} -- " + link.Capacity;
                ret.Add(item);
            }
            return ret;
        }


  



        /*
        public string Get(Socket sock, string param)
        {
            string response = Query(sock, "get " + param);
            string[] tokens = response.Split(' ');
            if (tokens.Length == 3 && tokens[0] == "getresp" && tokens[1] == param)
                return tokens[2];
            return "";
        }

        public string Get(int id, string param)
        {
            string response = Query(id, "get " + param);
            string[] tokens = response.Split(' ');
            if (tokens.Length == 3 && tokens[0] == "getresp" && tokens[1] == param)
                return tokens[2];
            return "";
        }

        public string Set(int id, string param, string value)
        {
            string response = Query(id, "set " + param + " " + value);
            string[] tokens = response.Split(' ');
            if (tokens.Length == 3 && tokens[0] == "setresp" && tokens[1] == param)
                return tokens[2];
            return "";
        }
        public bool Ping(int id)
        {
            string pong = Query(id, "ping");
            if (pong == "pong")
                return true;
            else
                return false;
        }
        /*
public Log GetLog(int id, int index)
{
    string response = Query(id, "get log " + index);
    if (response != "")
        return (Log)Serial.DeserializeObject(response, typeof(Log));
    else
        return null;
}

public C GetConfig(int id)
{
    string response = Query(id, "get config");
    if (response != "")
        return (C)Serial.DeserializeObject(response, typeof(C));
    else
        return null;
}

public Routing GetRouting(int id)
{
    string response = Query(id, "get routing");
    if (response != "")
        return (Routing)Serial.DeserializeObject(response, typeof(Routing));
    else
        return null;
}

        public bool AddRouting(int id, string label, string value)
        {
            string response = Query(id, "rtadd " + label + " " + value);
            string[] tokens = response.Split(' ');
            if (tokens.Length == 4 && tokens[3] == "ok")
                return true;
            else
                return false;
        }

        public bool AddRouting(int nodeId, string label, string value, int connectionId)
        {
            string response = Query(nodeId, "rtadd " + label + " " + value + " " + connectionId);
            string[] tokens = response.Split(' ');
            if (tokens.Length == 5 && tokens[4] == "ok")
                return true;
            else
                return false;
        }

        public bool RemoveRouting(int id, string label)
        {
            string response = Query(id, "rtdel " + label);
            string[] tokens = response.Split(' ');
            if (tokens.Length == 3 && tokens[2] == "ok")
                return true;
            return false;
        }

        public bool RemoveRouting(int nodeId, int connectionId)
        {
            string response = Query(nodeId, "rtdel " + connectionId);
            string[] tokens = response.Split(' ');
            if (tokens.Length == 3 && tokens[2] == "ok")
                return true;
            return false;
        }
        */
        public int ConnectedNodes
        {
            get { return nodes.Count; }
        }


        
    }
}
