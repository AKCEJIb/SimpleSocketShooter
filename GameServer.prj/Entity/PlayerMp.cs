using Game.Networking;
using Game.Networking.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Game.Server.Entity
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
            Alive = true;
            Name = ply.Name;
            Health = 100;
            Position = new Vector2(_rand.Next(-100 / 2, 100 / 2), _rand.Next(-100 / 2, 100 / 2));
            Guid = Guid.NewGuid();


        }
        public override bool Tick()
        {
            //Console.WriteLine($"{Guid} TICKED!");
            UpdatePlayer();
            UpdateSelfPlayer();

            return true;
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
            Position = new Vector2(Position.X + pos.X * _server.PlayerSpeed, Position.Y + pos.Y * _server.PlayerSpeed);
        }

        internal void Spawn(Vector2 pos)
        {
            Health = 100;
            Alive = true;
            Position = pos;
        }
        internal void Spawn()
        {
            Health = 100;
            Alive = true;
            Position = new Vector2(_rand.Next(-100 / 2, 100 / 2), _rand.Next(-100 / 2, 100 / 2));
        }
        internal void Hit(int hp)
        {
            Health -= hp;

            // Уведомляем всех, что урон был получен (для отрисовки например крови)
            _server.BroadcastPacket(new Packet
            {
                Type = PacketType.PLAYER_HIT,
                Content = hp
            }, Guid.Empty);

            if (Health <= 0)
                Kill();
        }

        internal void Kill()
        {
            Health = 0;
            Alive = false;

            // Уведомляем всех, что игрок умер :(
            _server.BroadcastPacket(new Packet
            {
                Type = PacketType.PLAYER_DEAD,
                Content = this.Name
            }, Guid.Empty);
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
