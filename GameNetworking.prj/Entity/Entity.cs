using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Networking.Entity
{
    [Serializable]
    public abstract class Entity : ITickingEntity
    {
        public Guid Guid { get; protected set; }
        public Vector2 Position { get; protected set; }
        public abstract bool Tick();
    }
}
