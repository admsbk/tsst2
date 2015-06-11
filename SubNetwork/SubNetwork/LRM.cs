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

        public List<SNPLink> getExternal()
        {
            List<SNPLink> links = new List<SNPLink>();
            foreach (KeyValuePair<string, SNPLink> kvp in resources)
            {
                if(kvp.Value.name.Contains("External"))
                    links.Add(kvp.Value);
            }

            return links;
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
                        if (domain == link[5] && domain == link[6])
                        {
                            //linkConnections.Add(link[0], new Topology.Link())
                            SNPLink l = new SNPLink(link.ElementAt(1), link.ElementAt(2), link.ElementAt(3), link.ElementAt(4), link.ElementAt(9));
                            resources.Add(link[0], l);
                            links.Items.Add(new SNPLink(link.ElementAt(1), link.ElementAt(2), link.ElementAt(3), link.ElementAt(4), link.ElementAt(9)));
                        }

                        else if (domain == link[5])
                        {
                            SNPLink l = new SNPLink(link.ElementAt(1), link.ElementAt(6), link.ElementAt(3), link.ElementAt(4), link.ElementAt(9));
                            resources.Add(link[0], l);
                            links.Items.Add(new SNPLink(link.ElementAt(1), link.ElementAt(6), link.ElementAt(3), link.ElementAt(4), link.ElementAt(9)));
                        }

                        else if (domain == link[6])
                        {
                            SNPLink l = new SNPLink(link.ElementAt(5), link.ElementAt(2), link.ElementAt(3), link.ElementAt(4), link.ElementAt(9));
                            resources.Add(link[0], l);
                            links.Items.Add(new SNPLink(link.ElementAt(5), link.ElementAt(2), link.ElementAt(3), link.ElementAt(4), link.ElementAt(9)));
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

        public bool isAvailable(string namesrc, string namedst, int slot)
        {
            foreach (KeyValuePair<string, SNPLink> entry in resources)
            {
                if ((entry.Value.nodeSrc == namesrc && entry.Value.nodeDst == namedst) || (entry.Value.nodeSrc == namedst && entry.Value.nodeDst == namesrc))
                    return entry.Value.slotFree(slot);
            }
            
            return false;
        }

        public void reserve(string namesrc, string namedst, int slot)
        {
            foreach (KeyValuePair<string, SNPLink> entry in resources)
            {
                if (entry.Value.nodeSrc == namesrc && entry.Value.nodeDst == namedst)
                    entry.Value.requestLink(slot);
            }
        }

        public void release(string namesrc, string namedst, int slot)
        {
            foreach (KeyValuePair<string, SNPLink> entry in resources)
            {
                if (entry.Value.nodeSrc == namesrc && entry.Value.nodeDst == namedst)
                    entry.Value.releaseLink(slot);
            }
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

        public string name { get; set; }

        private bool[] isBusy;

        public SNPLink(string nodeIn, string nodeOut, string portIn, string portOut, string name)
        {
            this.portSrc = portIn;
            this.portDst = portOut;
            this.nodeSrc = nodeIn;
            this.nodeDst = nodeOut;
            this.name = name;
            isBusy = new bool[3];
            for (int i = 0; i < isBusy.Length; i++)
                isBusy[i] = false;
        }

        public bool slotFree(int index)
        {
            return !isBusy[index];
        }

        
        public bool requestLink(int i)
        {
            if (!isBusy[i])
            {
                isBusy[i] = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void releaseLink(int i)
        {
            isBusy[i] = false;
        }
    }
}
