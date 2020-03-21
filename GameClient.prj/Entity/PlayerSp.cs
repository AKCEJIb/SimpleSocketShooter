using Game.Networking;
using Game.Networking.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Game.Client.Entity
{
    [Serializable]
    public class PlayerSp : PlayerShared
    {

        public PlayerSp(string name)
        {
            Name = name;
            Position = new Vector2();
        }
        public PlayerSp(string name, int health, Vector2 pos)
        {
            Name     = name;
            Health   = health;
            Position = pos;
        }

        public PlayerSp(PlayerShared ply)
        {
            Name        = ply.Name;
            Health      = ply.Health;
            Position    = ply.Position;
            Guid        = ply.Guid;
            Alive       = ply.Alive;
        }

        public void UpdatePlayer(PlayerShared ply)
        {
            Name        = ply.Name;
            Health      = ply.Health;
            Position    = ply.Position;
            Guid        = ply.Guid;
            Alive       = ply.Alive;
        }

        public void UpdatePos(Vector2 vec)
        {
            Position = vec;
        }

        public void SetName(string newName)
        {
            Name = newName;
        }

        public override bool Tick()
        {
            // pass...
            return true;
        }
    }
}
