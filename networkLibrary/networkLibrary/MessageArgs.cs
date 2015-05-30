using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace networkLibrary
{
    public class MessageArgs : EventArgs
    {
        public TcpClient ID { get; set; }
        public MessageArgs(string message)
        {
            this.message = message;
        }
        public string message { get; set; }
    }
}
