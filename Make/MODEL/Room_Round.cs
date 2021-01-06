using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Make.MODEL
{
    public abstract class Room_Round:Room
    {
        #region --方法--
        public Room_Round(int max,int min)
        {
            Max_Personals = max;
            Min_Personals = min;
            GeneralControl.Rooms.Add(this);
            GeneralControl.Menu_Data_Monitor_Class.Instance.Rooms = $"房间总数:{GeneralControl.Rooms.Count}";
        }
        public override void SendMessages(string msg, object bound = null)
        {
            Players.ForEach((Player player) => player.Send(msg, bound));
        }
        public override void Action_Stage(Player player, string[] messages)
        {
            lock (this)
            {
                if (messages.Length >= 2)
                {
                    SkillCard skillCard = player.Hand_SkillCards[messages[0]];
                    if (skillCard.Need_Mp <= player.Mp)
                    {
                        if (player.Action == false)
                        {
                            if (player.State_Is_Exist("缴械", skillCard.Need_Mp) != null && skillCard.Is_Physics)
                            {
                                player.Send("您已被缴械，无法释放物理技能卡");
                                return;
                            }
                            if (player.State_Is_Exist("冻结", skillCard.Need_Mp) != null)
                            {
                                player.Send("您被冻结,此技能卡无法突破冻结上限！");
                                return;
                            }
                            if (player.State_Is_Exist("沉默", skillCard.Need_Mp) != null && skillCard.Is_Magic)
                            {
                                player.Send("您已被沉默，无法释放魔法技能卡");
                                return;
                            }
                            player.Mp -= skillCard.Need_Mp;
                            Random random = new Random();
                            if (random.Next(1, 100) <= skillCard.Probability)
                            {
                                //skillCard = BLL.SkillCard_Helper.Get_SkillCardsModel_ID(skillCard.Father_ID).SkillCards[skillCard.Level + 1];
                                player.Send("您释放的技能在命运的帮助下提升为更高品质-" + skillCard.Name);
                            }
                            player.Action_Skill = skillCard.Clone(player);
                            player.Action = true;
                            if (messages.Length == 1)
                            {
                                /*
                                if (player.Action_Skill.Is_Benefit)
                                {
                                    if (player.Action_Skill.Friends.Count < player.Action_Skill.Auxiliary_Number) player.Action_Skill.Friends.Add(player);
                                    foreach (Player friend in player.Friends)
                                    {
                                        if (player.Action_Skill.Friends.Count < player.Action_Skill.Auxiliary_Number)
                                        {
                                            player.Action_Skill.Friends.Add(friend);
                                        }
                                        else break;
                                    }
                                }
                                else
                                {
                                    foreach (Player direct in player.Enemies)
                                    {
                                        if (player.Action_Skill.Enemies.Count < player.Action_Skill.Attack_Number)
                                        {
                                            player.Action_Skill.Enemies.Add(direct);
                                        }
                                        else break;
                                    }
                                }
                                */
                            }
                            else
                            {
                                foreach (string item in messages)
                                {
                                    if (int.TryParse(item, out int direct) && direct >= 0 && direct < Players.Count && !(player.Action_Skill.Enemies.Contains(Players[direct]) && player.Action_Skill.Friends.Contains(Players[direct])))
                                    {
                                        if (player.Friends.Contains(Players[direct]))
                                        {
                                            if (player.Action_Skill.Friends.Count >= player.Action_Skill.Auxiliary_Number) continue;
                                            else player.Action_Skill.Friends.Add(Players[direct]);
                                        }
                                        else
                                        {
                                            if (player.Action_Skill.Enemies.Count >= player.Action_Skill.Attack_Number) continue;
                                            else player.Action_Skill.Enemies.Add(Players[direct]);
                                        }
                                    }
                                }
                            }
                            //先赋予状态
                            foreach (State state in player.Action_Skill.Buff)
                            {
                                /*
                                state.Expire_Round = Round + state.Duration_Round - 1;
                                if (state.Is_Benefit)
                                {
                                    foreach (Player attacked in player.Action_Skill.Friends)
                                    {
                                        //判断对方是否有霸体
                                        if (attacked.State_Is_Exist("霸体", player.Action_Skill.Need_Mp) != null)
                                        {
                                            return;
                                        }
                                        state.Owner = player;
                                        state.Direct = attacked;
                                        attacked.Add_States(state);
                                    }
                                }
                                else
                                {
                                    foreach (Player attacked in player.Action_Skill.Enemies)
                                    {
                                        //判断对方是否有霸体
                                        if (attacked.State_Is_Exist("霸体", player.Action_Skill.Need_Mp) != null)
                                        {
                                            return;
                                        }
                                        state.Owner = player;
                                        state.Direct = attacked;
                                        attacked.Add_States(state);
                                    }
                                }
                                */
                            }
                            if (Players.Find((Player item) => item != player) == null)
                            {
                                Result_Stage();
                            }
                            if (!((from Player item in Players where item.Action == false select item).Any())) Result_Stage();
                        }
                        else player.Send("您已出招");
                    }
                    else player.Send("您的仙气不足");
                }
            }
        }
        public override void Force_Close()
        {
            while (true)
            {
                if (this != null && Stage == Enums.Room.Action)
                {
                    foreach (Player player in Players)
                    {
                        player.Send("房间已被强制中止");
                    }
                    Clean();
                    break;
                }
                else Thread.Sleep(5000);
            }

        }
        /// <summary>
        /// 得到该房间随机一个敌人的Player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        public Player Get_Direct(Player player, Room_Round room)
        {
            IEnumerable<Player> players = from Player item in room.Players where item.Team != player.Team && item.Is_Death == false select item;
            Random random = new Random();
            return players.ElementAt(random.Next(0, players.Count() - 1));
        }
        /// <summary>
        /// 战后打扫
        /// </summary>
        public void Clean()
        {
            foreach(Player player in Players)
            {
                GeneralControl.Players.Remove(player.ID);
            }
            GeneralControl.Rooms.Remove(this);
            GeneralControl.Menu_Data_Monitor_Class.Instance.Players = $"当前在线:{GeneralControl.Players.Count}人";
            GeneralControl.Menu_Data_Monitor_Class.Instance.Map_Players = $"当前地图在线:{GeneralControl.Map.Players.Count}人";
            GeneralControl.Menu_Data_Monitor_Class.Instance.Room_Players = $"当前房间在线:{GeneralControl.Players.Count - GeneralControl.Map.Players.Count}人";
        }
        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="player"></param>
        public override void Enter(User user)
        {
            if (Players.Count < Max_Personals)
            {
                //生成一个玩家实体
                Player player = new Player(user);
                player.DeathEvent += player.OnDeath_Room;
                player.Active = Enums.Player_Active.Round;
                Players.Add(player);
                user.Active = Enums.User_Active.Ready;
                GeneralControl.Players.Add(player.ID, player);
                GeneralControl.Menu_Data_Monitor_Class.Instance.Players = $"当前在线:{GeneralControl.Players.Count}人";
                GeneralControl.Menu_Data_Monitor_Class.Instance.Map_Players = $"当前地图在线:{GeneralControl.Map.Players.Count}人";
                GeneralControl.Menu_Data_Monitor_Class.Instance.Room_Players = $"当前房间在线:{GeneralControl.Players.Count - GeneralControl.Map.Players.Count}人";
            }
            else user.SendMessages("人数已满,加入房间失败");
        }
        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="player"></param>
        public override void Enter(Player player)
        {
            if (Players.Count < Max_Personals)
            {
                player.Active = Enums.Player_Active.Round;
                Players.Add(player);
                GeneralControl.Players.Add(player.ID, player);
                GeneralControl.Menu_Data_Monitor_Class.Instance.Players = $"当前在线:{GeneralControl.Players.Count}人";
                GeneralControl.Menu_Data_Monitor_Class.Instance.Map_Players = $"当前地图在线:{GeneralControl.Map.Players.Count}人";
                GeneralControl.Menu_Data_Monitor_Class.Instance.Room_Players = $"当前房间在线:{GeneralControl.Players.Count - GeneralControl.Map.Players.Count}人";
            }
        }
        /// <summary>
        /// 离开房间
        /// </summary>
        /// <param name="player"></param>
        public override void Leave(Player player)
        {
            Players.Remove(player);
            GeneralControl.Players.Remove(player.ID);
            if (Players.Count == 0)
            {
                GeneralControl.Rooms.Remove(this);
                GeneralControl.Menu_Data_Monitor_Class.Instance.Rooms = $"房间总数:{GeneralControl.Rooms.Count}";
            }
            GeneralControl.Menu_Data_Monitor_Class.Instance.Players = $"当前在线:{GeneralControl.Players.Count}人";
            GeneralControl.Menu_Data_Monitor_Class.Instance.Map_Players = $"当前地图在线:{GeneralControl.Map.Players.Count}人";
            GeneralControl.Menu_Data_Monitor_Class.Instance.Room_Players = $"当前房间在线:{GeneralControl.Players.Count - GeneralControl.Map.Players.Count}人";
            player.Init();
        }
        #endregion
    }
}
