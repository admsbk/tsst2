using QuickGraph.Algorithms.Observers;
using QuickGraph.Algorithms.ShortestPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubNetwork
{
    class RC
    {

        private Manager manager;
        private LRM lrm;
        private CC ConnectionController;
        private String strDebug = "";
        private Random random = new Random();

        public RC(Manager manager, LRM linkResourceManager, CC cc) 
        {
            this.manager = manager;
            this.lrm = linkResourceManager;
            this.ConnectionController = cc;
            //musi byc kopiowany, bo jak porty beda zajete to trzeba bedzie usunac dana krawedz

        }

        void LocalTopology() { }
        void RouteTableQuery() { }
        void NetworkTopology() { }

        public class SetupStore
        {
            public int source { get; set; }
            public int target { get; set; }
            public RoutingGraph ownTopology { get; set; }
            public List<RoutingGraph.Link> path { get; set; }
            public List<string> vcivpiList { get; set; }
            public int connectN { get; set; }
            public int requieredCapacity { get; set; }
            public SetupStore(int source, int target, RoutingGraph ownTopology, int connectN, int requieredCapacity)
            {
                this.source = source;
                this.target = target;
                this.ownTopology = ownTopology;
                this.connectN = connectN;
                path = new List<RoutingGraph.Link>();
                vcivpiList = new List<string>();
                this.requieredCapacity = requieredCapacity;

            }
            public SetupStore() { }
        }

        public NetworkConnection assignRoute(int src, int trg, int connectN, int requieredCapacity)
        {
            RoutingGraph ownTopology = RoutingGraph.MapTopology(manager.Topology, ConnectionController.VirtualPaths, requieredCapacity);
            SetupStore ss = new SetupStore(src, trg, ownTopology, connectN, requieredCapacity);
            if (ss.ownTopology.EdgeCount != 0)
            {
                if (this.findBestPath(ss) && this.askLRMs(ss))  //if true -> creating list vcivpi
                {
                    return this.parseToNetworConnection(ss);
                }
                else
                    return null;
            }
            return null;
        }

        private NetworkConnection parseToNetworConnection(SetupStore ss)
        {
            NetworkConnection networkConnection = new NetworkConnection(ss.connectN);
            //  List<LinkConnection> links = new List<LinkConnection>();
            LinkConnection link;
            networkConnection.Capacity = ss.requieredCapacity;

            foreach (var e in ss.path)
            {
                link = new LinkConnection();
                link.SourceId = e.Source.tNode.Name;
                link.TargetId = e.Target.tNode.Name;
                link.SourceRouting = e.SourceRouting;
                link.TargetRouting = e.TargetRouting;
                link.Link = e.tLink;
                networkConnection.Path.Add(link);
            }
            return networkConnection;
        }

        public bool askLRMs(SetupStore ss)
        {
            string VpiVci = "";
            /*
            if (!manager.Ping(ss.source))
                return false;*/
            foreach (var e in ss.path)
            {/*
                if (!manager.Ping(e.Target.Id))
                    return false;*/
                string[] srcrt;
                string[] trgrt;
                int i = 0;
                do
                {
                    srcrt = e.SourceRouting.Split(':'); // [0] -> Port, [1] -> Slot
                    trgrt = e.TargetRouting.Split(':');
                    if (srcrt[1] == "" && trgrt[1] == "")
                    {
                        srcrt[1] = rand();
                        trgrt[1] = rand();
                        //lrm.reserve(e.Source.tNode.Name, e.Target.tNode.Name, Convert.ToInt32(srcrt[1]));
                        //lrm.reserve(e.Source.tNode.Name, e.Target.tNode.Name, Convert.ToInt32(trgrt[1]));
                    }
                    i++;
                } while (
                    !lrm.isAvailable(e.Source.tNode.Name, e.Target.tNode.Name, Convert.ToInt32(srcrt[1])) &&
                    !lrm.isAvailable(e.Source.tNode.Name, e.Target.tNode.Name, Convert.ToInt32(trgrt[1])) &&
                    i<3
                    
                /*
                    !doIHaveFreePorts(manager.Get(e.Source.Id, "PortsOut." + srcrt[0] + ".Available." + srcrt[1] + "." + srcrt[2])) ||
                    !doIHaveFreePorts(manager.Get(e.Target.Id, "PortsIn." + trgrt[0] + ".Available." + trgrt[1] + "." + trgrt[2]))*/
                    );
                e.SourceRouting = srcrt[0] + "." + srcrt[1];
                e.TargetRouting = trgrt[0] + "." + trgrt[1];
                ss.vcivpiList.Add(VpiVci);

            }
            return true;
        }

        private bool doIHaveFreePorts(String response)
        {
            switch (response)
            {
                case "True": return true;
                case "False": return false;
                default: return false;
            }
        }

        private String rand()
        {

            int num = random.Next() % 3;
            return Convert.ToString(num);
        }

        bool findBestPath(SetupStore ss)
        {

            Dictionary<RoutingGraph.Link, double> edgeCost = new Dictionary<RoutingGraph.Link, double>(ss.ownTopology.EdgeCount);

            int index = 0;
            int max = ss.ownTopology.EdgeCount;
            while (index < max)
            {       //free capisity < requierd
                if (ss.ownTopology.Edges.ElementAt(index).Capacity < ss.requieredCapacity)
                {
                    ss.ownTopology.RemoveEdge(ss.ownTopology.Edges.ElementAt(index));
                    max = ss.ownTopology.EdgeCount;
                }
                else
                    index++;
            }
            foreach (var e in ss.ownTopology.Edges)
            {
                edgeCost.Add(e, e.Capacity);

            }

            //var dijkstra = new DijkstraShortestPathAlgorithm<RoutingGraph.Node, RoutingGraph.Link>(ss.ownTopology, e => edgeCost[e]);
            var dijkstra = new DijkstraShortestPathAlgorithm<RoutingGraph.Node, RoutingGraph.Link>(ss.ownTopology, e => edgeCost[e]);
            
            // Attach a Vertex Predecessor Recorder Observer to give us the paths
            //var predecessorObserver = new VertexPredecessorRecorderObserver<RoutingGraph.Node, RoutingGraph.Link>();
            //predecessorObserver.Attach(dijkstra);
            var predecessor = new VertexPredecessorRecorderObserver<RoutingGraph.Node, RoutingGraph.Link>();
            predecessor.Attach(dijkstra);
            dijkstra.Compute(this.IDtoNode(ss.source, ss.ownTopology));
            IEnumerable<RoutingGraph.Link> path;
            //List<Topology.Link> ddd = new List<Topology.Link>();
            if (predecessor.TryGetPath(this.IDtoNode(ss.target, ss.ownTopology), out path))
            {
                ss.path.AddRange(path);
                return true;
            }
            else return false;
        }

        private RoutingGraph.Node IDtoNode(int id, RoutingGraph ownTopology)
        {

            return ownTopology.Vertices.ToList().Find(delegate(RoutingGraph.Node no)
            {
                return no.Id == id;
            });

        }

    }
}
