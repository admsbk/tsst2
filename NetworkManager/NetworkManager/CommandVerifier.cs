using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace NetworkManager
{
    class CommandVerifier
    {
        string error = "";

        public string getErrorMessage()
        {
            return error;
        }

        public bool verifyCommand(string command/*,List<TcpClient> clientSockets*/)
        {
            string[] subcommands = command.Split('%');
            /*int i = 0;
            List<string> clientSocketsID = new List<string>();

            foreach(TcpClient client in clientSockets)
            {
                
                 clientSocketsID[i] = clientSockets[i].ToString();
                 i++;
            }
            
            
            
            if (!clientSocketsID.Contains(subcommands[0]))
            {
                error = networkLibrary.Constants.NONEXISTENT_NODE;
                return false;
            }*/

            if (subcommands[1].Equals(networkLibrary.Constants.SET_LINK))
                {
                    /*if ((!subcommands[2].Contains("C")) && (!subcommands[2].Contains("N")))
                    {
                        error = networkLibrary.Constants.WRONG_IPORT;
                        return false;
                    }

                    if (!int.TryParse(subcommands[2].Substring(1), out value))
                    {
                        error = networkLibrary.Constants.WRONG_IPORT;
                        return false;
                    }

                    else if ((!subcommands[3].Contains("C")) && (!subcommands[3].Contains("N")))
                    {
                        error = networkLibrary.Constants.WRONG_IPORT;
                        return false;
                    }

                    else if (!int.TryParse(subcommands[3].Substring(1), out value))
                    {
                        error = networkLibrary.Constants.WRONG_IPORT;
                        return false;
                    }

                    else/*/
                    {
                        return true;
                    }


                }

            else if (subcommands[1].Equals(networkLibrary.Constants.DELETE_LINK))
                {
                    if (subcommands.Length>3)
                    {
                        error = networkLibrary.Constants.TOO_MANY;
                        return false;
                    }
                else
                    {
                        return true;
                    }

                }

            else if (subcommands[1].Equals(networkLibrary.Constants.SHOW_LINK))
                {
                    if (subcommands.Length > 2)
                    {
                        error = networkLibrary.Constants.TOO_MANY;
                        return false;
                    }
                else
                    {
                        return true;
                    }

                }

             else
                {
                    error = networkLibrary.Constants.WRONG_COM;
                    return false;
                }
            }
        }


    
    
    
    }

