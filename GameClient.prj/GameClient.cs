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

        public event Action ConnectedToServer;
        public event Action DisconnectedFromServer;
        public event Action PlayerUpdated;
        public event Action<string> CriticalErrorOccured;

        private GameTcpClient ClientSocket;
        private PacketProtocol PacketProtocol;
        private bool _connected;
        private Queue<Vector2> _moveQueue = new Queue<Vector2>();
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
            World = World.GetInstance();

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
                ClientSocket?.ShutdownAsync();
                Console.WriteLine($"Plys: {World.Players.Count}");
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

                this.ConnectedToServer();

                _connected = true;
            }
            else
            {
                CriticalErrorOccured($"Удалённый сервер не отвечает! {e.Data}");
            }
        }
        public void MovePlayer(Vector2 pos)
        {
            _moveQueue.Enqueue(pos);
        }
        public void SendMoveQueue()
        {
            while(_moveQueue.Count > 0)
            {
                PacketProtocol.SendPacket(ClientSocket, new Packet
                {
                    Type = PacketType.PLAYER_MOVE,
                    Content = _moveQueue.Dequeue()
                });
            }
        }
        private void PacketProtocol_PacketArrived(object sender, TcpCompletedEventArgs e)
        {
            if (!e.Error)
            {
                var bytePacket = Packet.Deserialize((byte[])e.Data);
                var packet = bytePacket as Packet;
                if (packet != null)
                {
                    switch (packet.Type)
                    {
                        case PacketType.PLAYER_INFO:
                            World.LocalPlayer.UpdatePlayer((PlayerShared)packet.Content);
                            break;
                        case PacketType.PLAYER_STATE:
                            var player = (PlayerShared)packet.Content;
                            var playerToUpdate = World.Players.Where(x => x.Guid == player.Guid).FirstOrDefault();

                            if (playerToUpdate != null)
                            {
                                playerToUpdate.UpdatePlayer(player);
                                //Console.WriteLine($"Updated player {playerToUpdate.Name}:{playerToUpdate.Guid}");
                            }
                            else
                            {
                                World.Players.Add(new PlayerSp(player));
                                Console.WriteLine("Added new player...");
                            }
                            break;
                        case PacketType.PLAYER_DISCONNECTED:
                            var playerDis = (PlayerShared)packet.Content;
                            var playerToDis = World.Players.Where(x => x.Guid == playerDis.Guid).FirstOrDefault();

                            if (playerToDis != null)
                            {
                                World.Players.Remove(playerToDis);
                            }
                            
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
                CriticalErrorOccured($"Удалённый сервер принудительно разовал соединение! {e.Data}");
            }
        }
    }
}
