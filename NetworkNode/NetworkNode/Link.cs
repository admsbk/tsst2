using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{

    class Link
    {
        public string ID { get; set; }
        public string src { get; set; }
        public string dst { get; set; }
        public string srcSlot { get; set; }
        public string dstSlot { get; set; }

        public Link(string id, string src, string srcSlot, string dst, string dstSlot)
        {
            this.ID = id;
            this.src = src;
            this.srcSlot = srcSlot;
            this.dst = dst;
            this.dstSlot = dstSlot;
        }
    }
}
