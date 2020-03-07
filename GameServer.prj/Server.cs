using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    internal enum PacketTypes
    {
        PLAYER,
        BULLET,
        MESSAGE
    }

    internal class SocketData
    {
        public Socket SocketConnection { get; set; }
        public const int BUFFER_SIZE = 1024;
        public byte[] buffer = new byte[BUFFER_SIZE];
    }

    public class Server
    {
        private Socket ServerSocket { get; set; }
        private List<SocketData> Connections { get; set; }

        public Server(string ip, int port)
        {

            Connections = new List<SocketData>();

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
                        Console.WriteLine($"Now server has {Connections.Count} players:");
                        foreach (var item in Connections)
                        {
                            Console.WriteLine("- " + (IPEndPoint)item.SocketConnection.RemoteEndPoint);
                        }

                        break;
                    case "helloworld":
                        foreach (var item in Connections)
                        {
                            byte[] data = Encoding.Unicode.GetBytes("Hello, World! :)");
                            item.SocketConnection.Send(data);
                        }
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

                Connections.Add(socketData);

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

                    Connections.Remove(so);

                    s.Close();
                }
            }
        }
    }
}
