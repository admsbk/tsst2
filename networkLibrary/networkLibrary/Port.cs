using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace networkLibrary
{
    class Port
    {
        public string portID;
        public List<string> slots = new List<string>();

        public Port(string portID, string slot)
        {
            this.portID = portID;
            this.slots.Add("1");
            this.slots.Add("2");
            this.slots.Add("3");
        }
    }
}
