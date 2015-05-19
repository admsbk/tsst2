using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NetworkManager
{
    class Configuration
    {
        private string managerId;
        public string ManagerId
        {
            get { return managerId; }
        }

        private int managerPort;
        public int ManagerPort
        {
            get { return managerPort; }
        }

        private Logs logs;

        private List<string[]> virtualContainerConfig;

        public List<string[]> VirtualContainerConfig
        {
            get { return virtualContainerConfig;}
        }

        public Configuration(Logs logs)
        {
            this.logs = logs;
        }

        private static List<string> readConfig(XmlDocument xml)
        {
            List<string> nodeConfig = new List<string>();
            foreach (XmlNode xnode in xml.SelectNodes("//Manager[@Id]"))
            {
                string nodeId = xnode.Attributes[networkLibrary.Constants.ID].Value;
                nodeConfig.Add(nodeId);
                string managerPort = xnode.Attributes[networkLibrary.Constants.MANAGER_PORT].Value;
                nodeConfig.Add(managerPort);
            }
            return nodeConfig;
        }

        private List<string[]> readVirtualContainers(XmlDocument xml)
        {
            List<string[]> modulationConfig = new List<string[]>();
            foreach (XmlNode xnode in xml.SelectNodes("//VirtualContainers/VirtualContainer"))
            {
                string[] tableString = new string[2];
                string virtualContainerId = xnode.Attributes[networkLibrary.Constants.ID].Value;
                tableString[0] = virtualContainerId;
                string virtualContainerBitrate = xnode.Attributes[networkLibrary.Constants.BITRATE].Value;
                tableString[1] = virtualContainerBitrate;

                modulationConfig.Add(tableString);
            }

            return modulationConfig;
        }

        public bool loadConfiguration(string path)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(path);
                List<string> managerConfig = new List<string>();
                managerConfig = Configuration.readConfig(xml);

                this.managerId = managerConfig[0];
                this.managerPort = Convert.ToInt32(managerConfig[1]);

                this.virtualContainerConfig = new List<string[]>();
                this.virtualContainerConfig = readVirtualContainers(xml);

                string[] filePath = path.Split('\\');
                logs.addLog(networkLibrary.Constants.CONFIG_OK + " " + filePath[filePath.Length - 1], true, 0);
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.StackTrace);
                logs.addLog(networkLibrary.Constants.CONFIG_ERROR,true,3);
                return false;
            }
        }


    }
}
