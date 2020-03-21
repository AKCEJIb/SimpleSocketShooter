using Game.Networking;
using Game.Networking.Entity;
using Game.Server.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        internal const double TICKRATE = 1000 / 64;
        internal int PlayerSpeed { get; set; } = 5;

        private static GameServer Instance { get; set; }
        private Timer _timer = new Timer();

        public static GameServer GetInstance()
        {
            return Instance ?? (Instance = new GameServer());
        }

        private List<ITickingEntity> _tickingEntities = new List<ITickingEntity>();


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
                    var context = clients.Where(x => x.Key == clientSocket).FirstOrDefault().Value;
                    switch (packet.Type)
                    {
                        case PacketType.PLAYER_INFO:
                            context.Player = new PlayerMp((PlayerShared)packet.Content);
                            PacketProtocol.SendPacket(clientSocket, new Packet
                            {
                                Content = context.Player,
                                Type = PacketType.PLAYER_INFO
                            });

                            BroadcastPacket(new Packet
                            {
                                Type = PacketType.MESSAGE_SYSTEM,
                                Content = $"connected>{context.Player.Name}"
                            }, context.Player.Guid);

                            _tickingEntities.Add(context.Player);

                            break;
                        case PacketType.MESSAGE_CHAT:
                            BroadcastPacket(packet, Guid.Empty);
                            break;
                        case PacketType.PLAYER_MOVE:
                            context.Player.AddPos((Vector2)packet.Content); 
                            break;
                        case PacketType.BULLET_SHOOT:
                            var bullet = new BulletMp((Vector2)packet.Content, context.Player.Position, context.Player.Guid);
                            _tickingEntities.Add(bullet);
                            break;
                        case PacketType.PLAYER_REVIVE:
                            context.Player.Spawn();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void Start()
        {
            ServerSocket.ClientAcceptAsync();

            _timer.Interval = TICKRATE;
            _timer.AutoReset = true;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();

            Console.WriteLine($"Server v{typeof(GameServer).Assembly.GetName().Version} started on {ServerSocket.LocalEndPoint}...");
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
                        Console.WriteLine("rename - Rename player with specified nickname.");
                        break;
                    case "test":
                        BroadcastPacket(new Packet
                        {
                            Content = "msg>SERVER_TEST_MESSAGE",
                            Type = PacketType.MESSAGE_SYSTEM
                        }, Guid.Empty);
                        break;
                    case "rename":
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
        private void CalculateBulletHit(BulletMp bullet)
        {
            foreach (var entity in _tickingEntities)
            {
                var player = entity as PlayerMp;
                if(player != null)
                {
                    if (bullet.OwnerGuid == player.Guid)
                        continue;

                    var leftUp = new Vector2(player.Position.X - 5, player.Position.Y - 5);
                    var rightBott = new Vector2(player.Position.X + 5, player.Position.Y + 5);

                    if (bullet.Position.InRange(leftUp, rightBott)
                        && player.Alive)
                    {
                        player.Hit(bullet.Damage);
                        bullet.Delete();
                        return;
                    }
                       
                }
            }
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //var sw = new Stopwatch();
            //sw.Start();
            //sw.Restart();
            foreach (var entity in _tickingEntities)
            {
                var bullet = entity as BulletMp;
                if (bullet != null)
                    CalculateBulletHit(bullet);
                if (!entity.Tick()) _tickingEntities.Remove(entity);
            }
            //sw.Stop();

            //Console.WriteLine($"Packets sent for: {sw.Elapsed.TotalMilliseconds}");
        }
    }
}
