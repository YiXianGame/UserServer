using System;
using System.Collections.Generic;

namespace Make.MODEL
{
    public class Pos
    {
        #region --字段--
        private string name = "🌾";
        private int x;
        private int y;
        private object item;
        private string informations;
        #endregion

        #region --属性--
        public string Informations { get => informations; set => informations = value; }
        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
        public string Name { get => name; set => name = value; }
        public object Item { get => item; set => item = value; }
        #endregion

        #region --方法--
        public int Distance(Pos pos)
        {
            return Math.Abs(pos.X - X) + Math.Abs(pos.Y - Y);
        }
        public void Effect(Player player)
        {
            if (item != null)
            {
                if (item is Adventure)
                {
                    Adventure adventure = item as Adventure;
                    player.Map.Current_Resources_Adventure--;
                    string messages="";
                    foreach(Player enemy in player.Enemies)
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
                        if (enemy != player && enemy.Active == Enums.Player_Active.Map)
                        {
                            if (enemy.Is_Death && player.Active == Enums.Player_Active.Map)
                            {
                                enemy.OnDeath(player,new EventArgsModel.DeathEventArgs(player,enemy));
                            }
                        }
                        player.Send(messages);
                    }
                    messages = "";
                    foreach (Player friend in player.Friends)
                    {
                        if (friend.Active != Enums.Player_Active.Map)
                        {
                            continue;
                        }
                        friend.Leisure = DateTime.Now.AddMinutes(5);
                        if (adventure.Cure != 0)
                        {
                            friend.Add_Hp(adventure.Cure);
                        }
                        if (adventure.Self_Mp != 0) friend.Add_Mp(adventure.Self_Mp);
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
                            if (friend.Is_Death && player.Active == Enums.Player_Active.Map)
                            {
                                friend.OnDeath(player,new EventArgsModel.DeathEventArgs(player,friend));
                            }
                        }
                        player.Send(messages);
                    }
                }
                else if (item is List<SkillCard>)
                {
                    List<SkillCard> SkillCards = item as List<SkillCard>;
                    player.Map.Current_Resources_SkillCard--;
                    foreach (SkillCard item in SkillCards)
                    {
                        player.Hand_Skill_Add(item);
                    }
                }
                else if (item is List<State>)
                {
                    List<State> States = item as List<State>;
                    player.Map.Current_Resources_State--;
                    foreach (State state in States)
                    {
                        state.Owner = null;
                        state.Direct = player;
                        player.Add_States(state);
                    }
                }
            }
        }

        public bool Add(object add_item)
        {
            if (Item == null)
            {
                if (add_item is Player)
                {
                    Player player = add_item as Player;
                    if (player.Is_Robot) Name = "🗡";
                    else Name = "⚔️";
                }
                else if (add_item is Adventure)
                {
                    Adventure adventure = add_item as Adventure;
                    Name = "✨";
                    System.Diagnostics.Debug.WriteLine(name);
                }
                else if (add_item is List<SkillCard>)
                {
                    List<SkillCard> SkillCards = add_item as List<SkillCard>;
                    Name = "📇";
                    System.Diagnostics.Debug.WriteLine(name);
                }
                else if (add_item is List<State>)
                {
                    List<State> States = add_item as List<State>;
                    Name = "💊";
                    System.Diagnostics.Debug.WriteLine(name);
                }
                Item = add_item;
                return true;
            }
            else return false;
        }
        public void Init()
        {
            item = null;
            Name = "🌾";
        }
        #endregion
    }
}
