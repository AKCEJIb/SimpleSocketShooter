using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GameShared
{
    public enum PacketType
    {
        PLAYER = 10,
        BULLET = 20,
        MESSAGE = 30
    }

    [Serializable]
    public class Packet
    {
        public PacketType Type { get; set; }
        public object Content { get; set; }

        public byte[] Serialize()
        {
            var bf = new BinaryFormatter();

            using(var ms = new MemoryStream())
            {
                bf.Serialize(ms, this);
                return ms.ToArray();
            }
        }
    }
}
