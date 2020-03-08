using GameShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient
{
    public class World
    {
        private static World Instance { get; set; }
        public List<PlayerSP> Players { get; private set; }
        private World()
        {
            Players = new List<PlayerSP>();
        }

        public static World GetInstance()
        {
            return Instance ?? (Instance = new World());
        }

        public void AddPlayer(PlayerSP player)
        {
            Players.Add(player);
        }
    }
}
