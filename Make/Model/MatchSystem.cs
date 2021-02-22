using Material.Entity;
using Material.Entity.Game;
using Material.Interface;
using Material.RPCServer.TCP_Async_Event;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Make.Model
{
    public class MatchSystem<T> where T:IMatchSystemItem
    {
        //将所有组进行对抗配对
        //参考 https://blog.csdn.net/best789248/article/details/78429538
        //优化内容：
        //1.泛型处理,达成了嵌套的目的，在游戏本例中，首先进行5人组的小队组团配置，之后在进行团对抗配对，这两个过程可以用嵌套匹配达到需求。
        //2.原博客N方嵌套整理顺序数据，计算时间等，现在采用链表式，时间顺序加锁Add本身就保证了时序。
        //3.全部都是动态维护，不存在计算时重新整理生成，节约了空间与时间，且是稳定排序遍历，所有的操作都是O(1)级操作
        //4.保留Rank总和，避免精度丢失问题，且嵌套的每一层都将进行多次战力数据对比。
        //5.采用深度搜索算法,匹配数量支持下限与上限.
        
        #region --委托--
        public delegate void MatchSucessDelegate(List<TeamGroup<T>> teamGroups);
        public delegate void MatchFailDelegate(T team);
        #endregion

        #region --事件--
        public event MatchSucessDelegate MatchSucessEvent;
        public event MatchFailDelegate MatchFailEvent;
        #endregion

        #region --字段--
        private Timer timer;
        private SortedDictionary<int, Dictionary<int, LinkedList<T>>> teamsPool = new SortedDictionary<int, Dictionary<int, LinkedList<T>>>();
        private List<TeamGroup<T>> groupsPool;
        private int min = 0;
        private int max = 0;
        #endregion

        #region --属性--
        public MatchSystem(int min,int max)
        {
            this.min = min;
            this.max = max;
        }
        #endregion

        #region --方法--
        public void Start()
        {
            //timer = new Timer(new TimerCallback(MatchProcess), null, 0, 5000);
            MatchProcess(null);
        }
        public bool Add(T team)
        {
            lock (teamsPool)
            {
                if (!teamsPool.TryGetValue(team.Count, out Dictionary<int, LinkedList<T>> rankTeams))
                {
                    rankTeams = new Dictionary<int, LinkedList<T>>();
                    if (!teamsPool.TryAdd(team.Count, rankTeams))return false;
                }
                if (!rankTeams.TryGetValue(team.AverageRank, out LinkedList<T> timeTeams))
                {
                    timeTeams = new LinkedList<T>();
                    if (!rankTeams.TryAdd(team.AverageRank,timeTeams))return false;
                }
                timeTeams.AddLast(team);
                return true;
            }
        }
        public bool Add(ICollection<T> teams)
        {
            lock (teamsPool)
            {
                foreach(T team in teams)
                {
                    if (!teamsPool.TryGetValue(team.Count, out Dictionary<int, LinkedList<T>> rankTeams))
                    {
                        rankTeams = new Dictionary<int, LinkedList<T>>();
                        if (!teamsPool.TryAdd(team.Count, rankTeams)) return false;
                    }
                    if (!rankTeams.TryGetValue(team.AverageRank, out LinkedList<T> timeTeams))
                    {
                        timeTeams = new LinkedList<T>();
                        if (!rankTeams.TryAdd(team.AverageRank, timeTeams)) return false;
                    }
                    timeTeams.AddLast(team);
                }
                return true;
            }
        }
        public void MatchProcess(object sender)
        { 
            Console.WriteLine($"{typeof(T)}获取匹配锁中");
            lock (teamsPool)
            {
                Console.WriteLine($"{typeof(T)}开始匹配");
                groupsPool = new List<TeamGroup<T>>();
                long startTime = Material.Utils.TimeStamp.Now();
                long now = Material.Utils.TimeStamp.Now();
                int teams_sum = 0, item_sum = 0;
                foreach (Dictionary<int, LinkedList<T>> linkTeams in teamsPool.Values)
                {
                    foreach (LinkedList<T> timeTeams in linkTeams.Values)
                    {
                        foreach (T team in timeTeams)
                        {
                            teams_sum++;
                            item_sum += team.Count;
                        }
                    }
                }
                Console.WriteLine($"{typeof(T)}深度组团开始,本次有{teams_sum}支队伍，{item_sum}个待匹配项");

                foreach (int key in teamsPool.Keys)
                {
                    if (teamsPool.TryGetValue(key, out Dictionary<int, LinkedList<T>> rankTeams))
                    {
                        foreach (int second_key in rankTeams.Keys)
                        {
                            if (rankTeams.TryGetValue(second_key, out LinkedList<T> timeTeams))
                            {
                                Console.WriteLine($"{typeof(T)}-{key}队 Rank:{second_key} 数量:{timeTeams.Count}");
                            }
                        }
                    }
                }
                int cnt = 0;
                foreach (Dictionary<int, LinkedList<T>> rankTeams in teamsPool.Values)
                {
                    foreach (LinkedList<T> teams in rankTeams.Values)
                    {
                        int a = 2;
                        while (teams.First != null)
                        {
                            T team = teams.First.Value;
                            teams.RemoveFirst();
                            //判断是否等待时间超过1小时,有一种极端的情况，已经超时，但是恰好被选中了，即被标记了true，则
                            if ((now - team.StartMatchTime) >= 60 * 60 * 1000)
                            {
                                if (MatchFailEvent != null) MatchFailEvent(team);
                                teams.RemoveFirst();
                            }
                            else
                            {
                                TeamGroup<T> group = new TeamGroup<T>();
                                group.Add(team);
                                if (DFSMatch(group))
                                {
                                    cnt++;
                                    if (groupsPool == null)
                                    {
                                        //中断匹配
                                        foreach (T item in group.Teams)
                                        {
                                            if (MatchFailEvent != null) MatchFailEvent(item);
                                        }
                                    }
                                    else groupsPool.Add(group);
                                }
                                else
                                {
                                    teams.AddFirst(team);
                                    group.Remove(team);
                                    break;
                                }
                            }
                        }
                    }
                }

                Console.WriteLine($"{typeof(T)}开始清理队伍池数据:" + teamsPool.Count + "队");
                foreach (int key in teamsPool.Keys)
                {
                    if (teamsPool.TryGetValue(key, out Dictionary<int, LinkedList<T>> rankTeams))
                    {
                        foreach (int second_key in rankTeams.Keys)
                        {
                            if (rankTeams.TryGetValue(second_key, out LinkedList<T> timeTeams))
                            {
                                Console.WriteLine($"{typeof(T)}-{key}队 Rank:{second_key} 数量:{timeTeams.Count}");
                            }
                        }
                    }
                }
                List<int> clearTeamspool = new List<int>();
                foreach (int key in teamsPool.Keys)
                {
                    if (teamsPool.TryGetValue(key, out Dictionary<int, LinkedList<T>> rankTeams))
                    {
                        foreach (int second_key in rankTeams.Keys)
                        {
                            if (rankTeams.TryGetValue(second_key, out LinkedList<T> timeTeams))
                            {
                                if (timeTeams.Count == 0)
                                {
                                    rankTeams.Remove(second_key);
                                }
                            }
                        }
                        if (rankTeams.Count == 0)
                        {
                            clearTeamspool.Add(key);
                        }
                    }
                }
                foreach (int key in clearTeamspool)
                {
                    teamsPool.Remove(key);
                }
                Console.WriteLine($"{typeof(T)}清理队伍池数据完毕:" + teamsPool.Count + "队");
                foreach (int key in teamsPool.Keys)
                {
                    if (teamsPool.TryGetValue(key, out Dictionary<int, LinkedList<T>> rankTeams))
                    {
                        foreach (int second_key in rankTeams.Keys)
                        {
                            if (rankTeams.TryGetValue(second_key, out LinkedList<T> timeTeams))
                            {
                                Console.WriteLine($"{typeof(T)}-{key}队 Rank:{second_key} 数量:{timeTeams.Count}");
                            }
                        }
                    }
                }
                teams_sum = 0;
                item_sum = 0;
                foreach (Dictionary<int, LinkedList<T>> rankTeams in teamsPool.Values)
                {
                    foreach (LinkedList<T> timeTeams in rankTeams.Values)
                    {
                        foreach (T team in timeTeams)
                        {
                            teams_sum++;
                            item_sum += team.Count;
                        }
                    }
                }
                Console.WriteLine($"{typeof(T)}深度组团结束,成功组成{groupsPool.Count}支团,{teams_sum}支队伍，{item_sum}个待匹配项");
                long end = Material.Utils.TimeStamp.Now();
                Console.WriteLine($"{typeof(T)}-总耗时:{end - startTime}毫秒");
                if(MatchSucessEvent!=null)MatchSucessEvent(groupsPool);
                groupsPool = null;
            }
        }
        private bool DFSMatch(TeamGroup<T> group)
        {
            if (group.Count == max) return true;
            //参考思路:找出同一分数段里，等待时间最长的玩家，用他来匹配，因为他的区间最大,如果他都不能匹配到，等待时间比他短的玩家更匹配不到
            //Console.WriteLine(oldest.AverageWaitTime + "|为该分数上等待最久时间的团开始匹配|rank|" + oldest.AverageRank);  
            //按等待时间扩大匹配范围
            double c2 = 1.5;
            int c3 = 5;
            int c4 = 100;
            double u = 0;//= Math.Pow(group.GetStartTime-now, c2);
            u = u + c3;
            u = Math.Round(u);
            u = Math.Min(u, c4);
            int min_result = (int)(group.AverageRank - (u));
            int rank_min = 0;//min_result < 0 ? 0 : min_result;
            int rank_max = 1000;//(int)(group.AverageRank + u);
            //Console.WriteLine(oldest.AverageWaitTime + "|本次搜索rank范围下限|" + min + "|rank范围上限|" + max);
            int middle = group.AverageRank;

            //从多到少选择人数队伍
            for (int i = max - group.Count; i > 0; i--)
            {
                //得到这个人数的所有Rank队伍
                if (teamsPool.TryGetValue(i, out Dictionary<int, LinkedList<T>> rankTeams))
                {
                    for (int searchRankUp = middle, searchRankDown = middle; searchRankUp <= rank_max || searchRankDown >= rank_min; searchRankUp++, searchRankDown--)
                    {
                        LinkedList<T> teams;
                        if ((rankTeams.TryGetValue(searchRankUp, out teams)) || (rankTeams.TryGetValue(searchRankDown, out teams)))
                        {
                            if(teams.First != null)
                            {
                                T team = teams.First.Value;
                                teams.RemoveFirst();
                                group.Add(team);
                                //可以做一些if判断
                                if (DFSMatch(group))
                                {
                                    return true;
                                }
                                else
                                {
                                    if (group.Count >= min) return true;
                                    else
                                    {
                                        teams.AddFirst(team);
                                        group.Remove(team);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        #endregion
        //迭代器暂时废弃
        class TeamsIterator : IEnumerator<Team>
        {
            private LinkedListNode<Team> current;
            private LinkedList<Team> teams = null;
            public TeamsIterator(SortedSet<Team> teams)
            {
                this.teams = new LinkedList<Team>();
                foreach (Team item in teams) this.teams.AddLast(item);
                this.teams.AddFirst(new LinkedListNode<Team>(null));
                current = this.teams.First;
            }
            
            public Team Current => current.Value;

            object IEnumerator.Current => current.Value;

            public void Dispose()
            {
                current = null;
                teams.Clear();
            }

            public bool MoveNext()
            {
                if (current == null || current.Next == null) return false;
                else
                {
                    current = current.Next;
                    return true;
                }  
            }
            public bool MoveBack()
            {
                if (current == null || current.Previous == null) return false;
                else
                {
                    current = current.Previous;
                    return true;
                }
            }
            public void Reset()
            {
                current = this.teams.First;
            }
        }
    }
}
