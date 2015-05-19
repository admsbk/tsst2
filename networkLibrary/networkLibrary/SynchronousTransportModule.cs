using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace networkLibrary
{
    public class SynchronousTransportModule
    {
        public string SOH { get; set; }
        int size;
        private string[] slot;

        public SynchronousTransportModule(int size)
        {
            this.size = size;
            slot = new string[size];


        }

        public string prepareToSend()
        {
            string toSend = SOH + ":";
            foreach (string s in slot)
            {
                toSend += s + "/";
            }
            return toSend;
        }

        public void reserveSlot(int s, string message)
        {
            if (slot[s]==null)
            {
                slot[s] = message;
            }
            else
            {
                throw new Exception("slot saved exception");
            }
        }

        public void clearSTM()
        {
            slot = new string[size];

        }

        public static string[] getSlots(string message) 
        {

                string[] splitted = message.Split('/');
                bool flag = false;
                foreach (string s in splitted)
                {
                    if (s.Length != 0)
                        flag = true;
                }
                if (flag)
                    return splitted;
                else
                    return null;
        
        }

    }
}
