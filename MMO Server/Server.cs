using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

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

        public static string escape(string text)
        {
            return MySql.Data.MySqlClient.MySqlHelper.EscapeString(text.ToString()).Replace("_", "\\_").Replace("%", "\\%");
        }

        public static string password(string pass)
        {
            SHA512 alg = SHA512.Create();

            byte[] result = alg.ComputeHash(Encoding.UTF8.GetBytes(pass));

            return BitConverter.ToString(result).Replace("-", "");
        }

        public static int GetUnixTimestamp()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}
