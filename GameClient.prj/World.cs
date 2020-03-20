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
        public PlayerSp LocalPlayer { get; internal set; }

        private static readonly Font fontArial = new Font("Arial", 8, FontStyle.Bold);
        private static readonly StringFormat _sf = new StringFormat(StringFormat.GenericDefault);
        private World()
        {
            Players = new List<PlayerSp>();
            _sf.Alignment = StringAlignment.Center;
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

        public void DrawWorld(object s, System.Windows.Forms.PaintEventArgs e)
        {
            var p = s as System.Windows.Forms.Panel;
            var g = e.Graphics;

            foreach (var ply in Players)
            {
                var color = (ply.Equals(LocalPlayer) ? System.Drawing.Brushes.Blue : System.Drawing.Brushes.Red);
                var newPosX = ply.Position.X + p.Width / 2;
                var newPosY = ply.Position.Y + p.Height / 2;

                g.DrawString($"{ply.Name} {ply.Health}HP",
                    fontArial,
                    color, newPosX + 5, newPosY - 10, _sf);

                g.FillRectangle(color,
                    newPosX,
                    newPosY, 10, 10);

            }

        }
       
    }
}
