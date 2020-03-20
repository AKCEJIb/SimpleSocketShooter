using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Game.Networking.Entity
{
    [Serializable]
    public abstract class PlayerShared : Entity
    {
        public string Name { get; protected set; }
        public int Health { get; protected set; }

        public override string ToString()
        {
            return "Player{" +
                $"name={Name}" +
                $", health={Health}" +
                $", posX={Position.X}" +
                $", posY={Position.Y}" +
                "}";
        }
    }
}
