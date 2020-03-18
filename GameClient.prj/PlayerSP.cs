using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Client
{
    [Serializable]
    public class PlayerSP
    {
        public string Name { get; private set; }
        public int Health { get; private set; }

        public int PosX { get; private set; }
        public int PosY { get; private set; }

        public PlayerSP(string name, int health, int posX, int posY)
        {
            Name = name;
            Health = health;
            PosX = posX;
            PosY = posY;
        }

        public void SetName(string newName)
        {
            Name = newName;
        }

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
