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
    public class BulletMp : BulletShared
    {
        [NonSerialized]
        private GameServer _server = GameServer.GetInstance();
        [NonSerialized]
        private Random _rand = new Random();
        [NonSerialized]
        private Vector2 _dir = new Vector2();
        [NonSerialized]
        Timer timer = new Timer();
        [NonSerialized]
        private const int BULLET_SPEED = 10;
        [NonSerialized]
        private const int BULLET_LIFE = 1000;
        [NonSerialized]
        private bool _alive = true;
        public BulletMp(Vector2 direction, Vector2 startPos, Guid ownerGuid)
        {
            OwnerGuid = ownerGuid;
            Position = new Vector2(startPos.X, startPos.Y);
            Damage = _rand.Next(10, 15);
            _dir = direction;
            Guid = Guid.NewGuid();
            
            timer.Interval = BULLET_LIFE;
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Delete();
        }
        internal void Delete()
        {
            _alive = false;

            _server.BroadcastPacket(new Packet
            {
                Type = PacketType.BULLET_REMOVE,
                Content = this
            }, Guid.Empty);
        }
        public override bool Tick()
        {
            if (!_alive)
                return false;

            Position = new Vector2(Position.X + _dir.X * BULLET_SPEED, Position.Y + _dir.Y * BULLET_SPEED);

            _server.BroadcastPacket(new Packet
            {
                Type = PacketType.BULLET_STATE,
                Content = this
            }, Guid.Empty);

            return _alive;
        }
    }
}
