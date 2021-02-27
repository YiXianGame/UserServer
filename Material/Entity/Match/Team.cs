﻿using Material.Entity;
using Material.Interface;
using Material.RPCServer.TCP_Async_Event;
using System.Collections.Generic;

namespace Material.Entity.Match
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
        public bool Add(Squad item)
        {
            items.Add(item);
            foreach(User user in item.Items)
            {
                user.Team = this;
            }
            rank += item.Rank;
            count += item.Count;
            averageRank = count > 0 ? (int)(rank / count) : 0;
            return true;
        }

        public bool Remove(Squad item)
        {
            if (items.Remove(item))
            {
                foreach (User user in item.Items)
                {
                    user.Team = null;
                }
                rank -= item.Rank;
                count -= item.Count;
                averageRank = count > 0 ? (int)(rank / count) : 0;
                return true;
            }
            else return false;
        }

        public ICollection<Squad> Items { get => items; set => items = value; }
    }
}