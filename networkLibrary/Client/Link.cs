using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud
{
    class CrossConnection
    {
        public string ID { get; set; }
        public string Source { get; set; }
        public string dst { get; set; }
        public string srcPort { get; set; }
        public string DstPort { get; set; }

        public CrossConnection(String CrossConnectionID, String nodeIn, String nodeOut, String portIn, String portOut)
        {
            this.ID = CrossConnectionID;
            this.Source = nodeIn;
            this.dst = nodeOut;
            this.srcPort = portIn;
            this.DstPort = portOut;
        }
    }
}
