using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using networkLibrary;

namespace SubNetwork
{
    class NCC
    {
        private CC connectionController;
        private Dictionary<string, string> directory;

        public NCC()
        {
            directory = new Dictionary<string, string>();
            connectionController = new CC();
        }

        public string checkClientNetAddress(string clientId)
        {
            try
            {
                string clientNetAddress = directory[clientId];
                return clientNetAddress;
            }
            catch (KeyNotFoundException)
            {
                return Constants.UNKNOWN_CLIENT;
            }

        }

        public string getName(string netAddress)
        {
            for (int i = 0; i < directory.Count; i++)
            {
                if (this.directory.ElementAt(i).Value.Equals(netAddress))
                    return directory.ElementAt(i).Key;
            }
            return null;

        }
    }
}
