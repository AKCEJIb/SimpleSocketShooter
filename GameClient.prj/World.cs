using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Client
{
    public class World
    {
        private static World Instance { get; set; }
        public List<PlayerSp> Players { get; private set; }
        private World()
        {
            Players = new List<PlayerSp>();
        }

        public static World GetInstance()
        {
            return Instance ?? (Instance = new World());
        }

        public void AddPlayer(PlayerSp player)
        {
            Players.Add(player);
        }
    }
}
