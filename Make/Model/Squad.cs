using Material.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Make.Model
{
    public class Squad : IMatchSystemTeam<UserToken>
    {
        private ICollection<UserToken> items = new HashSet<UserToken>();
        private int rank = 0;//队伍分数
        private long startMatchTime = 0;//开始匹配时间
        private int averageRank = 0;
        private int count = 0;
        public long StartMatchTime { get => startMatchTime; set => startMatchTime = value; }
        public int Count { get => count; set => count = value; }
        public int Rank { get => rank; set => rank = value; }
        public int AverageRank { get => averageRank; set => averageRank = value; }
        public Squad()
        {
            this.startMatchTime = Material.Utils.TimeStamp.Now();
        }
        public void Add(UserToken item)
        {
            items.Add(item);
            rank += item.Rank;
            count += item.Count;
            averageRank = count > 0 ? (int)(rank / count) : 0;
        }

        public void Remove(UserToken item)
        {
            if (items.Remove(item))
            {
                rank -= item.Rank;
                count -= item.Count;
                averageRank = count > 0 ? (int)(rank / count) : 0;
            }
        }

        public ICollection<UserToken> Items { get => items; set => items = value; }
    }
}
