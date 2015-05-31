using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using networkLibrary;

namespace SubNetwork
{
    class AdministrativeDomain
    {
        private transportClient signalization;
        private networkLibrary.transportServer.NewMsgHandler msgHandler;
        private Dictionary<string, string> directory;

        public AdministrativeDomain()
        {
            signalization = new transportClient("localhost", "3333");
            msgHandler = new transportServer.NewMsgHandler(newMessageReceived);
            directory = new Dictionary<string, string>();
            signalization.sendMessage("NCC1#");
        }

        #region message received methods
        private void newMessageReceived(object a, MessageArgs e) { }
        private void nameCastReceived(string message) { }
        private void callCoordinationReceived(string message) { }
        private void callRequestReceived(string message) { }
        #endregion
    }
}
