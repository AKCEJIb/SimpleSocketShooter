using Game.Networking;
using Game.Networking.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Game.Server
{
    [Serializable]
    public class PlayerMp : PlayerShared
    {
        [NonSerialized]
        private GameServer _server = GameServer.GetInstance();
        [NonSerialized]
        private Random _rand = new Random();
        public PlayerMp(PlayerShared ply)
        {
            Name = ply.Name;
            Health = 100;
            Position = new Vector2(_rand.Next(-466 / 2, 466 / 2), _rand.Next(-351 / 2, 351 / 2));
            Guid = Guid.NewGuid();


        }
        public override void Tick()
        {
            //Console.WriteLine($"{Guid} TICKED!");
            UpdatePlayer();
            UpdateSelfPlayer();
        }

        internal void SetName(string name)
        {
            Name = name;
        }

        internal void SetPos(Vector2 pos)
        {
            Position = pos;
        }

        internal void AddPos(Vector2 pos)
        {
            Position = new Vector2(Position.X + pos.X, Position.Y + pos.Y);
        }

        private void UpdateSelfPlayer()
        {
            _server.SendPacketByGuid(new Packet
            {
                Type = PacketType.PLAYER_INFO,
                Content = this
            }, this.Guid);
        }

        public void UpdatePlayer()
        {
            _server.BroadcastPacket(new Packet
            {
                Type = PacketType.PLAYER_STATE,
                Content = this
            }, this.Guid);
        }

        public void SendDisconnect()
        {
            _server.BroadcastPacket(new Packet
            {
                Type = PacketType.PLAYER_DISCONNECTED,
                Content = this
            }, this.Guid);
        }
    }
}
