using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubNetwork
{
    public class RoutingGraph : UndirectedGraph<RoutingGraph.Node, RoutingGraph.Link>
    {
        public class Node
        {
            public Topology.Node tNode { get; private set; }
            public int Id { get { return tNode.Id; } }

            public Node(Topology.Node node)
            {
                tNode = node;
            }
        }

        public class Link : Edge<Node>
        {
            public Topology.Link tLink { get; private set; }
            public int Capacity { get { return tLink.Capacity; } }
            public string SourceRouting;
            public string TargetRouting;
            public Link(Node source, Node target, Topology.Link link)
                : base(source, target)
            {
                tLink = link;
                SourceRouting = link.SourceRouting;
                TargetRouting = link.TargetRouting;
            }
            public Link(Node source, Node target, Topology.Link link, bool path)
                : base(source, target)
            {
                tLink = link;
                SourceRouting = link.SourceRouting + "-";
                TargetRouting = link.TargetRouting + "-";
            }
        }

        public RoutingGraph()
            : base() { }

        public RoutingGraph(bool allowParallelEdges)
            : base(allowParallelEdges) { }

        /*
        public RoutingGraph(bool allowParallelEdges, int vertexCapacity)
            : base(allowParallelEdges, vertexCapacity) { }*/

        public static RoutingGraph MapTopology(Topology topology, Dictionary<int, STM> vpaths, int minCapacity)
        {
            RoutingGraph graph = new RoutingGraph();
            Dictionary<Topology.Node, Node> vertices = new Dictionary<Topology.Node, Node>();
            foreach (Topology.Node node in topology.Vertices)
            {
                Node n = new Node(node);
                graph.AddVertex(n);
                vertices.Add(node, n);
            }
            foreach (Topology.Link link in topology.Edges)
                if (link.Capacity >= minCapacity)
                    graph.AddEdge(new Link(vertices[link.Source], vertices[link.Target], link));
            foreach (STM vpath in vpaths.Values)
                if (vpath.Capacity >= minCapacity)
                    graph.AddEdge(new Link(vertices[vpath.Source], vertices[vpath.Target], vpath));
            return graph;
        }

        public static RoutingGraph MapTopology(Topology topology, int minCapacity)
        {
            RoutingGraph graph = new RoutingGraph();
            Dictionary<Topology.Node, Node> vertices = new Dictionary<Topology.Node, Node>();
            foreach (Topology.Node node in topology.Vertices)
            {
                Node n = new Node(node);
                graph.AddVertex(n);
                vertices.Add(node, n);
            }
            foreach (Topology.Link link in topology.Edges)
                if (link.Capacity >= minCapacity)
                    graph.AddEdge(new Link(vertices[link.Source], vertices[link.Target], link, true));
            return graph;
        }
    }
}
