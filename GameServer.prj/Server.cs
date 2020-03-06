using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class Server
    {
        private Socket ServerSocket { get; set; }

        public Server(string ip, int port)
        {

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

            try
            {
                ServerSocket.Listen(10);

                while (true)
                {
                    var userSocket = ServerSocket.Accept();
                }
            }
            catch
            {
                // Ingrore..
            }
        }
    }
}
