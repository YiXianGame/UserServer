using Material.Entity;
using Material.Entity.Game;
using Material.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Entity.Match
{
    public class Squad : IMatchSystemTeam<User>
    {
        private ICollection<User> items = new HashSet<User>();
        private int rank = 0;//队伍分数
        private long startMatchTime = 0;//开始匹配时间
        private int averageRank = 0;
        private int count = 0;
        private string secretKey;
        private Room.RoomType roomType;
        private User captain;
        public long StartMatchTime { get => startMatchTime; set => startMatchTime = value; }
        public int Count { get => count; set => count = value; }
        public int Rank { get => rank; set => rank = value; }
        public int AverageRank { get => averageRank; set => averageRank = value; }
        public string SecretKey { get => secretKey; set => secretKey = value; }
        
        public ICollection<User> Items { get => items; set => items = value; }
        public Room.RoomType RoomType { get => roomType; set => roomType = value; }
        public User Captain { get => captain; set => captain = value; }

        public Squad(string secretKey,Room.RoomType roomType)
        {
            this.startMatchTime = Material.Utils.TimeStamp.Now();
            this.secretKey = secretKey;
            this.roomType = roomType;
        }
        public bool Add(User user)
        {
            lock (items)
            {
                if (count < 5)
                {
                    items.Add(user);
                    user.Squad = this;
                    rank += user.Rank;
                    count += user.Count;
                    averageRank = count > 0 ? (int)(rank / count) : 0;
                    return true;
                }
                else return false;
            }
        }

        public bool Remove(User user)
        {
            if (items.Remove(user))
            {
                rank -= user.Rank;
                count -= user.Count;
                averageRank = count > 0 ? (int)(rank / count) : 0;
                return true;
            }
            else return false;
        }
        
    }
}
