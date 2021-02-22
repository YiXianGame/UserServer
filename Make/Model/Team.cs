using Material.Interface;
using Material.RPCServer.TCP_Async_Event;
using System.Collections.Generic;

namespace Make.Model
{
    public class Team:IMatchSystemTeam<Squad>
    {
        private ICollection<Squad> items = new HashSet<Squad>();
        private int rank = 0;//队伍分数
        private long startMatchTime = 0;//开始匹配时间
        private int averageRank = 0;
        private int count = 0;
        public long StartMatchTime { get => startMatchTime; set => startMatchTime = value; }
        public int Count { get => count; set => count = value; }
        public int Rank { get => rank; set => rank = value; }
        public int AverageRank { get => averageRank; set => averageRank = value; }
        public Team()
        {
            this.startMatchTime = Material.Utils.TimeStamp.Now();
        }
        public void Add(Squad item)
        {
            items.Add(item);
            rank += item.Rank;
            count += item.Count;
            averageRank = count > 0 ? (int)(rank / count) : 0;
        }

        public void Remove(Squad item)
        {
            if (items.Remove(item))
            {
                rank -= item.Rank;
                count -= item.Count;
                averageRank = count > 0 ? (int)(rank / count) : 0;
            }
        }

        public ICollection<Squad> Items { get => items; set => items = value; }
    }
}
