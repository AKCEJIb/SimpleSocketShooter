using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Networking
{
    [Serializable]
    public class Vector2
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Vector2()
        {
            X = 0;
            Y = 0;
        }
        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool InRange(Vector2 upperLeft, Vector2 bottomRight)
        {
            return this.X >= upperLeft.X 
                && this.Y >= upperLeft.Y
                && this.X <= bottomRight.X
                && this.Y <= bottomRight.Y;
        }
    }
}
