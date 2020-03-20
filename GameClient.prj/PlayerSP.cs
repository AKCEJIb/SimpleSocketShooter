using Game.Networking;
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

        public PlayerSp(string name)
        {
            Name = name;
            Position = new Vector2();
        }
        public PlayerSp(string name, int health, Vector2 pos)
        {
            Name = name;
            Health = health;
            Position = pos;
        }

        public PlayerSp(PlayerShared ply)
        {
            Name        = ply.Name;
            Health      = ply.Health;
            Position    = ply.Position;
            Guid        = ply.Guid;
        }

        public void UpdatePlayer(PlayerShared plyInfo)
        {
            Name        = plyInfo.Name;
            Health      = plyInfo.Health;
            Position    = plyInfo.Position;
            Guid        = plyInfo.Guid;
        }

        public void UpdatePos(Vector2 vec)
        {
            Position = vec;
        }

        public void SetName(string newName)
        {
            Name = newName;
        }

        public override void Tick()
        {
            // pass...
        }
    }
}
