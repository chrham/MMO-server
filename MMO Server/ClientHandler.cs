using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace MMO_Server
{
    class ClientHandler
    {
        TcpClient clientSocket;
        int clientNumber;
        string clientIP;

        MySqlConnection mConnection;

        public ClientHandler()
        {
        }

        private void OnReceiveClientMessage(int message, params object[] parameters)
        {
            switch (message)
            {
                case Messages.LOGIN:
                {
                        Server.ConsoleWrite("1");
                    MySqlDataReader mData = (new MySqlCommand("SELECT COUNT(*) AS `rows`, `id`, `username` FROM `accounts` WHERE `username` = '" + Server.escape(parameters[0].ToString()) + "' AND `password` = '" + Server.password(parameters[1].ToString()) + "'", mConnection)).ExecuteReader();

                    if (mData.Read())
                    {
                        Server.ConsoleWrite("2");
                        if (Convert.ToInt32(mData["rows"]) == 0)
                        {
                            Server.ConsoleWrite("3");
                            sendMessage(Messages.LOGIN, false);
                        }
                        else
                        {
                            Server.ConsoleWrite("4");
                            sendMessage(Messages.LOGIN, true);
                        }
                    }

                    break;
                }
            }
        }

        /* ========= Internal ========= */

        public void startClient(TcpClient userSocket, int userNo, string userIP, MySqlConnection myConnection)
        {
            this.clientSocket = userSocket;
            this.clientNumber = userNo;
            this.clientIP = userIP;
            this.mConnection = myConnection;

            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }

        private void sendMessage(int message, params object[] parameters)
        {
            Byte[] sendBytes = null;

            NetworkStream networkStream = clientSocket.GetStream();

            string sendingMessage = message + ";" + String.Join(";", parameters);

            sendBytes = Encoding.ASCII.GetBytes(sendingMessage);
            networkStream.Write(sendBytes, 0, sendBytes.Length);
            networkStream.Flush();
        }

        private void doChat()
        {
            while (clientSocket.Connected)
            {
                try
                {
                    byte[] bytesFrom = new byte[505196];
                    string dataFromClient = null;

                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                    List<string> receivedMessage = dataFromClient.Split(';').ToList();
                    int messageID = Convert.ToInt32(receivedMessage[0]);

                    receivedMessage.RemoveAt(0);

                    OnReceiveClientMessage(messageID, receivedMessage.ToArray());
                }
                catch
                {
                    
                }
            }

            Server.ConsoleWrite("Client #" + clientNumber + " [IP: " + clientIP + "] has disconnected");
            Server.clientList[clientNumber] = false;
            clientSocket.Close();
        }
    }
}
