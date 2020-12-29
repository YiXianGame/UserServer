using Make.BLL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Make.MODEL
{
    public class XianYu_Map : Map
    {
        #region --方法--
        public XianYu_Map(int H,int W) : base(H, W)
        {
            Height = H;
            Width = W;
            Pos_Map = new Pos[Height, Width];
            Height--;
            Width--;
            for (int i = 0; i <= Height; i++)
            {
                for (int j = 0; j <= Width; j++)
                {
                    Pos_Map[i, j] = new Pos();
                    Pos_Map[i, j].X = i;
                    Pos_Map[i, j].Y = j;
                }
            }
        }


        /// <summary>
        /// 行动
        /// </summary>
        /// <param name="player"></param>
        /// <param name="messages"></param>
        public override void Action_Stage(Player player, string[] messages)
        {
            SkillCard skillCard = player.Hand_SkillCards[messages[0]];
            if (skillCard != null)
            {
                if (skillCard.Need_Mp <= player.Mp)
                {
                    if (player.State_Is_Exist("缴械", skillCard.Need_Mp) != null && skillCard.Is_Physics)
                    {
                        player.Send("缴械#出招失败#");
                        return;
                    }
                    if (player.State_Is_Exist("冻结", skillCard.Need_Mp) != null)
                    {
                        player.Send("冻结#出招失败#");
                        return;
                    }
                    if (player.State_Is_Exist("沉默", skillCard.Need_Mp) != null && skillCard.Is_Magic)
                    {
                        player.Send("沉默#出招失败#");
                        return;
                    }
                    player.Mp -= skillCard.Need_Mp;
                    Random random = new Random();
                    while (skillCard.Level < GeneralControl.MaxLevel)
                    {
                        if (random.Next(1, 100) <= skillCard.Probability)
                        {
                            skillCard = SkillCard_Helper.Get_SkillCardsModel_ID(skillCard.Father_ID).SkillCards[skillCard.Level + 1];
                            player.Send("您释放的技能在命运的帮助下提升为更高品质-" + skillCard.Name);
                        }
                        else break;
                    }
                    player.Action_Skill = skillCard.Clone(player);
                    player.Send("【" + player.Title + "】" + player.NickName + "释放了技能卡");
                    if(messages.Length == 1)
                    {
                        if (player.Action_Skill.Is_Self)
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
                    }
                    else
                    {
                        if (messages[1] == "命")
                        {
                            foreach (string item in messages)
                            {
                                int direct;
                                if (int.TryParse(item, out direct) && player.Action_Skill.Enemies.Count < player.Action_Skill.Attack_Number && direct >= 0 && direct < player.Enemies.Count)
                                {
                                    player.Action_Skill.Enemies.Add(player.Enemies.ElementAt(direct));
                                }
                            }
                        }
                        else
                        {
                            foreach (string item in messages)
                            {
                                if (player.Action_Skill.Friends.Count < player.Action_Skill.Auxiliary_Number) player.Action_Skill.Friends.Add(player);
                                int direct;
                                if (int.TryParse(item, out direct) && player.Action_Skill.Friends.Count < player.Action_Skill.Auxiliary_Number && direct >= 0 && direct < player.Friends.Count)
                                {
                                    player.Action_Skill.Friends.Add(player.Friends.ElementAt(direct));
                                }
                            }
                        }
                    }
                    player.Action_Skill.Release(player);
                }
                else player.Send("您的仙气不足");
            }
            else player.Send("您未具有此技能！");
        }
        public override void Resource_Init()
        {
            Random random = new Random();
            Pos[] pos_Array = (from Pos item in Pos_Map where item.Item == null select item).ToArray();
            //奇遇 
            if (GeneralControl.Adventures.Count > 0)
            {
                for (int i = 0; i < GeneralControl.Menu_GameControl_Class.Instance.Map_Resources_Adventure; i++)
                {
                    pos_Array[random.Next(0, pos_Array.Length - 1)].Add(GeneralControl.Adventures[random.Next(0, GeneralControl.Adventures.Count - 1)]);
                    Current_Resources_Adventure++;
                }
            }

            //技能卡
            if (GeneralControl.Adventures.Count > 0)
            {
                pos_Array = (from Pos item in Pos_Map where item.Item == null select item).ToArray();
                for (int i = 0; i < GeneralControl.Menu_GameControl_Class.Instance.Map_Resources_SkillCard; i++)
                {
                    List<SkillCard> skillCards = new List<SkillCard>();
                    skillCards.AddRange(SkillCard_Helper.Get_Random(1, random.Next(1, 3)));
                    if(skillCards.FirstOrDefault()!=null && skillCards.FirstOrDefault().Level>=3 && random.Next(1, 100) <= 30)
                    {
                        continue;
                    }
                    Console.WriteLine(skillCards.FirstOrDefault().Name + ":" + skillCards.FirstOrDefault().Level);
                    pos_Array[random.Next(0, pos_Array.Length - 1)].Add(skillCards);
                    Current_Resources_SkillCard++;
                }
            }

            //状态
            if (GeneralControl.States.Count > 0)
            {
                pos_Array = (from Pos item in Pos_Map where item.Item == null select item).ToArray();
                for (int i = 0; i < GeneralControl.Menu_GameControl_Class.Instance.Map_Resources_State; i++)
                {
                    List<State> states = GeneralControl.States.Values.ToList();
                    pos_Array[random.Next(0, pos_Array.Length - 1)].Add(states[random.Next(0, GeneralControl.States.Count - 1)]);
                    Current_Resources_State++;
                }
            }
            //野怪;
            pos_Array = (from Pos item in Pos_Map where item.Item == null select item).ToArray();
            for (int i = 0; i < GeneralControl.Menu_GameControl_Class.Instance.Map_Resources_Monster; i++)
            {
                Monster monster = NPC_Helper.New_Monster();
                monster.Map = GeneralControl.Map;
                monster.Active = Enums.Player_Active.Map;
                monster.Move(pos_Array[random.Next(0, pos_Array.Count() - 1)]);
                monster.Hp = GeneralControl.Menu_GameControl_Class.Instance.Map_Hp_Max;
                monster.Mp = 0;//**GeneralControl.Menu_GameControl_Class.Instance.Map_Hp_Max / 3;
                Current_Resources_Monster++;
            }

        }
        #endregion
    }
}
