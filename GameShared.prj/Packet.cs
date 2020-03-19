using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Game.Networking
{
    public enum PacketType
    {
        PLAYER_POS = 10,
        BULLET_POS = 20,
        SYSTEM_MESSAGE = 30,
        CHAT_MESSAGE = 40
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

        public static Packet Deserialize(byte[] bytes)
        {
            using(var ms = new MemoryStream(bytes))
            {
                var bf = new BinaryFormatter();
                return (Packet)bf.Deserialize(ms);
            }
        }
    }
}
