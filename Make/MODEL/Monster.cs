using Make.BLL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Make.MODEL
{
    public class Monster:Player
    {
        #region --属性--
        Task AI_Analysis_Task;
        #endregion

        #region --方法--
        public void Attack(Player player)
        {
            Add_Enemy(player);
            player.Add_Enemy(this);
            Debug.WriteLine("怪兽锁定玩家:"+player.NickName);
            if (AI_Analysis_Task == null)
            {
                AI_Analysis_Task = Task.Run(AI_Analysis);
                Debug.WriteLine("怪兽线程启动");
            }
        }
        public new int Move(Pos pos)
        {
            lock (pos)
            {
                int dis_X = 0, dis_Y = 0;
                if (Pos != null)
                {
                    dis_X = Math.Abs(pos.X - Pos.X);
                    dis_Y = Math.Abs(pos.Y - Pos.Y);
                }
                else Pos = pos;
                int wait = (dis_X + dis_Y) - Mp;
                if (wait <= 0)
                {
                    if (Pos.Item != null) Pos.Init();
                    Pos = pos;
                    Pos.Effect(this);
                    Pos.Init();
                    pos.Add(this);
                    foreach (Player player in Enemiesed)
                    {
                        player.Send($"魂命:{NickName}已移动至[{Pos.X},{Pos.Y}]\n与您相距[{Pos.X - player.Pos.X},{player.Pos.Y - Pos.Y}]");
                    }
                    foreach (Player player in Friendsed)
                    {
                        player.Send($"灵命:{NickName}已移动至[{Pos.X},{Pos.Y}]\n与您相距[{Pos.X - player.Pos.X},{player.Pos.Y - Pos.Y}]");
                    }
                    Console.WriteLine($"怪兽移动到了{Pos.X},{Pos.Y}");
                    return 0;
                }
                else return wait;
            }
        }
        public async void AI_Analysis()
        {
            try
            {
                int wait = 0;
                int min = -1;
                int temp;
                int cnt = 0;
                int can_Attack_x = 4, can_Attack_y = 4;
                Pos select = null;
                Player min_direct = null;
                List<Player> monster_directs = new List<Player>();
                while (true)
                {
                    Console.WriteLine("怪兽线程进行第" + ++cnt + "次循环");
                    monster_directs.Clear();
                    select = null;
                    min_direct = null;
                    min = -1;
                    if (wait != 0 && wait > 10 && wait <= 30)
                    {
                        await Task.Delay(wait * 1000);
                        Console.WriteLine("怪兽线程由于无法移动开始睡眠:" + wait + "秒");
                    }
                    else
                    {
                        await Task.Delay(10000);
                        Mp = 20;
                        Console.WriteLine("怪兽线程正常进入10秒一次的CD");
                    }
                    foreach (Player remove in Enemies.Where((Player item) => item.Active != Enums.Player_Active.Map))
                    {
                        Enemies.Remove(remove);
                    }
                    foreach (Player player in Enemies.ToArray())
                    {
                        if (player.Active != Enums.Player_Active.Map || player.Map == null)
                        {
                            Enemies.Remove(player);
                            continue;
                        }
                        temp = Distance(player);
                        if (min > temp || min == -1)
                        {
                            if (temp > can_Attack_x + can_Attack_x + 5)
                            {
                                Cancel_Direct(player);
                                Console.WriteLine($"怪兽距离{player.NickName}太远消除了仇恨");
                            }
                            else
                            {
                                monster_directs.Add(player);
                                min_direct = player;
                                min = temp;
                            }
                        }
                    }
                    foreach (Player player in Friends.ToArray())
                    {
                        if (Distance(player) >= can_Attack_x + can_Attack_x + 5)
                        {
                            Cancel_Friend(player);
                            Console.WriteLine(player.NickName + "距离太远" + NickName + "怪兽取消了朋友关系");
                        }
                    }

                    if (min != -1)
                    {
                        if (min >= can_Attack_x + can_Attack_y)
                        {
                            Console.WriteLine("距离太远了，怪兽决定走路");
                            for (int y = min_direct.Pos.Y - can_Attack_y; y <= min_direct.Pos.Y + can_Attack_y && y <= Map.Height; y++)
                            {
                                if (y < 0) continue;
                                for (int x = min_direct.Pos.X - can_Attack_x; x <= min_direct.Pos.X + can_Attack_x && x <= Map.Width; x++)
                                {
                                    if (x < 0) continue;
                                    if(Map.Pos_Map[x, y].Item != this)
                                    {
                                        if ((x != min_direct.Pos.X || y != min_direct.Pos.Y) && !(Map.Pos_Map[x, y].Item is Player))
                                        {
                                            if (min > Pos.Distance(Map.Pos_Map[x, y]) || min == -1)
                                            {
                                                min = Pos.Distance(Map.Pos_Map[x, y]);
                                                select = Map.Pos_Map[x, y];
                                            }
                                        }
                                        else
                                        {
                                            if (Map.Pos_Map[x, y].Item != min_direct)
                                            {
                                                if (Map.Pos_Map[x, y].Item is Monster)
                                                {
                                                    Add_Friend(Map.Pos_Map[x, y].Item as Player);
                                                    Console.WriteLine($"{NickName}交了一个朋友{(Map.Pos_Map[x, y].Item as Player).NickName}");
                                                }
                                                else
                                                {
                                                    Add_Enemy(Map.Pos_Map[x, y].Item as Player);
                                                    Console.WriteLine($"{NickName}找到了一个敌人{(Map.Pos_Map[x, y].Item as Player).NickName}");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (select != null)
                            {
                                Console.WriteLine("怪兽找到了Pos");
                                wait = Move(select);
                                Console.WriteLine("需要休眠" + wait + "秒");
                            }
                        }
                        else
                        {
                            NPC_Helper.AI_Skill_Analysis_Immediate(this, monster_directs);
                        }
                    }
                    else
                    {
                        AI_Analysis_Task = null;
                        Console.WriteLine("怪兽线程销毁");
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion
    }
}
