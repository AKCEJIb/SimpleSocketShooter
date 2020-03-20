using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Game.Networking.Entity
{
    [Serializable]
    public abstract class PlayerShared
    {
        public string Name { get; protected set; }
        public int Health { get; protected set; }
        public int PosX { get; protected set; }
        public int PosY { get; protected set; }

        public Guid Guid{ get; protected set; }

        public override string ToString()
        {
            return "Player{" +
                $"name={Name}" +
                $", health={Health}" +
                $", posX={PosX}" +
                $", posY={PosY}" +
                "}";
        }
    }
}
