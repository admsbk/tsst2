using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace networkLibrary
{
    public class SwitchingBoxNode
    {
        //Wpisy w słowniku: "KTO_PRZYSLAL%NA_KTORY_PORT", "KOMU_WYSLAC%NA_KTORY_PORT"
        private Dictionary<string, string> SwitchingTable;
        public SwitchingBoxNode()
        {
            SwitchingTable = new Dictionary<string, string>();
        }

        //WZÓR WIADOMOSCI: "KTO_PRZYSLAL%Z_KTOREGO_PORTU&cos_tam_dalej" 
        //

        //ZWRACA: "KOMU%NA_KTORY_PORT&cos_tam_dalej"           ale w cos_tam_dalej trzeba dać inne delimetery niż & i %
        public string forwardMessage(string message)
        {
            try
            {
                string dstPort;
                string toReturn;
                string[] tempMessage = message.Split('%'); //od kogo + pdu
                string[] tempMessage2 = tempMessage[1].Split('&'); // port + dane
                if(tempMessage[0].Contains("C"))
                    dstPort = (SwitchingTable[tempMessage2[0]+"."]);
                else
                    dstPort = SwitchingTable[tempMessage2[0]];
                string[] dstPortTemp = dstPort.Split('.');
                toReturn = dstPortTemp[0] + "^" + dstPortTemp[1] + "&" + tempMessage2[1];//dodanie payloadu po prostu
                return toReturn;
            }
            catch(Exception e)
            {
                return null;
            }

        }

        //szablon: src - "OD_KOGO%PORT"    dst - "DO_KOGO%PORT"
        public bool addLink(string src, string srcSlot, string dst, string dstSlot)
        {
            string tempInPort = src + "." + srcSlot;
            string tempOutPort = dst + "." + dstSlot;

            if (!SwitchingTable.ContainsKey(tempInPort))
            {
                this.SwitchingTable.Add(tempInPort, tempOutPort);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void removeLink(string src, string srcSlot)
        {
            string tempInPort = src+"."+srcSlot;
            this.SwitchingTable.Remove(tempInPort);
        }

        public void removeAllLinks()
        {

            this.SwitchingTable.Clear();
        }
    }
}
