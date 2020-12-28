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
    public class Team_Room : Room_Round
    {
        public Team_Room(int max, int min) : base(max,min)
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
            int flag=0;
            foreach (Player player in Players)
            {
                player.Action = false;
                player.Hp = GeneralControl.Menu_GameControl_Class.Instance.Room_Hp_Max;
                player.Mp = 0;
                player.Action_Skill = null;
                player.Action = false;
                player.Kills = 0;
                player.Death_Round = 0;
                player.Team = flag;
                player.States.Clear();
            }
            for (int i = 0; i < Players.Count / 2; i++)
            {
                Players[i].Team = 0;
            }
            for (int i = Players.Count / 2; i < Players.Count; i++)
            {
                Players[i].Team = 1;
            }
            Raise_Stage();
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
                            if (adventure.Direct_Mp != 0)
                            {
                                enemy.Add_Mp(-adventure.Direct_Mp);
                            }
                            foreach (State state in adventure.Effect_States)
                            {
                                enemy.Add_States(state.Clone());
                            }
                        }
                        foreach (Player friend in player.Friends)
                        {
                            friend.Leisure = DateTime.Now.AddMinutes(5);
                            if (adventure.Cure != 0)
                            {
                                friend.Add_Hp(adventure.Cure);
                            }
                            if (adventure.Self_Mp != 0)
                            {
                                friend.Add_Mp(adventure.Self_Mp);
                            }
                            foreach (State state in adventure.Effect_States)
                            {
                                friend.Add_States(state.Clone());
                            }
                        }
                        if (!GeneralControl.Menu_GameControl_Class.Instance.Adventure_Concurrent) break;
                    }
                }
            }

            //获得仙气
            universal_Informations = "";
            if (Round <= 4)
            {
                foreach (Player player in Players)
                {
                    player.Add_Mp(random.Next(1, 5));
                }
            }
            else if (Round <= 8)
            {
                foreach (Player player in Players)
                {
                    player.Add_Mp(random.Next(5, 10));
                }
            }
            else if (Round >= 8)
            {
                foreach (Player player in Players)
                {
                    player.Add_Mp(random.Next(10, 15));
                }
            }
            SendMessages(universal_Informations);

            //检测冻结状态
            foreach (Player player in Players)
            {
                if (player.State_Is_Exist("冻结", player.Mp) != null)
                {
                    player.Send("冻结#无法出招");
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
                    player.Send("死亡#无法出招");
                }
            }
            //发送手牌
            foreach (Player player in Players) player.Send("展示手牌", player);

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
                if (player.Action_Skill != null) universal_Informations += player.NickName + "发动了:" + player.Action_Skill.Name + "\n";
                else universal_Informations += player.NickName + "本回合不出招" + "\n";
            }
            SendMessages(universal_Informations);


            universal_Informations = "";
            foreach (Player player in Players)
            {
                foreach (State state in player.States)
                {
                    if (state.Name == "恒血")
                    {
                        player.Add_Hp(Convert.ToInt32(state.Effect_mp));
                    }
                    else if (state.Name == "恒气")
                    {
                        player.Add_Mp(Convert.ToInt32(state.Effect_mp));
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
            bool blue = (from Player item in Players where item.Team == 0 && !item.Is_Death select item).Any(), red = (from Player item in Players where item.Team == 1 && !item.Is_Death select item).Any();
            if (!blue || !red)
            {
                if(!blue && !red)
                {
                    foreach(Player player in Players)
                    {
                        User user = User.Load(player.UserName);
                        user.Settle(Round, Round * 2);
                        user.Save();
                    }
                }
                else if(blue)
                {
                    foreach(Player player in (from Player item in Players where item.Team == 0 select item))
                    {
                        User user = User.Load(player.UserName);
                        user.Settle(Round, Round * 2);
                        user.Save();
                    }
                    SendMessages(universal_Informations);
                    universal_Informations = "";
                    foreach (Player player in (from Player item in Players where item.Team == 1 select item))
                    {
                        User user = User.Load(player.UserName);
                        user.Settle(1, 1);
                        user.Save();
                    }
                    SendMessages(universal_Informations);
                }
                else if (red)
                {
                    foreach (Player player in (from Player item in Players where item.Team == 0 select item))
                    {
                        User user = User.Load(player.UserName);
                        user.Settle(1,1);
                        user.Save();
                    }
                    SendMessages(universal_Informations);
                    universal_Informations = "";
                    foreach (Player player in (from Player item in Players where item.Team == 1 select item))
                    {
                        User user = User.Load(player.UserName);
                        user.Settle(Round, Round * 2);
                        user.Save();
                    }
                }
                Clean();
                return;
            }
            //判断是不是只剩下机器人
            if(!((from Player item in Players where !item.Is_Robot select item).Any()))
            {
                Clean();
                return;
            }

            //检测冻结状态
            foreach (Player player in Players)
            {
                if (player.State_Is_Exist("冻结", player.Mp) != null)
                {
                    player.Send("冻结#无法出招");
                    player.Action_Skill = null;
                    player.Action = true;
                }
            }

            //状态回合刷新
            foreach (Player player in Players) player.Refresh_State();

            //重置技能
            foreach (Player player in Players)
            {
                player.Action = false;
            }

            Raise_Stage();
        }

    }
}
