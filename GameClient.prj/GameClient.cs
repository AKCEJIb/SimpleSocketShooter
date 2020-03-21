using Game.Client.Entity;
using Game.Networking;
using Game.Networking.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Game.Client
{
    public class GameClient
    {

        public event Action<string,string> ConnectedToServer;
        public event Action DisconnectedFromServer;
        public event Action<string> ChatMessageArrived;
        public event Action<string> CriticalErrorOccured;

        private GameTcpClient ClientSocket;
        private PacketProtocol PacketProtocol;
        private bool _connected;
        private const double TICKRATE = 1000 / 64;

        private Queue<Packet> _packetQueue = new Queue<Packet>();
        private GameClient()
        {
            World = World.GetInstance();
        }

        public World World { get; private set; }

        private static GameClient Instance { get; set; }

        public static GameClient GetInstance()
        {
            return Instance ?? (Instance = new GameClient());
        }

        public void Connect(string ipPort, string plyName)
        {
            World.LocalPlayer = new PlayerSp(plyName);

            ClientSocket = new GameTcpClient();
            ClientSocket.ConnectCompleted += ClientSocket_ConnectCompleted;
            ClientSocket.ShutdownCompleted += ClientSocket_ShutdownCompleted;

            var splited = ipPort.Split(':');

            ClientSocket.ConnectAsync(
                new IPEndPoint(
                    IPAddress.Parse(splited[0]),
                    int.Parse(splited[1])));
        }

        public void Disconnect()
        {
            if (_connected)
            {
                World.Players.Clear();
                World.Bullets.Clear();
                ClientSocket?.ShutdownAsync();
            }
        }

        private void ClientSocket_ShutdownCompleted(object sender, TcpCompletedEventArgs e)
        {
            Console.WriteLine("Disconnected from server.");

            PacketProtocol.PacketArrived -= PacketProtocol_PacketArrived;

            ClientSocket.ConnectCompleted -= ClientSocket_ConnectCompleted;
            ClientSocket.ShutdownCompleted -= ClientSocket_ShutdownCompleted;

            ClientSocket.Close();
            ClientSocket = null;
            PacketProtocol = null;

            _connected = false;

            this.DisconnectedFromServer();
        }

        private void ClientSocket_ConnectCompleted(object sender, TcpCompletedEventArgs e)
        {
            if (!e.Error)
            {
                Console.WriteLine($"Connected to {(IPEndPoint)e.Data}");
                PacketProtocol = new PacketProtocol(ClientSocket);
                PacketProtocol.PacketArrived += PacketProtocol_PacketArrived;
                PacketProtocol.Start();

                Console.WriteLine("Sending client info...");

                PacketProtocol.SendPacket(ClientSocket, new Packet
                {
                    Content = World.LocalPlayer,
                    Type = PacketType.PLAYER_INFO
                });

                World.AddPlayer(World.LocalPlayer);

                ConnectedToServer?.Invoke(ClientSocket.RemoteEndPoint.ToString(), World.LocalPlayer.Name);

                _connected = true;
            }
            else
            {
                CriticalErrorOccured($"Произошла критическая ошибка!\n{((Exception)e.Data).Message}");
            }
        }

        private void LimitPacketCount()
        {
            if (_packetQueue.Count + 1 > TICKRATE)
            {
                Console.WriteLine("Packet overflow!");
                _packetQueue.Dequeue();
            }
        }

        public void EnqueueMove(Vector2 pos)
        {
            LimitPacketCount();

            _packetQueue.Enqueue(new Packet
            {
                Type = PacketType.PLAYER_MOVE,
                Content = pos
            });
        }

        public void EnqueueRevive()
        {
            LimitPacketCount();

            _packetQueue.Enqueue(new Packet
            {
                Type = PacketType.PLAYER_REVIVE,
                Content = null
            });
        }

        public void EnqueueShoot(Vector2 dir)
        {
            LimitPacketCount();

            _packetQueue.Enqueue(new Packet
            {
                Type = PacketType.BULLET_SHOOT,
                Content = dir
            });
        }

        public void SendPacketQueue()
        {
            while(_packetQueue.Count > 0)
            {
                PacketProtocol.SendPacket(ClientSocket, _packetQueue.Dequeue());
            }
        }

        public void EnqueueChatMessage(string message)
        {
            LimitPacketCount();

            _packetQueue.Enqueue(new Packet
            {
                Type = PacketType.MESSAGE_CHAT,
                Content = $"{World.LocalPlayer.Name}: {message}\n"
            });
        }
        private void PacketProtocol_PacketArrived(object sender, TcpCompletedEventArgs e)
        {
            if (!e.Error)
            {
                var bytePacket = Packet.Deserialize((byte[])e.Data);
                var packet = bytePacket as Packet;
                if (packet != null)
                {
                    var player = packet.Content as PlayerShared;
                    var bullet = packet.Content as BulletShared;
                    PlayerSp playerToUpdate = null;
                    BulletSp bulletToUpdate = null;
                    if(player != null)
                        playerToUpdate = World.Players.Where(x => x.Guid == player.Guid).FirstOrDefault();
                    if (bullet != null)
                        bulletToUpdate = World.Bullets.Where(x => x.Guid == bullet.Guid).FirstOrDefault();
                    switch (packet.Type)
                    {
                        case PacketType.PLAYER_INFO:
                            World.LocalPlayer.UpdatePlayer(player);
                            break;
                        case PacketType.PLAYER_STATE:
                            
                            if (playerToUpdate != null)
                            {
                                playerToUpdate.UpdatePlayer(player);
                            }
                            else
                            {
                                World.Players.Add(new PlayerSp(player));
                            }
                            break;
                        case PacketType.PLAYER_DISCONNECTED:
  
                            if (playerToUpdate != null)
                            {
                                World.MarkRemoved(playerToUpdate);
                                ChatMessageArrived?.Invoke($"{playerToUpdate.Name} left server!\n");
                            }
                            
                            break;

                        case PacketType.BULLET_STATE:
                            if (bulletToUpdate != null)
                                bulletToUpdate.Update(bullet);
                            else if(bullet != null)
                            {
                                World.Bullets.Add(new BulletSp(bullet));
                            }
                            break;

                        case PacketType.BULLET_REMOVE:
                            if (bulletToUpdate != null)
                            {
                                World.MarkRemoved(bulletToUpdate);
                            }
                            break;
                        case PacketType.PLAYER_HIT:
                            break;
                        case PacketType.PLAYER_DEAD:
                            ChatMessageArrived?.Invoke($"{(string)packet.Content} умирает\n");
                            break;
                        case PacketType.MESSAGE_CHAT:
                            ChatMessageArrived?.Invoke((string)packet.Content);
                            break;
                        case PacketType.MESSAGE_SYSTEM:
                            var sysMsg = ((string)packet.Content).Split('>');
                            switch (sysMsg[0].ToLower())
                            {
                                case "connected":
                                    ChatMessageArrived?.Invoke($"{sysMsg[1]} connected to server!\n");
                                    break;
                                case "msg":
                                    ChatMessageArrived?.Invoke($"{sysMsg[1]}\n");
                                    break;
                                default:
                                    ChatMessageArrived?.Invoke($"(!) UNHANDLED SYSTEM MESSAGE (!)\n");
                                    break;
                            }
                            break;
                        default:
                            Console.WriteLine("(!) GOT UNHANDLED PACKET TYPE (!)");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine($"Server send corrupted packet!");
                }
            }
            else
            {
                CriticalErrorOccured($"Произошла критическая ошибка!\n{((Exception)e.Data).Message}");
            }
        }
    }
}
