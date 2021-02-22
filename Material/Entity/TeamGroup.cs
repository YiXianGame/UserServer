using Material.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Material.Entity
{
    public class TeamGroup<T>:IMatchSystemItem where T:IMatchSystemItem
    {
        private HashSet<T> teams = new HashSet<T>();
        private int sumRank;//队伍平均分数
        private long startMatchTime;//开始匹配时间
        private int averageRank;
        private int count;
        public HashSet<T> Teams { get => teams; set => teams = value; }
        public TeamGroup()
        {
            this.startMatchTime = Utils.TimeStamp.Now();
        }
        public void Add(T team)
        {
            if (teams.Add(team))
            {
                sumRank += team.SumRank;
                count += team.Count;
                averageRank = count > 0 ? (int)(sumRank / count) : 0;
            }
        }

        public void Remove(T team)
        {
            if (teams.Remove(team))
            {
                sumRank -= team.SumRank;
                count -= team.Count;
                averageRank = count > 0 ? (int)(sumRank / count) : 0;
            }
        }

        public long StartMatchTime { get => startMatchTime; set => startMatchTime = value; }
        public int Count { get => count; set => count = value; }
        public int SumRank { get => sumRank; set => sumRank = value; }
        public int AverageRank { get => averageRank; set => averageRank = value; }

    }
}
