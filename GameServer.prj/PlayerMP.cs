using Game.Networking.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Server
{
    [Serializable]
    public class PlayerMp : PlayerShared
    {
        public PlayerMp(PlayerShared ply)
        {
            Name = ply.Name;
            Health = 100;
            PosX = 0;
            PosY = 0;
            Guid = Guid.NewGuid();
        }
    }
}
