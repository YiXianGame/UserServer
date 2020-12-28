using Make.BLL;
using Material;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Make.MODEL
{
    public class Battle_Royale_Room : Room_Round
    {
        public Battle_Royale_Room(int max, int min,Socket socket ,long fromGroup = 0) : base(max,min,socket,fromGroup)
        { 
            Max_Personals = max;
            Min_Personals = min;
        }

        /// <summary>
        /// 房间开始初始化
        /// </summary>
        public override void Start()
        {
            SendMessages("游戏开始啦！");
            int flag = 0;
            foreach (Player player in Players)
            {
                player.Action = false;
                player.Hp = GeneralControl.Menu_GameControl_Class.Instance.Room_Hp_Max;
                player.Mp = 0;
                player.Action_Skill = null;
                player.Action = false;
                player.Team = flag++;
                player.Kills = 0;
                player.Death_Round = 0;
                player.Team = flag;
                player.States.Clear();
            }
        }
        /// <summary>
        /// 蓄气阶段
        /// </summary>
        public override void Raise_Stage()
        {
            //将所有机器人置为释放技能
            foreach (Player player in Players)
            {
                if (player.Is_Robot) player.Action = true;
            }
            Latest_Date = DateTime.Now;
            Stage = Enums.Room.Action;
            string universal_Informations;
            Random random = new Random();
            ++Round;
            SendMessages("当前回合:" + Round.ToString());

            //奇遇
            foreach (Player player in Players)
            {
                if (random.Next(1, 100) <= GeneralControl.Menu_GameControl_Class.Instance.Adventure_Probability) continue;
                int random_personal = random.Next(1, 100);
                foreach (Adventure adventure in GeneralControl.Adventures)
                {
                    if (random_personal >= adventure.Probability)
                    {
                        foreach (Player enemy in player.Enemies)
                        {
                            if (adventure.Attack != 0)
                            {
                                enemy.Add_Hp(-adventure.Attack);
                            }
                            if (adventure.Direct_Mp != 0) enemy.Add_Mp(-adventure.Direct_Mp);
                            if (enemy.Active == Enums.Player_Active.Map)
                            {
                                //先赋予状态
                                foreach (State state in player.Action_Skill.Effect_States)
                                {
                                    if (!state.Is_Self)
                                    {
                                        state.Expire_Immediate = DateTime.Now.AddSeconds(state.Duration_Immediate);
                                        state.Owner = player;
                                        state.Direct = enemy;
                                        enemy.Add_States(state);
                                    }
                                }
                            }
                            enemy.OnDeath(this,new EventArgsModel.DeathEventArgs(null,player));
                        }

                        foreach (Player friend in player.Friends)
                        {
                            if (friend.Active != Enums.Player_Active.Map)
                            {
                                player.SendMessages($"{friend}已离开仙域,该目标无法进行技能释放");
                                continue;
                            }
                            friend.Leisure = DateTime.Now.AddMinutes(5);
                            if (adventure.Cure != 0)
                            {
                                // messages += friend.Add_Hp(adventure.Cure, "被" + player.Name + "所经历的奇遇【" + adventure.Name + "】");
                                friend.Add_Hp(adventure.Cure);
                            }
                            if (adventure.Self_Mp != 0)
                            {
                                //messages += friend.Add_Mp(adventure.Self_Mp, "被" + player.Name + "所经历的奇遇【" + adventure.Name + "】" + "使");
                                friend.Add_Mp(adventure.Self_Mp);
                            }
                            if (friend.Active == Enums.Player_Active.Map)
                            {
                                //先赋予状态
                                foreach (State state in player.Action_Skill.Effect_States)
                                {
                                    if (state.Is_Self)
                                    {
                                        state.Expire_Immediate = DateTime.Now.AddSeconds(state.Duration_Immediate);
                                        state.Owner = player;
                                        state.Direct = friend;
                                        friend.Add_States(state);
                                    }
                                }
                            }
                            if (friend != player && friend.Active == Enums.Player_Active.Map)
                            {
                                friend.SendMessages($"{player.Name}所经历的奇遇【{adventure.Name}】\n{messages}");
                                if (friend.Is_Death && player.Active == Enums.Player_Active.Map)
                                {
                                    friend.OnDeath(player);
                                }
                            }
                            SendMessages(messages);
                        }
                        if (!GeneralControl.Menu_GameControl_Class.Instance.Adventure_Concurrent) break;
                    }
                }
                SendMessages(universal_Informations);
            }

            //获得仙气
            universal_Informations = "";
            if (Round <= 4)
            {
                foreach (Player player in Players)
                {
                    universal_Informations += player.Add_Mp(random.Next(1, 5), "通过吸收仙气,");
                }
            }
            else if (Round <= 8)
            {
                foreach (Player player in Players)
                {
                    universal_Informations += player.Add_Mp(random.Next(5, 10), "通过吸收仙气,");
                }
            }
            else if (Round >= 8)
            {
                foreach (Player player in Players)
                {
                    universal_Informations += player.Add_Mp(random.Next(10, 15), "通过吸收仙气,");
                }
            }
            SendMessages(universal_Informations);

            //个人面板
            universal_Informations = "【天道盟】\n";
            int pos = 0;
            foreach (Player player in Players)
            {
                string statetemp = "";
                foreach (State item in player.States)
                {
                    statetemp += item.Name + "(" + item.Expire_Round.ToString() + "-" + item.Effect_mp + ")";
                }
                universal_Informations += "【" + player.User.Title + "】" + player.Name + "\n"
                                            + "ID:" + (pos++).ToString() + "\n"
                                            + "血量:" + player.Hp.ToString() + "\n"
                                            + "仙气:" + player.Mp.ToString() + "\n"
                                            + "状态:" + statetemp + "\n"
                                            + BQ.d + BQ.d + BQ.d + BQ.d + BQ.d + BQ.d + BQ.d + "\n";
            }
            SendMessages(universal_Informations);

            //检测冻结状态
            foreach (Player player in Players)
            {
                if (player.State_Is_Exist("冻结", player.Mp) != null)
                {
                    player.Private_SendMessages("您已被冻结");
                    player.Action_Skill = null;
                    player.Action = true;
                }
            }

            //检测是否死亡
            foreach (Player player in Players)
            {
                if (player.Hp <= 0)
                {
                    player.Action = true;
                    player.Private_SendMessages("您已死亡");
                }
            }
            //发送手牌
            foreach (Player player in (from Player item in Players where !item.Is_Robot select item)) player.Private_SendMessages_JSON(JSON_Helper.Hand_Skill_Show(player));

            //检测是否全部出招
            if ((from Player item in Players where item.Action == false select item).Any() == false) Result_Stage();
        }
        /// <summary>
        /// 结算阶段
        /// </summary>
        public override void Result_Stage()
        {

            //该房间AI分析
            NPC_Helper.AI_Skill_Analysis_Room(this);

            Stage = Enums.Room.Result;

            string universal_Informations = "";

            //输出技能显示
            universal_Informations = "";
            foreach (Player player in Players)
            {
                if (player.Action_Skill != null) universal_Informations += player.Name + "发动了:" + player.Action_Skill.Name + "\n";
                else universal_Informations += player.Name + "本回合不出招" + "\n";
            }
            SendMessages(universal_Informations);


            universal_Informations = "";
            foreach (Player player in Players)
            {
                foreach (State state in player.States)
                {
                    if (state.Name == "恒血")
                    {
                        universal_Informations += player.Add_Hp(Convert.ToInt32(state.Effect_mp), "因为恒血BUFF");
                    }
                    else if (state.Name == "恒气")
                    {
                        universal_Informations += player.Add_Mp(Convert.ToInt32(state.Effect_mp), "因为恒气BUFF");
                    }
                }
            }
            SendMessages(universal_Informations);

            //释放技能
            foreach (Player player in Players)
            {
                if (player.Action_Skill != null) player.Action_Skill.Release(player);
            }
            SendMessages(universal_Informations);

            universal_Informations = "";
            //判断是否胜利
            if (Players.Where((Player item) => !item.Is_Death).Count()<=1)
            {
                Player win_playr = Players.Where((Player item) => !item.Is_Death).FirstOrDefault();
                if (win_playr != null)
                {
                    SendMessages($"本局杀戮King为{win_playr.Name}!!!!!");
                    win_playr.Settle(Round * 10, Round * 10, "本场次平局！再来一场吧！\n");
                }
                else SendMessages($"本场势均力敌！");

                foreach(Player player in Players)
                {
                    if (player != win_playr)
                    {
                        player.Settle(Round, Round, "本场次平局！再来一场吧！\n");
                    }
                }
                Clean();
            }
            
            //判断是不是只剩下机器人
            if (!((from Player item in Players where !item.Is_Robot select item).Any()))
            {
                Clean();
                return;
            }

            foreach (Player player in Players)
            {
                if (player.State_Is_Exist("冻结")!=null)
                {
                    player.Private_SendMessages("您已被冻结");
                    player.Action_Skill = null;
                    player.Action = true;
                }
                else
                {
                    player.Action_Skill = null;
                    player.Action = true;
                }
            }

            //状态回合刷新
            foreach (Player player in Players) player.Refresh_State_Round();

            //重置技能
            foreach (Player player in Players)
            {
                player.Action = false;
            }
            Raise_Stage();
        }

    }
}
