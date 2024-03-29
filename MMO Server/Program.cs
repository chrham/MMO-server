﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySql.Data.Common;
using MySql.Data.Types;

namespace MMO_Server
{
    class Program
    {
        static MySqlConnection mConnection;

        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Any, Server.SERVER_PORT);
            TcpClient clientSocket = default(TcpClient);

            serverSocket.Start();

            Server.ConsoleWrite("Connecting to MySQL...");

            try
            {
                mConnection = new MySqlConnection();
                mConnection.ConnectionString = "server=127.0.0.1;uid=root;pwd=;database=MMO;";
                mConnection.Open();

                Server.ConsoleWrite("Successfully connected to MySQL");
            }
            catch (MySqlException ex)
            {
                Server.ConsoleWrite(ex.Message, ConsoleColor.Red);
            }

            Server.ConsoleWrite("Waiting for clients...");

            while (true)
            {
                clientSocket = serverSocket.AcceptTcpClient();

                int uniqueID = -1;

                for (int clientListIndex = 0; clientListIndex < Server.MAX_USERS; clientListIndex++)
                {
                    if (Server.clientList[clientListIndex] == false)
                    {
                        uniqueID = clientListIndex;
                        Server.clientList[clientListIndex] = true;
                        break;
                    }
                }

                if (uniqueID == -1)
                {
                    Server.ConsoleWrite("Client [IP: " + (clientSocket.Client.RemoteEndPoint as IPEndPoint).Address.ToString() + "] rejected, max users (" + Server.MAX_USERS + ") reached");

                    clientSocket.Close();
                }
                else
                {
                    Server.ConsoleWrite("Client #" + uniqueID + " [IP: " + (clientSocket.Client.RemoteEndPoint as IPEndPoint).Address.ToString() + "] has connected");

                    ClientHandler clientHandler = new ClientHandler();
                    clientHandler.startClient(clientSocket, uniqueID, (clientSocket.Client.RemoteEndPoint as IPEndPoint).Address.ToString(), mConnection);
                }
            }

            clientSocket.Close();
            serverSocket.Stop();

            Server.ConsoleWrite("Shutting down");
            Console.ReadLine();
        }
    }
}
