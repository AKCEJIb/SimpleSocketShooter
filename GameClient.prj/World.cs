using Game.Client.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Client
{
    public class World
    {
        private static World Instance { get; set; }
        public List<PlayerSp> Players { get; private set; }
        public List<BulletSp> Bullets { get; private set; }

        private List<Networking.Entity.ITickingEntity> _entitiesForRemove;
        public PlayerSp LocalPlayer { get; internal set; }

        private static readonly Font fontArial = new Font("Arial", 8, FontStyle.Bold);
        private static readonly StringFormat _sf = new StringFormat(StringFormat.GenericDefault);
        private World()
        {
            Players = new List<PlayerSp>();
            Bullets = new List<BulletSp>();

            _entitiesForRemove = new List<Networking.Entity.ITickingEntity>();

            _sf.Alignment     = StringAlignment.Center;
            _sf.LineAlignment = StringAlignment.Center;
        }

        public static World GetInstance()
        {
            return Instance ?? (Instance = new World());
        }

        public void AddPlayer(PlayerSp player)
        {
            Players.Add(player);
        }

        public void MarkRemoved(Networking.Entity.ITickingEntity entity)
        {
            _entitiesForRemove.Add(entity);
        }

        private void AcceptRemove()
        {
            foreach(var ent in _entitiesForRemove)
            {
                var player = ent as PlayerSp;
                var bullet = ent as BulletSp;

                if (player != null)
                    this.Players.Remove(player);
                if (bullet != null)
                    this.Bullets.Remove(bullet);
            }
        }

        public void DrawWorld(object s, System.Windows.Forms.PaintEventArgs e)
        {
            var p = s as System.Windows.Forms.Panel;
            var g = e.Graphics;

            foreach (var ply in Players)
            {
                var color = (ply.Equals(LocalPlayer) ? Brushes.Blue : Brushes.Red);
                if (!ply.Alive)
                    color = Brushes.Gray;

                var newPosX = ply.Position.X + p.Width / 2;
                var newPosY = ply.Position.Y + p.Height / 2;

                g.DrawString($"{ply.Name}\n{ply.Health}HP",
                    fontArial,
                    color, newPosX, newPosY - 20, _sf);

                g.FillRectangle(color,
                    newPosX - 5,
                    newPosY - 5, 10, 10);

            }

            foreach (var bullet in Bullets)
            {
                var newPosX = bullet.Position.X + p.Width / 2;
                var newPosY = bullet.Position.Y + p.Height / 2;

                g.FillRectangle(Brushes.Black,
                    newPosX - 2,
                    newPosY - 2, 4, 4);
            }

            AcceptRemove();
        }
       
    }
}
