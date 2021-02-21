using Material.RPCServer.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Material.Entity
{
    public class Team:IComparable<Team>
    {
        private List<BaseUserToken> users;
        private double averageRank;//队伍平均分数
        private long startMatchTime;//开始匹配时间
        private bool isCheck;
        static Random random = new Random();
        public Team(List<BaseUserToken> users,double averageRank)
        {
            this.users = users;
            this.averageRank = averageRank;
            this.startMatchTime = Utils.TimeStamp.Now();
        }

        public double AverageRank { get => averageRank; set => averageRank = value; }
        public long StartMatchTime { get => startMatchTime; set => startMatchTime = value; }
        public List<BaseUserToken> Users { get => users; set => users = value; }
        public bool IsCheck { get => isCheck; set => isCheck = value; }

        public int CompareTo([AllowNull] Team other)
        {
            if (other == null) return -1;
            else if (this == other) return 0;
            else if (startMatchTime <= other.startMatchTime) return -1;
            else if (startMatchTime > other.startMatchTime) return 1;
            else
            {
                if (averageRank < other.averageRank) return -1;
                else return 1;
            }
        }
    }
}
