using Game.Networking;
using Game.Networking.Entity;
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
            public PlayerMp Player { get; set; }
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
        public GameServer(IPAddress ip, int port)
        {
            ServerSocket = new GameTcpServer();
            ServerSocket.Bind(ip, port, 2);
            ServerSocket.AcceptCompleted += ServerSocket_AcceptCompleted;
        }

        private void CloseConnectionWithClient(GameTcpServerConnection socket)
        {
            socket?.Close();

            clients.Remove(socket);
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

            Console.WriteLine($"Client {client.RemoteEndPoint} connected to server.");
        }

        private void Protocol_PacketArrived(object sender, TcpCompletedEventArgs e)
        {
            var clientSocket = (GameTcpServerConnection)sender;
            if (e.Error)
            {
                Console.WriteLine($"Server closed connection with client during error: {e.Data}");
                CloseConnectionWithClient(clientSocket);
            }
            else if(e.Data == null)
            {
                Console.WriteLine($"Client {clientSocket.RemoteEndPoint} disconnected.");
                CloseConnectionWithClient(clientSocket);
            }
            else
            {
                var bytePacket = Packet.Deserialize((byte[])e.Data);
                Packet packet = bytePacket as Packet;

                if(packet != null)
                {
                    switch (packet.Type)
                    {
                        case PacketType.PLAYER_INFO:
                            var context = clients.Where(x => x.Key == clientSocket).FirstOrDefault().Value;
                            context.Player = new PlayerMp((PlayerShared)packet.Content);
                            PacketProtocol.SendPacket(clientSocket, new Packet
                            {
                                Content = context.Player,
                                Type = PacketType.PLAYER_INFO
                            });
                            break;
                        case PacketType.BULLET_POS:
                            break;
                        case PacketType.SYSTEM_MESSAGE:
                            break;
                        case PacketType.CHAT_MESSAGE:
                            break;
                    }
                }
            }
        }

        private void Client_SendCompleted(object sender, TcpCompletedEventArgs e)
        {
            Console.WriteLine($"Send event, bytes sent: {(int)e.Data}");
        }

        public void Start()
        {
            ServerSocket.ClientAcceptAsync();

            Console.WriteLine($"Server started on {ServerSocket.LocalEndPoint}...");
            Console.WriteLine("Type \"help\" to get commands list...");

            while (true)
            {
                var str = Console.ReadLine();

                switch (str.ToLowerInvariant())
                {
                    case "help":
                        Console.WriteLine("Command List:");
                        Console.WriteLine("test - Send test message to all connected clients.");
                        Console.WriteLine("status - Get list of all connected clients.");
                        break;
                    case "test":
                        foreach (KeyValuePair<GameTcpServerConnection, ClientContext> client in clients)
                        {
                            PacketProtocol.SendPacket(client.Key, new Packet
                            {
                                Content = "<TEST_MESSAGE>",
                                Type = PacketType.SYSTEM_MESSAGE
                            });
                        }
                        break;
                    case "status":
                        if(clients.Count > 0)
                            foreach (KeyValuePair<GameTcpServerConnection, ClientContext> client in clients)
                            {
                                Console.WriteLine($"PLAYER: {client.Value.Player.Name}; IP: {client.Key.RemoteEndPoint}; Guid: {client.Value.Player.Guid}");
                            }
                        else
                            Console.WriteLine("No one client are connected!");
                        break;
                    default:
                        Console.WriteLine($"No such command \"{str}\"!");
                        Console.WriteLine("Type \"help\" to get commands list...");
                        break;
                }
            }
        }
    }
}
