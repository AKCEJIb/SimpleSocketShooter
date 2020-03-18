using Game.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game.Server
{
   
    public class GameServer
    {
        private GameTcpServer ServerSocket;
        private List<GameTcpServerConnection> connections;
        public GameServer(int port)
        {
            connections = new List<GameTcpServerConnection>();
            ServerSocket = new GameTcpServer();
            ServerSocket.Bind(port, 2);
            ServerSocket.AcceptCompleted += ServerSocket_AcceptCompleted;
        }

        private void ServerSocket_AcceptCompleted(object sender, TcpCompletedEventArgs e)
        {
            Console.WriteLine("Someone connected :)");

            ServerSocket.ClientAcceptAsync();

            var client = (GameTcpServerConnection) e.Data;

            connections.Add(client);
            client.SendCompleted += Client_SendCompleted;
            client.ReadCompleted += Client_ReadCompleted;
        }

        private void Client_ReadCompleted(object sender, TcpCompletedEventArgs e)
        {
            Console.WriteLine("Read event");
        }

        private void Client_SendCompleted(object sender, TcpCompletedEventArgs e)
        {
            Console.WriteLine("Send event");
        }

        public void Start()
        {
            ServerSocket.ClientAcceptAsync();

            Console.WriteLine("Server started");

            while (true)
            {
                var str = Console.ReadLine();
                Console.WriteLine(connections.Count);
                foreach(var con in connections)
                {
                    con.SendAsync(new byte[] { 0,1,1,0 });
                }
            }
        }
    }
}
