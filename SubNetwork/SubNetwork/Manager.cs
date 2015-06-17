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
                    if (entry.Value.nodeSrc == sourceNode.Name && entry.Value.name == "Internal")
                    {
                        if (nodes.ContainsKey(Convert.ToInt32(match.Value)))
                        {
                            topology.AddEdge(new Topology.Link(nodes[sourceNode.Id].tnode,
                                nodes[Convert.ToInt32(match.Value)].tnode, entry.Value.portSrc,
                                entry.Value.portDst, 140, entry.Value.name));

                            topology.AddEdge(new Topology.Link(nodes[Convert.ToInt32(match.Value)].tnode,
                                nodes[sourceNode.Id].tnode,
                                entry.Value.portDst, entry.Value.portSrc, 140, entry.Value.name));
                        }
                    }

                    else if (entry.Value.nodeSrc==sourceNode.Name && entry.Value.nodeDst.Contains("Domain"))
                    {
                        if (nodes.ContainsKey(Convert.ToInt32(match.Value) * 1000))
                        {
                            topology.AddEdge(new Topology.Link(nodes[sourceNode.Id].tnode,
                                nodes[Convert.ToInt32(match.Value) * 1000].tnode, entry.Value.portSrc,
                                entry.Value.portDst, 140, entry.Value.name));

                            topology.AddEdge(new Topology.Link(nodes[Convert.ToInt32(match.Value) * 1000].tnode,
                                nodes[sourceNode.Id].tnode,
                                entry.Value.portDst, entry.Value.portSrc, 140, entry.Value.name));
                        }
                    }

                    else if (entry.Value.nodeDst == sourceNode.Name && entry.Value.nodeSrc.Contains("Domain"))
                    {
                        if (nodes.ContainsKey(Convert.ToInt32(match.Value) * 1000))
                        {
                            topology.AddEdge(new Topology.Link(nodes[sourceNode.Id].tnode,
                                nodes[Convert.ToInt32(match.Value) * 1000].tnode, entry.Value.portSrc,
                                entry.Value.portDst, 140, entry.Value.name));

                            topology.AddEdge(new Topology.Link(nodes[Convert.ToInt32(match.Value) * 1000].tnode,
                                nodes[sourceNode.Id].tnode,
                                entry.Value.portDst, entry.Value.portSrc, 140, entry.Value.name));
                        }
                    }
                    
                    
                }
            }
        }

        public void Reset()
        {
            foreach (Node node in nodes.Values)
            {
                node.Socket.Close();
            }
            nodes.Clear();
            topology.Clear();
        }


        public int ConnectedNodes
        {
            get { return nodes.Count; }
        }


        
    }
}
