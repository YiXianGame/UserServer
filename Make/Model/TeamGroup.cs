using Material.Interface;
using System.Collections.Generic;

namespace Make.Model
{
    public class TeamGroup:IMatchSystemTeam<Team>
    {
        private ICollection<Team> items = new HashSet<Team>();
        private int rank = 0;//队伍分数
        private long startMatchTime = 0;//开始匹配时间
        private int averageRank = 0;
        private int count = 0;
        public TeamGroup()
        {
            this.startMatchTime = Material.Utils.TimeStamp.Now();
        }
        public void Add(Team item)
        {
            items.Add(item);
            rank += item.Rank;
            count += item.Count;
            averageRank = count > 0 ? (int)(rank / count) : 0;
        }

        public void Remove(Team item)
        {
            if (items.Remove(item))
            {
                rank -= item.Rank;
                count -= item.Count;
                averageRank = count > 0 ? (int)(rank / count) : 0;
            }
        }

        public long StartMatchTime { get => startMatchTime; set => startMatchTime = value; }
        public int Count { get => count; set => count = value; }
        public int Rank { get => rank; set => rank = value; }
        public int AverageRank { get => averageRank; set => averageRank = value; }
        public ICollection<Team> Items { get => items; set => items = value; }
    }
}
