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
using System.Threading.Tasks;
using System.Timers;

namespace Game.Server
{
   
    public class GameServer
    {
        private static GameServer Instance { get; set; }
        private Timer _timer = new Timer();
        public static GameServer GetInstance()
        {
            return Instance ?? (Instance = new GameServer());
        }

        List<ITickingEntity> _tickingEntities = new List<ITickingEntity>();


        internal class ClientContext
        {
            public PacketProtocol PacketProtocol { get; set; }
            public PlayerMp Player { get; set; }
            public bool Connected;
        }

        private GameTcpServer ServerSocket;

        internal Dictionary<GameTcpServerConnection, ClientContext> clients =
            new Dictionary<GameTcpServerConnection, ClientContext>();
        
        private GameServer()
        {
            ServerSocket = new GameTcpServer();

        }

        public void SetPort(int port)
        {
            ServerSocket.Bind(port, 2);
            ServerSocket.AcceptCompleted += ServerSocket_AcceptCompleted;
        }
        public void SetAddress(IPAddress ip, int port)
        {
            ServerSocket.Bind(ip, port, 2);
            ServerSocket.AcceptCompleted += ServerSocket_AcceptCompleted;
        }

        internal void SendPacketByGuid(Packet packet, Guid guid)
        {
            var client = clients.Where(x => x.Value.Player.Guid == guid).FirstOrDefault();

            PacketProtocol.SendPacket(client.Key, packet);
        }

        internal void BroadcastPacket(Packet packet, Guid excludeGuid)
        {
            foreach (KeyValuePair<GameTcpServerConnection, ClientContext> client in clients)
            {
                if (client.Value.Player.Guid == excludeGuid)
                    continue;
                PacketProtocol.SendPacket(client.Key, packet);
            }
        }

        private void CloseConnectionWithClient(GameTcpServerConnection socket)
        {
            ClientContext clientContext;
            clients.TryGetValue(socket, out clientContext);

            if(clientContext != null)
            {
                clientContext.Player.SendDisconnect();

                _tickingEntities.Remove(clientContext.Player);
            }

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
            //client.SendCompleted += Client_SendCompleted;

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

                            _tickingEntities.Add(context.Player);

                            break;
                        case PacketType.SYSTEM_MESSAGE:
                            break;
                        case PacketType.CHAT_MESSAGE:
                            break;
                        case PacketType.PLAYER_STATE:
                            break;
                        case PacketType.PLAYER_MOVE:
                            var contextMove = clients.Where(x => x.Key == clientSocket).FirstOrDefault().Value;
                            contextMove.Player.AddPos((Vector2)packet.Content); 
                            break;
                        case PacketType.BULLET_STATE:
                            break;
                        default:
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

            _timer.Interval = 1000 / 64;
            _timer.AutoReset = true;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();

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
                    case "setname":
                        Console.Write("Rename player: ");
                        var oldName = Console.ReadLine();

                        var user = clients.Values.Where(x => x.Player.Name.Contains(oldName)).FirstOrDefault();

                        if (user != null) { 
                            Console.Write("New player name: ");
                            var newName = Console.ReadLine();
                            user.Player.SetName(newName);
                            Console.WriteLine($"Player \"{oldName}\" renamed successfully to \"{newName}\"");
                        }
                        else
                            Console.WriteLine("User not found!");

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

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var entity in _tickingEntities)
            {
                entity.Tick();
            }
        }
    }
}
