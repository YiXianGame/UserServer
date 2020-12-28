using Make.MODEL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Make.BLL
{
    //NPC的一系列行为操作
    public static class NPC_Helper
    {
        /// <summary>
        /// 回合状态下的AI技能分析
        /// </summary>
        /// <param name="room"></param>
        public static void AI_Skill_Analysis_Room(Room_Round room)
        {
            Random random = new Random();
            //循环所有机器人
            foreach (Player player in (from Player item in room.Players where item.Is_Robot && !item.Is_Death && item.State_Is_Exist("冻结")!=null select item))
            {
                List<SkillCard> array = (from SkillCard item in player.Hand_SkillCards where item.Need_Mp <= player.Mp select item).ToList();
                List<Player> attack = new List<Player>();
                //寻找出针对这个机器人的所有玩家
                foreach(Player room_player in (from Player item in room.Players where item!=player && item.Action_Skill!=null && item.Action_Skill.Enemies.Contains(player) && item.Team!=player.Team select item))
                {
                    State temp = room_player.State_Is_Exist("反弹");
                    if (temp!=null)
                    {
                        array = (from SkillCard item in array  where (item.Is_Self == true||item.Need_Mp > temp.Effect_mp) select item).ToList();
                    }
                    temp = room_player.State_Is_Exist("反制");
                    if (temp != null)
                    {
                        array = (from SkillCard item in array where (item.Is_Magic == false || item.Need_Mp > temp.Effect_mp) select item).ToList() ;
                    }
                    temp = room_player.State_Is_Exist("格挡");
                    if (temp != null)
                    {
                        array = (from SkillCard item in array where (item.Is_Physics == false || item.Need_Mp > temp.Effect_mp) select item).ToList();
                    }
                    if (room_player.Action_Skill!=null && room_player.Action_Skill.Is_Physics)
                    {
                        array = (from SkillCard item in array where (item.State_Is_Exist("格挡")>=room_player.Action_Skill.Need_Mp) select item).ToList();
                    }
                    if (room_player.Action_Skill != null && room_player.Action_Skill.Is_Magic)
                    {
                        array = (from SkillCard item in array where (item.State_Is_Exist("反制") >= room_player.Action_Skill.Need_Mp) select item).ToList();
                    }
                    temp = player.State_Is_Exist("缴械");
                    if (temp != null) array = (from SkillCard item in array where (item.Need_Mp > temp.Effect_mp || !item.Is_Physics) select item).ToList();
                    temp = player.State_Is_Exist("沉默");
                    if (temp != null) array = (from SkillCard item in array where (item.Need_Mp > temp.Effect_mp || !item.Is_Magic) select item).ToList();
                    attack.Add(room_player);
                }
                if (array.Count > 0)
                {
                    SkillCard temp = (SkillCard)array[random.Next(0, array.Count - 1)];
                    player.Mp -= temp.Need_Mp;
                    player.Action_Skill=temp.Clone(player);
                    if (player.Action_Skill.Is_Self)
                    {
                        foreach (Player direct in player.Friends)
                        {
                            if (player.Action_Skill.Friends.Count >= player.Action_Skill.Auxiliary_Number) break;
                            player.Action_Skill.Friends.Add(player);
                        }
                    }
                    else
                    {
                        foreach (Player direct in attack)
                        {
                            if (player.Action_Skill.Enemies.Count >= player.Action_Skill.Attack_Number) break;
                            player.Action_Skill.Enemies.Add(direct);
                        }
                        foreach (Player direct in player.Enemies)
                        {
                            if (player.Action_Skill.Enemies.Count >= player.Action_Skill.Attack_Number) break;
                            player.Action_Skill.Enemies.Add(direct);
                        }
                    }
                    //先赋予状态
                    foreach (State state in player.Action_Skill.Effect_States)
                    {
                        state.Expire_Round = player.Room.Round + state.Duration_Round - 1;
                        if (state.Is_Self)
                        {
                            foreach (Player friend in player.Action_Skill.Friends)
                            {
                                state.Owner = player;
                                state.Direct = friend;
                                friend.Add_States(state);
                            }
                        }
                        else
                        {
                            foreach (Player attacked in player.Action_Skill.Enemies)
                            {
                                state.Owner = player;
                                state.Direct = attacked;
                                attacked.Add_States(state);
                            }
                        }
                    }
                    player.Action_Skill.Release(player);
                }
            }
        }
        /// <summary>
        /// 即时状态下的AI技能分析
        /// </summary>
        /// <param name="player">NPC</param>
        /// <param name="enemies">敌人</param>
        public static void AI_Skill_Analysis_Immediate(Player player,List<Player> enemies)
        {
            Random random = new Random();
            List<SkillCard> array = (from SkillCard item in player.Hand_SkillCards.Values where item.Need_Mp <= player.Mp select item).ToList();
            List<Player> attack = new List<Player>();
            //寻找出针对这个机器人的所有玩家
            foreach (Player enemy in enemies)
            {
                State temp = enemy.State_Is_Exist("反弹");
                if (temp != null)
                {
                    array = (from SkillCard item in array where (item.Is_Self == true || item.Need_Mp > temp.Effect_mp) select item).ToList();
                }
                temp = enemy.State_Is_Exist("反制");
                if (temp != null)
                {
                    array = (from SkillCard item in array where (item.Is_Magic == false || item.Need_Mp > temp.Effect_mp) select item).ToList();
                }
                temp = enemy.State_Is_Exist("格挡");
                if (temp != null)
                {
                    array = (from SkillCard item in array where (item.Is_Physics == false || item.Need_Mp > temp.Effect_mp) select item).ToList();
                }
                if (enemy.Action_Skill!=null && enemy.Action_Skill.Is_Physics)
                {
                    array = (from SkillCard item in array where (item.State_Is_Exist("格挡") >= enemy.Action_Skill.Need_Mp) select item).ToList();
                }
                if (enemy.Action_Skill != null && enemy.Action_Skill.Is_Magic)
                {
                    array = (from SkillCard item in array where (item.State_Is_Exist("反制") >= enemy.Action_Skill.Need_Mp) select item).ToList();
                }
                temp = player.State_Is_Exist("缴械");
                if (temp != null) array = (from SkillCard item in array where (item.Need_Mp > temp.Effect_mp || !item.Is_Physics) select item).ToList();
                temp = player.State_Is_Exist("沉默");
                if (temp != null) array = (from SkillCard item in array where (item.Need_Mp > temp.Effect_mp || !item.Is_Magic) select item).ToList();
                attack.Add(enemy);
            }
            if (array.Count > 0)
            {
                SkillCard temp = (SkillCard)array[random.Next(0, array.Count - 1)];
                player.Mp -= temp.Need_Mp;
                player.Action_Skill = temp.Clone(player);
                if (player.Action_Skill.Friends.Count < player.Action_Skill.Auxiliary_Number)
                {
                    player.Action_Skill.Friends.Add(player);
                }
                foreach (Player direct in player.Friends)
                {
                    if (player.Action_Skill.Friends.Count >= player.Action_Skill.Auxiliary_Number) break;
                    player.Action_Skill.Friends.Add(player);
                }
                foreach (Player direct in attack)
                {
                    if (player.Action_Skill.Enemies.Count >= player.Action_Skill.Attack_Number) break;
                    player.Action_Skill.Enemies.Add(direct);
                }
                foreach (Player direct in player.Enemies)
                {
                    if (player.Action_Skill.Enemies.Count >= player.Action_Skill.Attack_Number) break;
                    player.Action_Skill.Enemies.Add(direct);
                }
                player.Action_Skill.Release(player);
            }
        }
        /// <summary>
        /// 生成一个新的机器人
        /// </summary>
        /// <returns>返回一个Player类</returns>
        public static Player New_NPC()
        {
            RandomChinese randomChinese = new RandomChinese();
            Player player = new Player();
            player.NickName = randomChinese.GetRandomChinese(3);
            player.UserName = player.GetHashCode().ToString();
            player.Is_Robot = true;
            player.Init();
            if (GeneralControl.Skill_Card_ID_Skllcard.Count > 0)
            {
                int cnt = 30; //机器人手牌数量
                Random random = new Random();
                SkillCard[] skillCards = GeneralControl.Skill_Card_ID_Skllcard.Values.ToArray();
                if (skillCards.Length > 0)
                {
                    for (int i = 0; i < cnt; i++)
                    {
                        SkillCard skillCard = skillCards[random.Next(0, skillCards.Length - 1)].Clone();
                        skillCard.Amount = 1;
                        player.Hand_Skill_Add(skillCard);
                    }
                }
            }
            return player;
        }
        /// <summary>
        /// 生成一个Monster
        /// </summary>
        /// <returns>返回一个Monster类</returns>
        public static Monster New_Monster()
        {
            RandomChinese randomChinese = new RandomChinese();
            Monster player = new Monster();
            player.Init();
            player.NickName = randomChinese.GetRandomChinese(3);
            player.UserName = player.GetHashCode().ToString();
            player.Is_Robot = true;
            if (GeneralControl.Skill_Card_ID_Skllcard.Count > 0)
            {
                int cnt = 30; //机器人手牌数量
                Random random = new Random();
                SkillCard[] skillCards = GeneralControl.Skill_Card_ID_Skllcard.Values.ToArray();
                if (skillCards.Length > 0)
                {
                    for (int i = 0; i < cnt; i++)
                    {
                        SkillCard skillCard = skillCards[random.Next(0, skillCards.Length - 1)].Clone();
                        skillCard.Amount = 1;
                        player.Hand_Skill_Add(skillCard);
                    }
                }
            }
            return player;
        }
    }
}
