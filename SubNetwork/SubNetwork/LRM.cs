using networkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SubNetwork
{
    public class LRM
    {
        public Dictionary<string, SNPLink> resources;
        private Dictionary<string, Topology.Link> linkConnections;
        private string domain;
        ListView links;

        public LRM(string domain, ListView view)
        {
            this.domain = domain;
            resources = new Dictionary<string, SNPLink>();
            linkConnections = new Dictionary<string, Topology.Link>();
            this.links = view;
        }

        public void loadTopology(string xmlPath)
        {
            try
            {
                Config conf = new Config(xmlPath, Constants.Link);
                foreach(List<string> link in conf.links)
                {
                    if (link.Count > 4)
                    {
                        if (domain == link[5])
                        {
                            //linkConnections.Add(link[0], new Topology.Link())
                            SNPLink l = new SNPLink(link.ElementAt(1), link.ElementAt(2), link.ElementAt(3), link.ElementAt(4));
                            resources.Add(link[0], l);
                            links.Items.Add(new SNPLink(link.ElementAt(1), link.ElementAt(2), link.ElementAt(3), link.ElementAt(4)));
                        }
                    }
                }
            }
            catch { }
        }

        public Dictionary<string, SNPLink> getLinks()
        {

            return resources;
        } 

        void LinkConnectionRequest() { }
        void LinkConnectionDeallocation() { }
        void SNPNegotiation() { }
        void SNPRelease() { }

        public void addLRM(String subConnection, string portIn, string portOut)
        {
            //resources.Add(subConnection, new SNPLink(portIn, portOut));
        }

        public bool isAvailable(string name, string slot)
        {
            return false;
        }

        public string Domain
        {
            get { return domain; }
        }
    }

    public class SNPLink{
        public string portSrc { get; set; }
        public string portDst { get; set; }
        public string nodeSrc { get; set; }
        public string nodeDst { get; set; }

        private bool isBusy;

        public SNPLink(string nodeIn, string nodeOut, string portIn, string portOut)
        {
            this.portSrc = portIn;
            this.portDst = portOut;
            this.nodeSrc = nodeIn;
            this.nodeDst = nodeOut;
            isBusy = false;
        }

        public bool busy()
        {
            if (isBusy == true)
                return true;
            else
                return false;
        }

        public bool requestLink()
        {
            if (!isBusy)
            {
                isBusy = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void releaseLink()
        {
            isBusy = false;
        }
    }
}
