using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace networkLibrary
{
    public class Pair<T, U>
    {
        public Pair()
        {
        }

        public Pair(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }

        bool ContainsKey(T key)
        {     
            return true;
        }
    };

    public class SwitchingBoxNode
    {
        //Wpisy w słowniku: "KTO_PRZYSLAL%NA_KTORY_PORT", "KOMU_WYSLAC%NA_KTORY_PORT"
        private List<Pair<string, string>> SwitchingTable;
        public SwitchingBoxNode()
        {
            SwitchingTable = new List<Pair<string, string>>();
        }

        //WZÓR WIADOMOSCI: "KTO_PRZYSLAL%Z_KTOREGO_PORTU&cos_tam_dalej" 
        //

        //ZWRACA: "KOMU%NA_KTORY_PORT&cos_tam_dalej"           ale w cos_tam_dalej trzeba dać inne delimetery niż & i %
        public string forwardMessage(string message)
        {
            try
            {
                string dstPort = "";
                string toReturn;
                string[] tempMessage = message.Split('%'); //od kogo + pdu
                string[] tempMessage2 = tempMessage[1].Split('&'); // port + dane
                if (tempMessage[0].Contains("C"))
                {
                    foreach (Pair<string, string> pair in SwitchingTable)
                    {
                        if (pair.First == tempMessage2[0] + ".")
                        {
                            dstPort = pair.Second + ".";
                            break;
                        }

                        else if (pair.Second == tempMessage2[0] + ".")
                        {
                            dstPort = pair.First + ".";
                            break;
                        }
                    }
                }
                else
                {
                    foreach (Pair<string, string> pair in SwitchingTable)
                    {
                        if (pair.First == tempMessage2[0])
                        {
                            dstPort = pair.Second;
                            break;
                        }

                        else if (pair.Second == tempMessage2[0])
                        {
                            dstPort = pair.First;
                            break;
                        }
                    }
                }
                    
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
            
            if (!contains(tempInPort))
            {
                this.SwitchingTable.Add(new Pair<string, string>(tempInPort, tempOutPort));
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool contains(string port)
        {
            foreach (Pair<string, string> pair in SwitchingTable)
            {
                if (pair.First == port)
                    return true;
                else if (pair.Second == port)
                    return true;
            }
            return false;
        }

        public void removeLink(string src, string srcSlot)
        {
            string tempInPort = src+"."+srcSlot;
            Pair<string, string> toRm = new Pair<string,string>();
            foreach (Pair<string, string> pair in SwitchingTable)
            {
                if (pair.First == tempInPort)
                    toRm = pair;
                else if (pair.Second == tempInPort)
                    toRm = pair;
            }
            this.SwitchingTable.Remove(toRm);
        }

        public void removeAllLinks()
        {
            this.SwitchingTable.Clear();
        }
    }
}
