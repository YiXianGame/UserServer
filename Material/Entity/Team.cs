using Material.Interface;
using Material.RPCServer.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Material.Entity
{
    public class Team:IMatchSystemItem
    {
        private List<BaseUserToken> users;
        private int sumRank;//队伍平均分数
        private long startMatchTime;//开始匹配时间
        private int averageRank;
        private int count;
        public Team(List<BaseUserToken> users,int sumRank)
        {
            this.users = users;
            this.startMatchTime = Utils.TimeStamp.Now();
            this.sumRank = sumRank;
            this.count = users.Count;
            AverageRank = users.Count > 0 ? (int)(sumRank / users.Count) : 0;
        }
        public List<BaseUserToken> Users { get => users; set => users = value; }
        public long StartMatchTime { get => startMatchTime; set => startMatchTime = value; }
        public int Count { get => count; set => count = value; }
        public int SumRank { get => sumRank; set => sumRank = value; }
        public int AverageRank { get => averageRank; set => averageRank = value; }
    }
}
