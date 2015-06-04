using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace networkLibrary
{
    public class Config
    {
        public List<string> config { get; set; }
        public List<string> portsIn { get; set; }
        public List<string> portsOut { get; set; }
        public Dictionary<string, string> switchTable { get; set; }
        public Dictionary<string, string> controlConnection { get; set; }
        public List<List<string>> links;

        public Config(string path, string elementType)
        {
            config = new List<string>();
            portsIn = new List<string>();
            portsOut = new List<string>();
            switchTable = new Dictionary<string, string>();
            controlConnection = new Dictionary<string, string>();
            links = new List<List<string>>();
            
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            if (elementType == Constants.Link)
            {
                foreach (XmlNode xnode in xml.SelectNodes(elementType))
                {
                    readLinks(xml, Constants.Link);
                }
            }
            else {
                foreach (XmlNode xnode in xml.SelectNodes(elementType))
                {
                    config.Add(xnode.Attributes[Constants.ID].Value);
                    config.Add(xnode.Attributes[Constants.CLOUD_IP].Value);
                    config.Add(xnode.Attributes[Constants.CLOUD_PORT].Value);
                    if (elementType == Constants.node)
                    {
                        config.Add(xnode.Attributes[Constants.MANAGER_IP].Value);
                        config.Add(xnode.Attributes[Constants.MANAGER_PORT].Value);
                        readPorts(xml, Constants.INPUT_PORT, portsIn);
                        readPorts(xml, Constants.OUTPUT_PORT, portsOut);
                    }
                    if (elementType == Constants.Cloud)
                    {
                        readSignalizationLinks(xml);
                        readCloudPorts(xml, Constants.Link);
                    }

                    if (elementType == Constants.Client)
                    {
                        config.Add(xnode.Attributes[Constants.CLIENT_NAME].Value);
                        readPorts(xml, Constants.INPUT_PORT, portsIn);
                        readPorts(xml, Constants.OUTPUT_PORT, portsOut);
                    }
                    if (elementType == Constants.AD)
                    {
                        config.Add(xnode.Attributes[Constants.DOMAIN].Value);
                    }
                }
            }
        }

        private void readSignalizationLinks(XmlDocument xml)
        {
            foreach (XmlNode xnode in xml.SelectNodes(Constants.CCLink))
            {
                controlConnection.Add(xnode.Attributes[Constants.SRC_ID].Value, 
                                        xnode.Attributes[Constants.DST_ID].Value);
            }
        }

        private void readLinks(XmlDocument xml, string attribute)
        {
            int i=0;
            foreach (XmlNode xnode in xml.SelectNodes(attribute))
            {
                links.Add(new List<string>());
                links[i].Add(xnode.Attributes[Constants.ID].Value);
                links[i].Add(xnode.Attributes[Constants.SRC_ID].Value);
                links[i].Add(xnode.Attributes[Constants.DST_ID].Value);
                links[i].Add(xnode.Attributes[Constants.SRC_PORT_ID].Value);
                links[i].Add(xnode.Attributes[Constants.DST_PORT_ID].Value);
                links[i].Add(xnode.Attributes[Constants.DOMAIN_SRC].Value);
                links[i].Add(xnode.Attributes[Constants.DOMAIN_DST].Value);
                links[i].Add(xnode.Attributes[Constants.WEIGHT].Value);
                links[i].Add(xnode.Attributes[Constants.LENGTH].Value);
                i++;
            }
        }

        void readPorts(XmlDocument xml, string attribute, List<string> listToWrite)
        {
            foreach (XmlNode xnode in xml.SelectNodes(attribute))
            {
                string input = xnode.Attributes[Constants.ID].Value;
                listToWrite.Add(input);
            }
        }

        private void readCloudPorts(XmlDocument xml, string attribute)
        {
            foreach (XmlNode xnode in xml.SelectNodes(attribute))
            {
                string id = xnode.Attributes[Constants.ID].Value;
                string srcId = xnode.Attributes[Constants.SRC_ID].Value;
                string dstId = xnode.Attributes[Constants.DST_ID].Value;
                string srcPortId = xnode.Attributes[Constants.SRC_PORT_ID].Value;
                string dstPortId = xnode.Attributes[Constants.DST_PORT_ID].Value;

                string key = srcId + "%" + srcPortId;
                string value = dstId + "%" + dstPortId;
                switchTable.Add(key, value);
            }
        }
    }
}
