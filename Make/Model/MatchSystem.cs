using Material.Entity;
using Material.Entity.Game;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Make.Model
{
    public class MatchSystem
    {
        #region --字段--
        private Timer timer;
        private SortedDictionary<int, SortedSet<Team>> playerPool = new SortedDictionary<int, SortedSet<Team>>();//匹配队列
        private Room.RoomType type;
        private int needUsers = 9999;
        private SortedSet<TeamGroup> groups;
        #endregion

        #region --属性--
        public Room.RoomType Type { get => type; set => type = value; }
        public int NeedUsers { get => needUsers; set => needUsers = value; }
        #endregion

        #region --方法--
        public MatchSystem(Room.RoomType type, int needUsers)
        {
            this.type = type;
            this.needUsers = needUsers;
            timer = new Timer(new TimerCallback(MatchProcess), null, 0, 1000);
        }
        public bool Enter(Team team)
        {
            lock (playerPool)
            {
                if (playerPool.TryGetValue(team.Users.Count, out SortedSet<Team> value))
                {
                    return value.Add(team);
                }
                else
                {
                    SortedSet<Team> teams = new SortedSet<Team>();
                    if (playerPool.TryAdd(team.Users.Count, teams))
                    {
                        return teams.Add(team);
                    }
                    else return false;
                }
            }
        }
        public void MatchProcess(object sender)
        {
            long startTime = Material.Utils.TimeStamp.Now();
            Console.WriteLine("执行匹配开始|开始时间|" + startTime);
            try
            {
                //找出所有队友组合
                foreach (SortedSet<Team> teams in playerPool.Values)
                {
                    foreach(Team team in teams)
                    {
                        if (team.IsBelong == false)
                        {
                            TeamGroup group = new TeamGroup();
                            group.Add(team);
                            if (DFSMatch(group))
                            {
                                groups.Add(group);
                            }
                            else
                            {
                                group.Remove(team);
                            }
                        }
                        else continue;
                    }
                }
                //将所有组进行对抗配对
                //参考 https://blog.csdn.net/best789248/article/details/78429538 求中值配对
            }
            catch
            {

            }
        }
        private bool DFSMatch(TeamGroup group)
        {
            if (group.Count == needUsers) return true;
            //从多到少选择人数队伍
            for (int i = needUsers - group.Count; i > 0; i--)
            {
                //得到这个人数的所有队伍
                playerPool.TryGetValue(i, out SortedSet<Team> value);
                //只需要得到第一个值即可
                foreach (Team team in value)
                {
                    if(team.IsBelong == false && (Math.Abs(group.AverageRank - team.AverageRank) <= 2))
                    {
                        //将这个队伍放入
                        group.Add(team);
                        //继续深度搜索
                        if (DFSMatch(group))
                        {
                            //搜索满足，跳出即可.
                            return true;
                        }
                        else
                        {
                            //加入这个队伍的情况下，无法达成条件，重置痕迹.
                            group.Remove(team);
                        }
                        break;
                    }
                }
            }
            return false;
        }
        #endregion

        #region --内部类--
        class TeamGroup : IComparable<TeamGroup>
        {
            private HashSet<Team> teams = new HashSet<Team>();
            private int count;
            private double rank;
            private double averageRank;
            public HashSet<Team> Teams { get => teams; set => teams = value; }
            public int Count { get => count; set => count = value; }
            public double AverageRank { get => averageRank; set => averageRank = value; }

            public void Add(Team team)
            {
                if (teams.Add(team)) 
                {
                    count += team.Users.Count;
                    rank += team.AverageRank;
                    AverageRank /= count;
                    team.IsBelong = true;
                }
            }

            public void Remove(Team team)
            {
                if (teams.Remove(team))
                {
                    count -= team.Users.Count;
                    rank -= team.AverageRank;
                    AverageRank /= count;
                    team.IsBelong = false;
                }
            }
            public int CompareTo([AllowNull] TeamGroup other)
            {
                if (averageRank > other.averageRank) return -1;
                else return 0;
            }
        }
        #endregion
    }
}
