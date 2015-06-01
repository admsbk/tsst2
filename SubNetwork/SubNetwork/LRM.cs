using networkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubNetwork
{
    class LRM
    {
        private Dictionary<string, SNPLink> resources;
        private string domain;
        MainWindow window;

        public LRM(string domain, MainWindow window)
        {
            this.domain = domain;
            resources = new Dictionary<string, SNPLink>();
            this.window = window;
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
                            SNPLink l = new SNPLink(link[1], link[2], link[3], link[4]);
                            resources.Add(link[0], l);
                            window.links.Items.Add(new SNPLink(link[1], link[2], link[3], link[4]));
                        }
                    }
                }
            }
            catch { }
        }

        void LinkConnectionRequest() { }
        void LinkConnectionDeallocation() { }
        void SNPNegotiation() { }
        void SNPRelease() { }

        public void addLRM(String subConnection, string portIn, string portOut)
        {
            //resources.Add(subConnection, new SNPLink(portIn, portOut));
        }

        public string Domain
        {
            get { return domain; }
        }
    }

    class SNPLink{
        public String portSrc { get; set; }
        public String portDst { get; set; }
        public String nodeSrc { get; set; }
        public String nodeDst { get; set; }

        private bool isBusy;

        public SNPLink(String nodeIn, String nodeOut, String portIn, String portOut)
        {
            this.portSrc = portSrc;
            this.portDst = portDst;
            this.nodeSrc = nodeSrc;
            this.nodeDst = nodeDst;
            isBusy = false;
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
