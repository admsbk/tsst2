using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace networkLibrary
{
    public class SwitchingBox
    {
        //Wpisy w słowniku: "KTO_PRZYSLAL%NA_KTORY_PORT", "KOMU_WYSLAC%NA_KTORY_PORT"
        private List<Pair<string, string>> SwitchingTable;
        public SwitchingBox()
        {
            SwitchingTable = new List<Pair<string, string>>();
        }

        //WZÓR WIADOMOSCI: "KTO_PRZYSLAL%Z_KTOREGO_PORTU&cos_tam_dalej" 
        //

        //ZWRACA: "KOMU%NA_KTORY_PORT&cos_tam_dalej"           ale w cos_tam_dalej trzeba dać inne delimetery niż & i %
        public string forwardMessage(string message)
        {
            string[] tempMessage = new string[2];
            tempMessage = message.Split('&');

            foreach (Pair<string, string> pair in SwitchingTable)
            {
                if (pair.First == tempMessage[0])
                {
                    return pair.Second + "&" + tempMessage[1];
                }

                else if (pair.Second == tempMessage[0])
                {
                    return pair.First + "&" + tempMessage[1];
                }
            }
            return null;

        }

        //szablon: src - "OD_KOGO%PORT"    dst - "DO_KOGO%PORT"
        public void addLink(string src, string dst)
        {
            if (!contains(src))
            {
                this.SwitchingTable.Add(new Pair<string, string>(src, dst));
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

        public void removeLink(string src)
        {
            //this.SwitchingTable.Remove(src);
        }

        public void removeAllLinks()
        {
            this.SwitchingTable.Clear();
        }
    }
}
