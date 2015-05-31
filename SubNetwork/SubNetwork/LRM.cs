using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubNetwork
{
    class LRM
    {
        private Dictionary<String, OneLRM> resources;
        private String domain;

        public LRM(String domain)
        {
            this.domain = domain;
            resources = new Dictionary<String, OneLRM>();
        }

        void LinkConnectionRequest() { }
        void LinkConnectionDeallocation() { }
        void SNPNegotiation() { }
        void SNPRelease() { }

        public void addLRM(String subConnection, List<String> portIn, List<String> portOut)
        {
            resources.Add(subConnection, new OneLRM(portIn, portOut));
        }

        public String Domain
        {
            get { return domain; }
        }
    }

    class OneLRM{
        public List<String> portIn { get; set; }
        public List<String> portOut { get; set; }
        public OneLRM(List<String> portIn, List<String> portOut)
        {
            this.portIn = portIn;
            this.portOut = portOut;
        }
    }
}
