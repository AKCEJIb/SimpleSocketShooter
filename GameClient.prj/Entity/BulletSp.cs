using Game.Networking.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Client.Entity
{
    [Serializable]
    public class BulletSp : BulletShared
    {
        public void Update(BulletShared bullet)
        {
            Damage = bullet.Damage;
            Position = bullet.Position;
        }
        public BulletSp(BulletShared bullet)
        {
            Damage = bullet.Damage;
            Position = bullet.Position;
            Guid = bullet.Guid;
        }
        public override bool Tick()
        {
            // pass ...

            return true;
        }
    }
}
