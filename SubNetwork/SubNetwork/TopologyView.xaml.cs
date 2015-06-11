using GraphSharp.Controls;
using networkLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace SubNetwork
{
    
    public class TopologyLayout : GraphLayout<Topology.Node, Topology.Link, Topology> { }


    /// <summary>
    /// Interaction logic for TopologyView.xaml
    /// </summary>
    public partial class TopologyView : Window, INotifyPropertyChanged
    {
        private Topology graph = new Topology();
        public Topology Graph
        {
            get { return graph; }
            set
            {
                graph = value;
                NotifyPropertyChanged("Graph");
            }
        }

        public TopologyView(string pathToTopology)
        {
            //var g = new BidirectionalGraph<object, IEdge<object>>();
            //foreach (string node in manager.GetElements())
            //{
            //    g.AddVertex(node);
            //}
            //foreach (Edge<int> connection in manager.GetLinks())
            //{
            //    g.AddEdge(new Edge<object>(connection.Source, connection.Target));
            //}
            readLinksToGraph(pathToTopology);
            
            this.DataContext = this;
            InitializeComponent();
        }

        private void readLinksToGraph(string path)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            
            Dictionary<int, Topology.Node> nodes = new Dictionary<int, Topology.Node>();
            List<List<string>> lc = new List<List<string>>();
            foreach (XmlNode xnode in xml.SelectNodes(Constants.Link))
            {
                List<string> links = new List<string>();
                links.Add(xnode.Attributes[Constants.ID].Value);
                links.Add(xnode.Attributes[Constants.SRC_ID].Value);
                links.Add(xnode.Attributes[Constants.DST_ID].Value);
                links.Add(xnode.Attributes[Constants.SRC_PORT_ID].Value);
                links.Add(xnode.Attributes[Constants.DST_PORT_ID].Value);
                links.Add(xnode.Attributes[Constants.DOMAIN_SRC].Value);
                links.Add(xnode.Attributes[Constants.DOMAIN_DST].Value);
                links.Add(xnode.Attributes[Constants.WEIGHT].Value);
                links.Add(xnode.Attributes[Constants.LENGTH].Value);

                string pattern = @"\d+";
                Match match = Regex.Match(links[1], pattern);
                int id = Convert.ToInt32(match.Value);
                int id_link = Convert.ToInt32(Regex.Match(links[0], pattern).Value);
                int id_dst = Convert.ToInt32(Regex.Match(links[2], pattern).Value);
                if(links[1].Contains("Client"))
                    id = Convert.ToInt32("100"+match.Value);
                if (links[2].Contains("Client"))
                    id_dst = Convert.ToInt32("100" + match.Value);
                if(!nodes.ContainsKey(id))
                    nodes.Add(id, new Topology.Node(id, links[1], "Type"));
                if (!nodes.ContainsKey(id_dst))
                    nodes.Add(id_dst, new Topology.Node(id_dst, links[2], "Type"));
                lc.Add(links);
            }
            foreach (KeyValuePair<int, Topology.Node> entry in nodes)
                graph.AddVertex(entry.Value);
            foreach (List<string> entry in lc)
            {
                string pattern = @"\d+";
                Match match = Regex.Match(entry[1], pattern);
                int id_src = Convert.ToInt32(Regex.Match(entry[1], pattern).Value);
                int id_dst = Convert.ToInt32(Regex.Match(entry[2], pattern).Value);
                if(entry[1].Contains("Client"))
                    id_src = Convert.ToInt32("100"+match.Value);
                if(entry[2].Contains("Client"))
                    id_dst = Convert.ToInt32("100"+match.Value);
                ///string arg1 = entry.Value[id_dst];
                string arg2 = entry[3];
                string arg3 = entry[4];
                graph.AddEdge(new Topology.Link(nodes[id_src], nodes[id_dst], arg2, arg3, 140, ""));
            }

        }

        new public void Show()
        {
            if (this.graph.Vertices.Count() == 0)
                this.Close();
            else
                base.Show();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
