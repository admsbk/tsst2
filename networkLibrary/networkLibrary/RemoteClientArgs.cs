using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace networkLibrary
{
    public class RemoteClientArgs
    {
        public string name {get; set;}
        public RemoteClientArgs(string n)
        {
            this.name = n;
        }
    }
}
