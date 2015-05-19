using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNode
{
    class Port
    {
        string portID;
        string slot;

        public Port(string portID, string slot)
        {
            this.portID = portID;
            this.slot = slot;
        }
    }
}
