using Material.Entity.Game;
using Material.Model.MatchSystem.Interface;
using System.Collections.Generic;

namespace Material.Model.MatchSystem.Entity
{
    public class BaseSquad<T> : IMatchSystemTeam<T> where T : IMatchSystemItem
    {
        protected ICollection<T> items = new HashSet<T>();
        protected int sumRank = 0;//队伍分数
        protected long startMatchTime = 0;//开始匹配时间
        protected int averageRank = 0;
        protected int count = 0;
        public long StartMatchTime { get => startMatchTime; set => startMatchTime = value; }
        public int Count { get => count; set => count = value; }
        public int SumRank { get => sumRank; set => sumRank = value; }
        public int AverageRank { get => averageRank; set => averageRank = value; }
        public ICollection<T> Items { get => items; set => items = value; }

        public virtual bool Add(T item)
        {
            Items.Add(item);
            SumRank += item.SumRank;
            Count += item.Count;
            AverageRank = Count > 0 ? (int)(SumRank / Count) : 0;
            return true;
        }
        public virtual bool Remove(T item)
        {
            if (Items.Remove(item))
            {
                SumRank -= item.SumRank;
                Count -= item.Count;
                AverageRank = Count > 0 ? (int)(SumRank / Count) : 0;
                return true;
            }
            else return false;
        }
    }
}
