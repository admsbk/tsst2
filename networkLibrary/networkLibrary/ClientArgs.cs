using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace networkLibrary
{
    public class ClientArgs : EventArgs
    {
     
        public string NodeName{get;set;}
        public TcpClient ID{ get; set; }
        public string NodeID { get; set; }
        
    }
}
