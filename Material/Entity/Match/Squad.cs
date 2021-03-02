using Material.Entity.Game;
using Material.Model.MatchSystem.Interface;
using System.Collections.Generic;

namespace Material.Entity.Match
{
    public class Squad : Model.MatchSystem.Entity.BaseSquad<User>
    {
        private string secretKey;
        private Room.RoomType roomType;
        private User captain;

        public string SecretKey { get => secretKey; set => secretKey = value; }

        public Room.RoomType RoomType { get => roomType; set => roomType = value; }
        public User Captain { get => captain; set => captain = value; }
        public Squad(string secretKey, Room.RoomType roomType)
        {
            this.startMatchTime = Material.Utils.TimeStamp.Now();
            this.secretKey = secretKey;
            this.roomType = roomType;
        }
        public override bool Add(User user)
        {
            lock (items)
            {
                if (count < 5)
                {
                    if (base.Add(user))
                    {
                        user.Squad = this;
                        return true;
                    }
                    else return false;
                }
                else return false;
            }
        }

        public override bool Remove(User user)
        {
            if (base.Remove(user))
            {
                user.Squad = null;
                return true;
            }
            else return false;
        }
    }
}
