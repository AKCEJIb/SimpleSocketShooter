using GameShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    internal class PlayerMP
    {
        public PlayerMP(SocketData connection)
        {
            Connection = connection;
        }

        internal SocketData Connection { get; private set; }

        private PlayerSP _playerSp;

        public void BindSpPlayer(PlayerSP ply)
        {
            _playerSp = ply;
        }
    }
}
