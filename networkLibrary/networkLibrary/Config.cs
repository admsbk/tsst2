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

        public Config(string path, string elementType)
        {
            config = new List<string>();
            portsIn = new List<string>();
            portsOut = new List<string>();
            switchTable = new Dictionary<string, string>();
            
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
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
                    
                    readCloudPorts(xml, Constants.Link);
                }

                if (elementType == Constants.Client)
                {
                    config.Add(xnode.Attributes[Constants.CLIENT_NAME].Value);
                    readPorts(xml, Constants.INPUT_PORT, portsIn);
                    readPorts(xml, Constants.OUTPUT_PORT, portsOut);
                }
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
