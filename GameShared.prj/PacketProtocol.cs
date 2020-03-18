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


        private byte[] lenghtBuffer;
        private byte[] dataBuffer;
        private int bytesReceived;

        public IAsyncSocket Socket { get; private set; }

        public PacketProtocol(IAsyncSocket socket)
        {
            this.Socket = socket;
            this.lenghtBuffer = new byte[sizeof(int)];
        }
      
    }
}
