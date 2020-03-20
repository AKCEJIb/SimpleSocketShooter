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
        PLAYER_INFO,            // Тип пакета для отправки состояния конкретного игрока при соединении или обновлении информации
        PLAYER_DISCONNECTED,    // Тип пакета для отправки состояния конкретного игрока при отсодинении
        PLAYER_STATE,           // Тип пакета для отправки состояния остальных игроков
        PLAYER_MOVE,            // Тип пакета для отправки попытки движения

        BULLET_STATE,           // Тип пакета для отправки данных о пулях
        
        SYSTEM_MESSAGE,         // Тип пакета для системных сообщений
        CHAT_MESSAGE,           // Тип пакета для сообщений чата
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

        public static byte[] Serialize(Packet packet)
        {
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, packet);
                return ms.ToArray();
            }
        }

        public static object Deserialize(byte[] bytes)
        {
            using(var ms = new MemoryStream(bytes))
            {
                var bf = new BinaryFormatter();
                return (Packet)bf.Deserialize(ms);
            }
        }
    }
}
