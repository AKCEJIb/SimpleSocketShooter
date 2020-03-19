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

        private class ClientContext
        {
            public PacketProtocol PacketProtocol { get; set; }
            public bool Connected;
        }
        private GameTcpServer ServerSocket;
        private Dictionary<GameTcpServerConnection, ClientContext> clients =
            new Dictionary<GameTcpServerConnection, ClientContext>();
        public GameServer(int port)
        {
            ServerSocket = new GameTcpServer();
            ServerSocket.Bind(port, 2);
            ServerSocket.AcceptCompleted += ServerSocket_AcceptCompleted;
        }

        private void ServerSocket_AcceptCompleted(object sender, TcpCompletedEventArgs e)
        {
            ServerSocket.ClientAcceptAsync();

            var client = (GameTcpServerConnection) e.Data;


            var protocol = new PacketProtocol(client);
            var context = new ClientContext();
            context.PacketProtocol = protocol;
            context.Connected = true;

            clients.Add(client, context);

            protocol.PacketArrived += Protocol_PacketArrived;
            client.SendCompleted += Client_SendCompleted;

            protocol.Start();
        }

        private void Protocol_PacketArrived(object sender, TcpCompletedEventArgs e)
        {
            Console.WriteLine(Packet.Deserialize((byte[])e.Data).Content);
        }

        private void Client_ReadCompleted(object sender, TcpCompletedEventArgs e)
        {
            if (!e.Error)
            {
                Console.WriteLine($"Read event, bytes read: {e.Data}");
            }
            else
            {
                Console.WriteLine("Client disconnected!");
            }
        }

        private void Client_SendCompleted(object sender, TcpCompletedEventArgs e)
        {
            Console.WriteLine($"Send event, bytes sent: {(int)e.Data}");
        }

        public void Start()
        {
            ServerSocket.ClientAcceptAsync();

            Console.WriteLine("Server started");

            while (true)
            {
                var str = Console.ReadLine();

                foreach(KeyValuePair<GameTcpServerConnection, ClientContext> client in clients)
                {
                    PacketProtocol.SendPacket(client.Key, new Packet
                    {
                        Content = str,
                        Type = PacketType.MESSAGE
                    });
                }
            }
        }
    }
}
