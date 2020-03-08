using GameShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameClient
{
    public class Client
    {
        private Socket ClientSocket { get; set; }
        private static int BUFFER_SIZE = 1024;
        private byte[] _buffer = new byte[BUFFER_SIZE];
        private IPEndPoint _iPEndPoint;
        public Client(
            string ip,
            int port)
        {
            _iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            ClientSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, 
                ProtocolType.Tcp);
        }
        public void Connect()
        {
            if (!IsSocketAvailable(ClientSocket))
            {
                try
                {
                    ClientSocket.Connect(_iPEndPoint);

                    ClientSocket.BeginReceive(
                        _buffer,
                        0,
                        BUFFER_SIZE,
                        SocketFlags.None,
                        new AsyncCallback(OnReceivePacket),
                        ClientSocket);

                    Console.WriteLine($"Connected to server {_iPEndPoint}");
                }
                catch(SocketException ex)
                {
                    throw new ConnectionException(ex.Message);
                }
            }
        }
        private bool IsSocketAvailable(Socket s)
        {
            var par1 = s.Poll(1000, SelectMode.SelectRead);
            var par2 = (s.Available == 0);
            var par3 = s.Connected;
            if ((par1 && par2) || !par3)
                return false;
            else
                return true;
        }
        private void OnReceivePacket(IAsyncResult ar)
        {
            Socket server = (Socket)ar.AsyncState;

            int bytesRead = 0;
            try
            {
                bytesRead = server.EndReceive(ar);
            }
            catch 
            { 
                // Ignored...
            }
            finally
            {
                if (bytesRead > 0)
                {
                    server.BeginReceive(_buffer, 0, 1024, 0,
                        new AsyncCallback(OnReceivePacket), server);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Received {bytesRead} bytes from server.");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("Server was closed!");
                }
            }
        }
        public void Send(byte[] bytes)
        {
            if (!IsSocketAvailable(ClientSocket))
                return;
            ClientSocket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(OnSendPacket), ClientSocket);
        }
        public void SendPacket(Packet packet)
        {
            if (!IsSocketAvailable(ClientSocket))
                return;

            var sPacket = packet.Serialize();

            ClientSocket.BeginSend(sPacket, 0, sPacket.Length, SocketFlags.None, new AsyncCallback(OnSendPacket), ClientSocket);
        }

        private void OnSendPacket(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                int bytesSent = client.EndSend(ar);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Sent {bytesSent} bytes to server.");
                Console.ResetColor();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void CloseConnection()
        {
            if (IsSocketAvailable(ClientSocket))
            {
                ClientSocket.Shutdown(SocketShutdown.Both);
                ClientSocket.Close();
            }
        }

        public class ConnectionException : Exception
        {
            public ConnectionException(string message) : base(message)
            {
            }
        }
    }
    
}
