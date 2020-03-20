using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Game.Networking
{
    public class PacketProtocol
    {

        public event EventHandler<TcpCompletedEventArgs> PacketArrived;

        /// <summary>Заворачиваем пакет в специальный байтовый массив</summary>
        public static byte[] WrapPacket(Packet packet)
        {
            var seralizedPacket = packet.Serialize();
            byte[] packetLength = BitConverter.GetBytes(seralizedPacket.Length);

            byte[] result = new byte[packetLength.Length + seralizedPacket.Length];
            packetLength.CopyTo(result, 0);
            seralizedPacket.CopyTo(result, packetLength.Length);

            return result;
        }

        public static void SendPacket(IAsyncSocket socket, Packet packet)
        {
            socket.SendAsync(WrapPacket(packet));
        }

        private byte[] lenghtBuffer;
        private byte[] dataBuffer;
        private int bytesReceived;

        public IAsyncSocket Socket { get; private set; }

        public PacketProtocol(IAsyncSocket socket)
        {
            this.Socket = socket;
            this.lenghtBuffer = new byte[sizeof(int)];
        }

        private void ReadLoop()
        {
            if (dataBuffer != null)
                Socket.ReadAsync(dataBuffer, bytesReceived, dataBuffer.Length - bytesReceived);
            else
                Socket.ReadAsync(lenghtBuffer, bytesReceived, lenghtBuffer.Length - bytesReceived);
        }
        
        public void Start()
        {
            Socket.ReadCompleted += Socket_ReadCompleted;
            ReadLoop();
        }

        private void Socket_ReadCompleted(object sender, TcpCompletedEventArgs e)
        {
            if (!e.Error)
            {
                bytesReceived += (int)e.Data;

                if((int)e.Data == 0)
                {
                    PacketArrived?.Invoke(Socket, new TcpCompletedEventArgs());
                    return;
                }

                if(dataBuffer == null)
                {
                    if(bytesReceived != sizeof(int))
                    {
                        ReadLoop();
                    }
                    else
                    {
                        int len = BitConverter.ToInt32(lenghtBuffer, 0);
                        if(len < 0)
                        {
                            PacketArrived?.Invoke(Socket, 
                                new TcpCompletedEventArgs {
                                    Data = new System.IO.InvalidDataException(
                                        "Packet length less than zero (corrupted message)"),
                                    Error=true
                                });

                            return;
                        }
                        // Keepalive packet
                        if(len == 0)
                        {
                            bytesReceived = 0;
                            ReadLoop();
                        }
                        else
                        {
                            dataBuffer = new byte[len];
                            bytesReceived = 0;
                            ReadLoop();
                        }
                    }
                }
                else
                {
                    if(bytesReceived != dataBuffer.Length)
                    {
                        // Keep reading data
                        ReadLoop();
                    }
                    else
                    {
                        PacketArrived?.Invoke(Socket, new TcpCompletedEventArgs(dataBuffer));

                        dataBuffer = null;
                        bytesReceived = 0;
                        ReadLoop();
                    }
                }
            }
            else
            {
                PacketArrived?.Invoke(Socket, e);
            }
        }
    }
}
