using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Material.Entity
{
    public class TeamGroup : IComparable<TeamGroup>
    {
        private HashSet<Team> teams = new HashSet<Team>();
        private int count;
        private double rank;
        private int averageRank;
        private double averageWaitTime;
        private bool isCheck = false;
        public HashSet<Team> Teams { get => teams; set => teams = value; }
        public int Count { get => count; set => count = value; }
        public int AverageRank { get => averageRank; set => averageRank = value; }
        public double AverageWaitTime { get => averageWaitTime; set => averageWaitTime = value; }
        public bool IsCheck { get => isCheck; set => isCheck = value; }

        public void RefreshAverageWaitTime(long now)
        {
            long wait = 0;
            foreach (Team team in teams)
            {
                wait += (now - team.StartMatchTime);
            }
            averageWaitTime = wait / count;
        }
        public void Add(Team team)
        {
            if (teams.Add(team))
            {
                count += team.Users.Count;
                rank += team.AverageRank;
                AverageRank = count > 0 ? (int)(rank / count) : 0;
                team.IsCheck = true;
            }
        }

        public void Remove(Team team)
        {
            if (teams.Remove(team))
            {
                count -= team.Users.Count;
                rank -= team.AverageRank;
                AverageRank = count > 0 ? (int)(rank / count) : 0 ;
                team.IsCheck = false;
            }
        }

        public int CompareTo([AllowNull] TeamGroup other)
        {
            if (other == null) return -1;
            else if (this == other) return 0;
            else if (averageWaitTime >= other.averageWaitTime) return -1;
            else return 1;
        }
    }
}
