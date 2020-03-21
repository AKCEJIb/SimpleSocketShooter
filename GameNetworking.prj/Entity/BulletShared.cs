using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Networking.Entity
{
    [Serializable]
    public abstract class BulletShared : Entity
    {
        public int Damage { get; protected set; }
        public Guid OwnerGuid { get; protected set; }

    }
}
