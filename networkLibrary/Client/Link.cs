using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud
{
    class Link
    {
        public string ID { get; set; }
        public string Source { get; set; }
        public string dst { get; set; }
        public string srcPort { get; set; }
        public string DstPort { get; set; }

        public Link(String linkID, String nodeIn, String nodeOut, String portIn, String portOut)
        {
            this.ID = linkID;
            this.Source = nodeIn;
            this.dst = nodeOut;
            this.srcPort = portIn;
            this.DstPort = portOut;
        }
    }
}
