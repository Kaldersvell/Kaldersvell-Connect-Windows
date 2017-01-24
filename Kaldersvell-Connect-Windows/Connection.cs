using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaldersvell_Connect_Windows
{
    public class Connection
    {
        public string Name;
        public string IP;
        public string Username;
        public string Password;
        public Connection()
        {
            Name = "";
            IP = "";
            Username = "";
            Password = "";
        }
        public Connection( string n, string i, string u, string p)
        {
            Name = n;
            IP = i;
            Username = u;
            Password = p;
        }
    }
}
