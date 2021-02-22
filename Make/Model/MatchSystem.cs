using Material.Entity;
using Material.Entity.Game;
using Material.ExceptionModel;
using Material.Interface;
using Material.RPCServer.TCP_Async_Event;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Make.Model
{
    public class MatchSystem<T,R> where T:IMatchSystemTeam<R>,new() where R:IMatchSystemItem
    {
        /*  将所有组进行对抗配对
         *  参考 https://blog.csdn.net/best789248/article/details/78429538
         *  优化内容：
         *  1.泛型处理,达成了嵌套的目的，在游戏本例中，首先进行5人组的小队组团配置，之后在进行团对抗配对，这两个过程可以用嵌套匹配达到需求。
         *  2.原博客N方嵌套整理顺序数据，计算时间等，现在采用链表式，时间顺序加锁Add本身就保证了时序。
         *  3.全部都是动态维护，不存在计算时重新整理生成，节约了空间与时间，且是稳定排序遍历，所有的操作都是O(1)级操作
         *  4.保留Rank总和，避免精度丢失问题，且嵌套的每一层都将进行多次战力数据对比。
         *  5.采用深度搜索算法,支持自定义匹配人数，匹配数量支持下限与上限.
         *  6.基于嵌套写法和事件系统可以进行Pipline通道流
         * 
         *  实例化: 
         *  MatchSystem<Team,Squad> soloMatchSystem = new MatchSystem<Team, Squad>(6,6);
         *  MatchSystem<TeamGroup,Team> soloGroupMatchSystem = new MatchSystem<TeamGroup, Team>(12,12);
         *  泛型第一个为父亲容器，第二个为容器子项
         * 
         *  通道流：A.MatchSystem.MatchPiplineEvent += B.GroupMatchSystem.PiplineEnter;
         *  此通道流将从A流向B，B无需增加时间轮询.
         *  
         *  2021.2.22 代码性能实测 1-4人 5人队10人组,Rank全匹配: 100W 3000ms 
         *            满足测试服的匹配需求绰绰有余.
         */

        #region --委托--
        public delegate bool MatchPiplineDelegate(List<T> teamGroups);
        public delegate void MatchSucessDelegate(List<T> teamGroups);
        public delegate void MatchFailDelegate(T team);
        #endregion

        #region --事件--
        public event MatchPiplineDelegate MatchPiplineEvent;
        public event MatchSucessDelegate MatchSucessEvent;
        public event MatchFailDelegate MatchFailEvent;
        #endregion

        #region --字段--
        private Timer timer;
        private SortedDictionary<int, Dictionary<int, LinkedList<R>>> teamsPool = new SortedDictionary<int, Dictionary<int, LinkedList<R>>>();
        private List<T> sucessPool;
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
        public bool Enter(R item)
        {
            lock (teamsPool)
            {
                if (!teamsPool.TryGetValue(item.Count, out Dictionary<int, LinkedList<R>> rankTeams))
                {
                    rankTeams = new Dictionary<int, LinkedList<R>>();
                    if (!teamsPool.TryAdd(item.Count, rankTeams))return false;
                }
                if (!rankTeams.TryGetValue(item.AverageRank, out LinkedList<R> timeTeams))
                {
                    timeTeams = new LinkedList<R>();
                    if (!rankTeams.TryAdd(item.AverageRank,timeTeams))return false;
                }
                timeTeams.AddLast(item);
                return true;
            }
        }
        public bool Enter(List<R> teams)
        {
            lock (teamsPool)
            {
                foreach(R item in teams)
                {
                    if (!teamsPool.TryGetValue(item.Count, out Dictionary<int, LinkedList<R>> rankTeams))
                    {
                        rankTeams = new Dictionary<int, LinkedList<R>>();
                        if (!teamsPool.TryAdd(item.Count, rankTeams)) return false;
                    }
                    if (!rankTeams.TryGetValue(item.AverageRank, out LinkedList<R> timeTeams))
                    {
                        timeTeams = new LinkedList<R>();
                        if (!rankTeams.TryAdd(item.AverageRank, timeTeams)) return false;
                    }
                    timeTeams.AddLast(item);
                }
                return true;
            }
        }
        public bool PiplineEnter(List<R> teams)
        {
            if (Enter(teams))
            {
                MatchProcess(null);
                return true;
            }
            else return false;
        }
        public void MatchProcess(object sender)
        { 
            Console.WriteLine($"{typeof(T)}获取匹配锁中");
            lock (teamsPool)
            {
                Console.WriteLine($"{typeof(T)}开始匹配");
                sucessPool = new List<T>();
                long startTime = Material.Utils.TimeStamp.Now();
                long now = Material.Utils.TimeStamp.Now();
                int teams_sum = 0, item_sum = 0;
                foreach (Dictionary<int, LinkedList<R>> linkTeams in teamsPool.Values)
                {
                    foreach (LinkedList<R> timeTeams in linkTeams.Values)
                    {
                        foreach (R item in timeTeams)
                        {
                            teams_sum++;
                            item_sum += item.Count;
                        }
                    }
                }
                Console.WriteLine($"{typeof(T)}深度组团开始,本次有{teams_sum}支队伍，{item_sum}个待匹配项");

                foreach (int key in teamsPool.Keys)
                {
                    if (teamsPool.TryGetValue(key, out Dictionary<int, LinkedList<R>> rankTeams))
                    {
                        foreach (int second_key in rankTeams.Keys)
                        {
                            if (rankTeams.TryGetValue(second_key, out LinkedList<R> timeTeams))
                            {
                                Console.WriteLine($"{typeof(T)}-{key}队 Rank:{second_key} 数量:{timeTeams.Count}");
                            }
                        }
                    }
                }
                foreach (Dictionary<int, LinkedList<R>> rankTeams in teamsPool.Values)
                {
                    foreach (LinkedList<R> teams in rankTeams.Values)
                    {
                        while (teams.First != null)
                        {
                            R item = teams.First.Value;
                            teams.RemoveFirst();
                            T container = new T();
                            container.Add(item);
                            //判断是否等待时间超过1小时,有一种极端的情况，已经超时，但是恰好被选中了，即被标记了true
                            if ((now - item.StartMatchTime) >= 60 * 60 * 1000)
                            {
                                container.Remove(item);
                                if (MatchFailEvent != null) MatchFailEvent(container);
                            }
                            else
                            {
                                if (DFSMatch(container))
                                {
                                    if (sucessPool != null) sucessPool.Add(container);
                                    else
                                    {
                                        MatchFailEvent(container);
                                        throw new MatchSystemException(MatchSystemException.ErrorCode.NotFoundSucessPool, "没有找到配对成功后存放的配对池");
                                    }
                                }
                                else
                                {
                                    teams.AddFirst(item);
                                    container.Remove(item);
                                    break;
                                }
                            }
                        }
                    }
                }

                Console.WriteLine($"{typeof(T)}开始清理队伍池数据:" + teamsPool.Count + "队");
                foreach (int key in teamsPool.Keys)
                {
                    if (teamsPool.TryGetValue(key, out Dictionary<int, LinkedList<R>> rankTeams))
                    {
                        foreach (int second_key in rankTeams.Keys)
                        {
                            if (rankTeams.TryGetValue(second_key, out LinkedList<R> timeTeams))
                            {
                                Console.WriteLine($"{typeof(T)}-{key}队 Rank:{second_key} 数量:{timeTeams.Count}");
                            }
                        }
                    }
                }
                List<int> clearTeamspool = new List<int>();
                foreach (int key in teamsPool.Keys)
                {
                    if (teamsPool.TryGetValue(key, out Dictionary<int, LinkedList<R>> rankTeams))
                    {
                        foreach (int second_key in rankTeams.Keys)
                        {
                            if (rankTeams.TryGetValue(second_key, out LinkedList<R> timeTeams))
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
                    if (teamsPool.TryGetValue(key, out Dictionary<int, LinkedList<R>> rankTeams))
                    {
                        foreach (int second_key in rankTeams.Keys)
                        {
                            if (rankTeams.TryGetValue(second_key, out LinkedList<R> timeTeams))
                            {
                                Console.WriteLine($"{typeof(T)}-{key}队 Rank:{second_key} 数量:{timeTeams.Count}");
                            }
                        }
                    }
                }
                teams_sum = 0;
                item_sum = 0;
                foreach (Dictionary<int, LinkedList<R>> rankTeams in teamsPool.Values)
                {
                    foreach (LinkedList<R> timeTeams in rankTeams.Values)
                    {
                        foreach (R team in timeTeams)
                        {
                            teams_sum++;
                            item_sum += team.Count;
                        }
                    }
                }
                Console.WriteLine($"{typeof(T)}深度组团结束,成功组成{sucessPool.Count}支团,{teams_sum}支队伍，{item_sum}个待匹配项");
                long end = Material.Utils.TimeStamp.Now();
                Console.WriteLine($"{typeof(T)}-总耗时:{end - startTime}毫秒");
                if(MatchSucessEvent!=null)MatchSucessEvent(sucessPool);
                if (MatchPiplineEvent != null) MatchPiplineEvent(sucessPool);
                sucessPool = null;
            }
        }
        private bool DFSMatch(T container)
        {
            if(container.Count == 7)
            {
                return true;
            }
            if (container.Count == max) return true;
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
            int min_result = (int)(container.AverageRank - (u));
            int rank_min = 0;//min_result < 0 ? 0 : min_result;
            int rank_max = 1000;//(int)(group.AverageRank + u);
            //Console.WriteLine(oldest.AverageWaitTime + "|本次搜索rank范围下限|" + min + "|rank范围上限|" + max);
            int middle = container.AverageRank;

            //从多到少选择人数队伍
            for (int i = max - container.Count; i > 0; i--)
            {
                //得到这个人数的所有Rank队伍
                if (teamsPool.TryGetValue(i, out Dictionary<int, LinkedList<R>> rankTeams))
                {
                    for (int searchRankUp = middle, searchRankDown = middle; searchRankUp <= rank_max || searchRankDown >= rank_min; searchRankUp++, searchRankDown--)
                    {
                        LinkedList<R> teams;
                        if ((rankTeams.TryGetValue(searchRankUp, out teams)) || (rankTeams.TryGetValue(searchRankDown, out teams)))
                        {
                            if(teams.First != null)
                            {
                                R other = teams.First.Value;
                                teams.RemoveFirst();
                                container.Add(other);
                                //可以做一些if判断
                                if (DFSMatch(container))
                                {
                                    return true;
                                }
                                else
                                {
                                    if (container.Count >= min) return true;
                                    else
                                    {
                                        teams.AddFirst(other);
                                        container.Remove(other);
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
    }
}
