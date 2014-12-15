using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMO_Server
{
    class Server
    {
        public const int SERVER_PORT = 8888;

        public const int MAX_USERS = 100;

        public static bool[] clientList = new bool[MAX_USERS];

        public static void ConsoleWrite(string message)
        {
            Console.WriteLine(" >> " + message);
        }
    }
}
