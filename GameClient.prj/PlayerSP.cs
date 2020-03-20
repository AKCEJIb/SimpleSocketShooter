using Game.Networking.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Game.Client
{
    [Serializable]
    public class PlayerSp : PlayerShared
    {
        public PlayerSp(string name, int health, int posX, int posY)
        {
            Name = name;
            Health = health;
            PosX = posX;
            PosY = posY;
        }


        public void UpdatePlayer(PlayerShared plyInfo)
        {
            Name = plyInfo.Name;
            PosX = plyInfo.PosX;
            PosY = plyInfo.PosY;
            Guid = plyInfo.Guid;

            Console.WriteLine($"Player info get: {Guid}");
        }

        public void SetName(string newName)
        {
            Name = newName;
        }

        
    }
}
