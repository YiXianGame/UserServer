using Material.Entity;
using Material.Entity.Game;
using Material.RPCServer.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Make.Model
{
    public class MatchSystem<T> where T : Room, new()
    {
        #region --字段--
        private Timer timer;
        private SortedDictionary<int, List<Team>> teamsPool;
        private Dictionary<int, SortedSet<TeamGroup>> teamgroupsPool = new Dictionary<int, SortedSet<TeamGroup>>();
        private int needPlayers = 0;
        #endregion

        #region --属性--

        #endregion

        #region --方法--
        public MatchSystem()
        {
            timer = new Timer(new TimerCallback(MatchProcess), null, 5000, 500000);
            needPlayers = new T().Max_players;
            teamsPool = new SortedDictionary<int, List<Team>>(new TeamsPoolCompare());
        }
        public bool Enter(Team team)
        {
            lock (teamsPool)
            {
                if (teamsPool.TryGetValue(team.Users.Count, out List<Team> value))
                {
                    return value.Add(team);
                }
                else
                {
                    SortedSet<Team> teams = new SortedSet<Team>();
                    if (this.teamsPool.TryAdd(team.Users.Count, teams))
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
            Console.WriteLine("开始匹配");
            lock (teamsPool)
            {
                lock (teamgroupsPool)
                {
                    long now = Material.Utils.TimeStamp.Now();
                    //待清理的队伍
                    List<Team> clearTeams = new List<Team>();
                    //清理队伍池数据
                    List<int> clearTeamsPool = new List<int>();
                    int teams_sum = 0,player_sum = 0;
                    foreach (SortedSet<Team> teams in teamsPool.Values)
                    {
                        teams_sum += teams.Count;
                        foreach(Team team in teams)
                        {
                            player_sum += team.Users.Count;
                        }
                    }
                    Console.WriteLine($"深度组团开始,本次有{teams_sum}支队伍，{player_sum}位玩家");
                    foreach (SortedSet<TeamGroup> item in teamgroupsPool.Values)
                    {
                        foreach (TeamGroup group in item)
                        {
                            group.RefreshAverageWaitTime(now);
                        }
                    }
                    try
                    {
                        //找出所有队友组合
                        foreach (List<Team> teams in teamsPool.Values)
                        {
                            for (int i = teams.Count;i>0;i++)
                            {
                                if (!teams[i].IsCheck)
                                {
                                    //判断是否等待时间超过1小时,有一种极端的情况，已经超时，但是恰好被选中了，即被标记了true，则
                                    if ((now - teams[i].StartMatchTime) >= 60 * 60 * 1000)
                                    {
                                        foreach (BaseUserToken user in teams[i].Users)
                                        {
                                            Core.UserRequest.CancelMatch(user);
                                        }
                                        teams[i].IsCheck = true;
                                        teams.Remove(team);
                                        ///clearTeams.Add(team);
                                    }
                                    else
                                    {
                                        TeamGroup group = new TeamGroup();
                                        group.Add(team);
                                        if (DFSMatch(group))
                                        {
                                            group.RefreshAverageWaitTime(now);//新生成的组，更新一下时间
                                            if (!teamgroupsPool.TryGetValue(group.AverageRank, out SortedSet<TeamGroup> teamGroups))
                                            {
                                                teamGroups = new SortedSet<TeamGroup>();
                                                if (!teamgroupsPool.TryAdd(group.AverageRank, teamGroups))
                                                {
                                                    teamGroups = null;
                                                }
                                            }
                                            if (teamGroups == null || !teamGroups.Add(group))
                                            {
                                                //中断匹配
                                                foreach (Team item in group.Teams)
                                                {
                                                    item.Users.ForEach((value) => Core.UserRequest.CancelMatch(value));
                                                    item.Users.Clear();
                                                }
                                            }
                                            teams.Remove(team);
                                            ///clearTeams.Add(team);
                                        }
                                        else
                                        {
                                            group.Remove(team);
                                        }
                                    }
                                }
                                else
                                {
                                    teams.Remove(team);
                                    ///clearTeams.Add(team);
                                    continue;
                                }
                            }
                            //清理队伍
                            if (clearTeams.Count > 0)
                            {
                                int key = clearTeams[0].Users.Count;
                                Console.WriteLine($"开始清理{key}队伍数据:" + teams.Count);
                                //删掉队伍数据
                                foreach (Team item in clearTeams) teams.Remove(item);
                                if (teams.Count == 0) clearTeamsPool.Add(key);
                                clearTeams.Clear();
                                Console.WriteLine($"清理{key}队伍数据完毕:" + teams.Count);
                            }
                        }
                        //清理队伍池
                        if (clearTeamsPool.Count > 0)
                        {
                            Console.WriteLine($"开始清理队伍池数据:" + teamsPool.Count);
                            clearTeamsPool.ForEach(value => teamsPool.Remove(value));
                            clearTeamsPool.Clear();
                            Console.WriteLine($"清理队伍池数据完毕:" + teamsPool.Count);
                        }
                        int teamsgroup_sum = 0;
                        foreach (SortedSet<TeamGroup> groups in teamgroupsPool.Values)
                        {
                            teamsgroup_sum += groups.Count;
                        }
                        teams_sum = 0;
                        player_sum = 0;
                        foreach (SortedSet<Team> teams in teamsPool.Values)
                        {
                            teams_sum += teams.Count;
                            foreach (Team team in teams)
                            {
                                player_sum += team.Users.Count;
                            }
                        }
                        Console.WriteLine($"深度组团完成,本次成功组成{teamsgroup_sum}个团，还有{teams_sum}支队伍，{player_sum}位玩家");
                        teamsPool.TryGetValue(1, out SortedSet<Team> ss);
                        //将所有组进行对抗配对
                        //参考 https://blog.csdn.net/best789248/article/details/78429538
                        GroupMatch();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            long end = Material.Utils.TimeStamp.Now();
            Console.WriteLine("匹配完成，总耗时:" + (end-startTime) + "毫秒");
        }
        private bool DFSMatch(TeamGroup group)
        {
            if (group.Count == needPlayers) return true;
            //从多到少选择人数队伍
            for (int i = needPlayers - group.Count; i > 0; i--)
            {
                //得到这个人数的所有队伍
                if (teamsPool.TryGetValue(i, out SortedSet<Team> value))
                {
                    foreach (Team team in value)
                    {
                        if (team.IsCheck == false && (Math.Abs(group.AverageRank - team.AverageRank) <= 1000))
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
                                //加入这个队伍人数的情况下，无法达成条件，重置痕迹.
                                group.Remove(team);
                            }
                            break;
                        }
                    }
                }
                else return false;
            }
            return false;
        }

        private void GroupMatch()
        {
            //待清理的团
            List<TeamGroup> clearTeamGroups = new List<TeamGroup>();
            //待清理的团池
            List<int> clearTeamGroupsPool = new List<int>();
            int cnt = 0;
            int sum = 0;
            foreach (SortedSet<TeamGroup> groups in teamgroupsPool.Values)
            {
                sum += groups.Count;
            }
            Console.WriteLine($"中值寻团开始，本次战局有{sum}个团");
            foreach (SortedSet<TeamGroup> groups in teamgroupsPool.Values)
            {
                foreach (TeamGroup oldest in groups)
                {
                    if (!oldest.IsCheck)
                    {
                        //清理超时的组合
                        if (oldest.AverageWaitTime >= 60 * 60 * 1000)
                        {
                            oldest.IsCheck = true;
                            //中断匹配
                            foreach (Team item in oldest.Teams)
                            {
                                item.Users.ForEach((value) => Core.UserRequest.CancelMatch(value));
                                item.Users.Clear();
                            }
                            oldest.Teams.Clear();
                            clearTeamGroups.Add(oldest);
                            continue;
                        }
                        //参考思路:找出同一分数段里，等待时间最长的玩家，用他来匹配，因为他的区间最大,如果他都不能匹配到，等待时间比他短的玩家更匹配不到
                        //Console.WriteLine(oldest.AverageWaitTime + "|为该分数上等待最久时间的团开始匹配|rank|" + oldest.AverageRank);  
                        //按等待时间扩大匹配范围
                        double c2 = 1.5;
                        int c3 = 5;
                        int c4 = 100;

                        double u = Math.Pow(oldest.AverageWaitTime, c2);
                        u = u + c3;
                        u = Math.Round(u);
                        u = Math.Min(u, c4);
                        int min_result = (int)(oldest.AverageRank - (u));
                        int min = min_result < 0 ? 0 : min_result;
                        int max = (int)(oldest.AverageRank + u);

                        //Console.WriteLine(oldest.AverageWaitTime + "|本次搜索rank范围下限|" + min + "|rank范围上限|" + max);
                        int middle = oldest.AverageRank;

                        for (int searchRankUp = middle, searchRankDown = middle; searchRankUp <= max || searchRankDown >= min; searchRankUp++, searchRankDown--)
                        {
                            SortedSet<TeamGroup> value;
                            TeamGroup result = null;
                            if (teamgroupsPool.TryGetValue(searchRankUp, out value) && value.Count > 0)
                            {
                                //所有组都很合适，可以直接选择第一个，也可以遍历找一找更合适的条件约束,例如后期可以加性别、地区等因子
                                foreach (TeamGroup teamGroup in value)
                                {
                                    if (!teamGroup.IsCheck && teamGroup != oldest)
                                    {
                                        result = teamGroup;
                                        break;
                                    }
                                }
                            }
                            else if (teamgroupsPool.TryGetValue(searchRankDown, out value) && value.Count > 0)
                            {
                                foreach (TeamGroup teamGroup in value)
                                {
                                    if (!teamGroup.IsCheck && teamGroup != oldest)
                                    {
                                        result = teamGroup;
                                        break;
                                    }
                                }
                            }
                            if (result != null)
                            {
                                result.IsCheck = true;
                                oldest.IsCheck = true;
                                clearTeamGroups.Add(oldest);
                                cnt++;
                                break;
                            }
                        }
                    }
                    else clearTeamGroups.Add(oldest);
                }
                //清理团数据
                if (clearTeamGroups.Count > 0)
                {
                    int key = clearTeamGroups[0].AverageRank;
                    Console.WriteLine($"开始清理{key}团数据:" + groups.Count);
                    foreach (TeamGroup item in clearTeamGroups)
                    {
                        groups.Remove(item);
                    }
                    if (groups.Count == 0) clearTeamGroupsPool.Add(key);
                    clearTeamGroups.Clear();
                    Console.WriteLine($"清理完毕{key}团数据:" + groups.Count);
                }
            }
            sum = 0;
            foreach (SortedSet<TeamGroup> groups in teamgroupsPool.Values)
            {
                sum += groups.Count;
            }
            Console.WriteLine($"本次成功配对{cnt}场战局，还有{sum}个团留存");
            //清理团池
            if (clearTeamGroupsPool.Count > 0)
            {
                Console.WriteLine($"开始清理团池数据:" + teamgroupsPool.Count);
                clearTeamGroupsPool.ForEach(value => teamgroupsPool.Remove(value));
                clearTeamGroupsPool.Clear();
                Console.WriteLine($"清理完毕团池数据:" + teamgroupsPool.Count);
            }

        }
        #endregion
        class TeamsPoolCompare : IComparer<int>
        {
            public int Compare([AllowNull] int x, [AllowNull] int y)
            {
                if (x == y) return 0;
                else if (x > y) return -1;
                else return 1;
            }
        }
    }
}
