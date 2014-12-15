using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace MMO_Server
{
    class ClientHandler
    {
        TcpClient clientSocket;
        int clientNumber;
        string clientIP;

        public ClientHandler()
        {
        }

        private void OnReceiveClientMessage(int message, params string[] parameters)
        {
            switch (message)
            {
                case Messages.LOGIN:
                {
                    break;
                }
            }
        }

        /* ========= Internal ========= */

        public void startClient(TcpClient inClientSocket, int clineNo, string clineIP)
        {
            this.clientSocket = inClientSocket;
            this.clientNumber = clineNo;
            this.clientIP = clineIP;

            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }

        private void sendMessage(int message, params string[] parameters)
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Server.ConsoleWrite("Client #" + clientNumber + " [IP: " + clientIP + "] has disconnected");
                    Server.clientList[clientNumber] = false;
                    clientSocket.Close();
                }
            }
        }
    }
}
