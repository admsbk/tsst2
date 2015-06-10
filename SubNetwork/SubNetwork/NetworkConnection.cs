using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubNetwork
{
    public class LinkConnection
    {
        public string SourceId;
        public string TargetId;
        public string SourceRouting; // Port:Vpi:Vci
        public string TargetRouting; // Port:Vpi:Vci
        public Topology.Link Link;
    }

    public class NetworkConnection
    {
        public int Id { get; private set; }
        public int Capacity;
        public List<LinkConnection> Path { get; private set; }
        public bool Active = false;

        public List<string> Nodes
        {
            get
            {
                List<string> nodes = new List<string>();
                if (Path.Count > 0)
                    nodes.Add(Path.First().SourceId);
                foreach (var link in Path)
                    nodes.Add(link.TargetId);
                return nodes;
            }
        }

        public string Source
        {
            get { return Path.First().SourceId; }
        }

        public string Target
        {
            get { return Path.Last().TargetId; }
        }

        public NetworkConnection(int id)
        {
            Id = id;
            Path = new List<LinkConnection>();
        }

        public NetworkConnection() { }
    }

    public class STM : Topology.Link
    {
        public int Id { get; private set; }
        public int SourceSlot { get; private set; }
        public int TargetSlot { get; private set; }
        public override string SourceRouting { get { return SourcePort + ":" + SourceSlot + ":"; } }
        public override string TargetRouting { get { return TargetPort + ":" + TargetSlot + ":"; } }
        public List<LinkConnection> Path;

        public STM(int id, Topology.Node source, Topology.Node target, string sourcePort, string targetPort, int sourceVpi, int targetVpi, int capacity)
            : base(source, target, sourcePort, targetPort, capacity)
        {
            this.Id = id;
            this.SourceSlot = sourceVpi;
            this.TargetSlot = targetVpi;
            this.Path = new List<LinkConnection>();
        }

        new public int Capacity
        {
            get
            {
                int cap = Int32.MaxValue;
                foreach (var link in Path)
                    if (link.Link.Capacity < cap)
                        cap = link.Link.Capacity;
                return cap;
            }
            set
            {
                int diff = value - Capacity;
                foreach (var link in Path)
                    link.Link.Capacity += diff;
            }
        }

    }
}
