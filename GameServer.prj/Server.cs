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

namespace GameServer
{

    internal class SocketData
    {
        public Socket SocketConnection { get; set; }
        public const int BUFFER_SIZE = 1024;
        public byte[] buffer = new byte[BUFFER_SIZE];
    }

    public class Server
    {
        private Socket ServerSocket { get; set; }
        private List<PlayerMP> Players { get; set; }

        public Server(string ip, int port)
        {

            Players = new List<PlayerMP>();

            ServerSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            var bindIp = new IPEndPoint(
                IPAddress.Parse(ip),
                port);

            ServerSocket.Bind(bindIp);

        }

       
        public void Start()
        {
            ServerSocket.Listen(10);
            ServerSocket.BeginAccept(new AsyncCallback(this.OnNewClient), ServerSocket);
            var lEndPoint = (IPEndPoint)ServerSocket.LocalEndPoint;
            Console.WriteLine($"Server started on {lEndPoint.Address}:{lEndPoint.Port}");

            while (true)
            {
                var command = Console.ReadLine();
                switch(command.ToLower())
                {
                    case "status":
                        Console.WriteLine($"Server works on {lEndPoint.Port} port");
                        Console.WriteLine($"Now server has {Players.Count} players:");
                        foreach (var item in Players)
                        {
                            Console.WriteLine(" - " + (IPEndPoint)item.Connection.SocketConnection.RemoteEndPoint);
                        }

                        break;
                    case "test":
                        foreach (var item in Players)
                        {
                            byte[] data = Encoding.Unicode.GetBytes("Hello, World! :)");
                            item.Connection.SocketConnection.Send(data);
                        }

                        Console.WriteLine($"Sending \"Hello World\" message to all clients!" +
                            $" {Players.Count} players are recieved message!");
                        break;
                    case "help":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Command List:");
                        Console.WriteLine("status - Get info about current connections.");
                        Console.WriteLine("test - Send a test message to all players.");
                        Console.ResetColor();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Unknown command \"{command.ToLower()}\". Type \"help\" to get list of all available commands.");
                        Console.ResetColor();
                        break;
                }
            }
        }
        
        private void OnNewClient(IAsyncResult ar)
        {
            try
            {
                var userSocket = ServerSocket.EndAccept(ar);
                var whoAre = (IPEndPoint)userSocket.RemoteEndPoint;

                Console.WriteLine($"Connected {whoAre}");

                var socketData = new SocketData();
                socketData.SocketConnection = userSocket;

                Players.Add(new PlayerMP(socketData));

                userSocket.BeginReceive(
                    socketData.buffer,
                    0,
                    SocketData.BUFFER_SIZE,
                    SocketFlags.None, 
                    new AsyncCallback(this.OnPacketRecive),
                    socketData);

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ServerSocket.BeginAccept(new AsyncCallback(this.OnNewClient), ServerSocket);
        }

        private void OnPacketRecive(IAsyncResult ar)
        {
            var so = (SocketData)ar.AsyncState;
            var s = so.SocketConnection;
            int read = 0;
            try
            {
                read = s.EndReceive(ar);
                Console.WriteLine($"{(IPEndPoint)s.RemoteEndPoint} sent {read} bytes");

                var bf = new BinaryFormatter();
                var packet = (GameShared.Packet)bf.Deserialize(new MemoryStream(so.buffer));

                switch (packet.Type)
                {
                    case GameShared.PacketType.PLAYER:
                        Console.WriteLine($"Server get player {(GameShared.PlayerSP)packet.Content}");
                        break;
                    case GameShared.PacketType.BULLET:
                        break;
                    case GameShared.PacketType.MESSAGE:
                        Console.WriteLine($"Server get message: \"{(string)packet.Content}\"");
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                // Ignored...
            }
            finally
            {
                if (read > 0)
                {
                    s.BeginReceive(so.buffer,
                        0,
                        SocketData.BUFFER_SIZE,
                        SocketFlags.None,
                        new AsyncCallback(this.OnPacketRecive),
                        so);
                }
                else
                {
                    var whoAre = (IPEndPoint)so.SocketConnection.RemoteEndPoint;
                    Console.WriteLine($"Disconnected {whoAre}");

                    Players.Remove(Players.Where(x=>x.Connection.Equals(so)).FirstOrDefault());

                    s.Close();
                }
            }
        }
    }
}
