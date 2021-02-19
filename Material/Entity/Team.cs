using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Material.Entity
{
    public class Team:IComparable<Team>
    {
        private List<long> users;
        private double averageRank;//队伍平均分数
        private long startMatchTime;//开始匹配时间
        private bool isBelong;

        private Team(List<long> users,double averageRank)
        {
            this.users = users;
            this.averageRank = averageRank;
            this.startMatchTime = Material.Utils.TimeStamp.Now();
        }

        public double AverageRank { get => averageRank; set => averageRank = value; }
        public long StartMatchTime { get => startMatchTime; set => startMatchTime = value; }
        public List<long> Users { get => users; set => users = value; }
        public bool IsBelong { get => isBelong; set => isBelong = value; }

        public int CompareTo([AllowNull] Team other)
        {
            if (startMatchTime < other.startMatchTime) return -1;
            else if (startMatchTime > other.startMatchTime) return 1;
            else
            {
                if (averageRank < other.averageRank) return -1;
                else return 1;
            }
        }
    }
}
