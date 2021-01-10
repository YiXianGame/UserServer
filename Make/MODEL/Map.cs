using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Make.MODEL
{
    public abstract class  Map
    {
        #region --字段--
        private Pos[,] pos_Map;
        #endregion

        #region --属性--
        public Pos[,] Pos_Map { get => pos_Map; set => pos_Map = value; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Current_Resources_Monster { get; set; } = 0;
        public int Current_Resources_SkillCard { get; set; } = 0;
        public int Current_Resources_State { get; set; } = 0;
        public int Current_Resources_Adventure { get; set; } = 0;
        public List<State> State_Hp_List = new List<State>();
        public List<State> State_Mp_List = new List<State>();
        public AutoResetEvent State_Hp_Manager = new AutoResetEvent(false);
        public AutoResetEvent State_Mp_Manager = new AutoResetEvent(false);
        public AutoResetEvent Map_Mp_Manager = new AutoResetEvent(false);
        public Dictionary<long,Player> Players = new Dictionary<long, Player>();
        #endregion

        #region --方法--
        public abstract void Action_Stage(Player player, string[] messages);
        public abstract void Resource_Init();

        public void Leave(Player player)
        {
            User user = User.Load(player.ID);
            foreach (SkillCard item in player.Hand_SkillCards.Values)
            {
                //user.Repository_Skill_Add(item);
            }
            player.Pos.Init();
            player.Enemies.ToList().ForEach(new Action<Player>(item => player.Cancel_Direct(item)));

            player.Enemiesed.ToList().ForEach(new Action<Player>(item => item.Cancel_Direct(player)));

            player.Friends.ToList().ForEach(new Action<Player>(item => player.Friends.Remove(item)));

            player.Friendsed.ToList().ForEach(new Action<Player>(item => item.Cancel_Friend(player)));

            GeneralControl.Players.Remove(player.ID);
            player.Hand_SkillCards.Clear();
            player.Init();
            player.Send("离开地图#成功");
            Players.Remove(player.ID);
            GeneralControl.Menu_Data_Monitor_Class.Instance.Players = $"当前在线:{GeneralControl.Players.Count}人";
            GeneralControl.Menu_Data_Monitor_Class.Instance.Map_Players = $"当前地图在线:{GeneralControl.Map.Players.Count}人";
            GeneralControl.Menu_Data_Monitor_Class.Instance.Room_Players = $"当前房间在线:{GeneralControl.Players.Count - GeneralControl.Map.Players.Count}人";
        }
        public void Enter(User user)
        {
            Random random = new Random();
            if (GeneralControl.Menu_GameControl_Class.Instance.Map_Enter_Is_Coast)
            {
                if (user.Add_Balances(-GeneralControl.Menu_GameControl_Class.Instance.Map_Enter_Coast))
                {
                    user.Save();
                    GeneralControl.Menu_Data_Monitor_Class.Instance.Players = $"当前在线:{GeneralControl.Players.Count}人";
                    GeneralControl.Menu_Data_Monitor_Class.Instance.Map_Players = $"当前地图在线:{GeneralControl.Map.Players.Count}人";
                    GeneralControl.Menu_Data_Monitor_Class.Instance.Room_Players = $"当前房间在线:{GeneralControl.Players.Count - GeneralControl.Map.Players.Count}人";
                }
                else
                {
                    return;
                }
                
                
            }
            Pos[] Leisure_Pos = (from Pos item in GeneralControl.Map.Pos_Map where item.Item == null select item).ToArray();
            if (Leisure_Pos.Count() > 1)
            {
                Player player = new Player(user);
                player.Init();
                player.Map = GeneralControl.Map;
                player.Active = Enums.Player_Active.Map;
                player.Move(Leisure_Pos[random.Next(0, Leisure_Pos.Count() - 1)]);
                
                player.Hp = GeneralControl.Menu_GameControl_Class.Instance.Map_Hp_Max;
                player.Mp = 20;//**GeneralControl.Menu_GameControl_Class.Instance.Map_Hp_Max / 3

                GeneralControl.Players.Add(player.ID, player);
                Players.Add(player.ID,player);
                if (Players.Count <= 1) Map_Mp_Manager.Set();
                /*
                foreach(Simple_SkillCard simple_SkillCard in player.Battle_SkillCards.Values)
                {
                    player.Hand_Skill_Add(GeneralControl.Skill_Card_Name_Skllcard[simple_SkillCard.Name].Clone(), false);
                }
                */
                GeneralControl.Menu_Data_Monitor_Class.Instance.Players = $"当前在线:{GeneralControl.Players.Count}人";
                GeneralControl.Menu_Data_Monitor_Class.Instance.Map_Players = $"当前地图在线:{GeneralControl.Map.Players.Count}人";
                GeneralControl.Menu_Data_Monitor_Class.Instance.Map_Players = $"当前地图在线:{GeneralControl.Players.Count - GeneralControl.Map.Players.Count}人";
                player.Send("进入房间#成功",player.Room);
            }
            else user.SendMessages("地图位置已满，暂时无法进入");
        }
        public Map(int H,int W)
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
            Thread State_Hp_Thread = new Thread(State_Hp);
            Thread State_Mp_Thread = new Thread(State_Mp);
            Thread Map_Mp_Thread = new Thread(Map_Mp);
            State_Hp_Thread.Start();
            State_Mp_Thread.Start();
            Map_Mp_Thread.Start();
        }
        public void State_Hp()
        {
            while (true)
            {
                Console.WriteLine("回血线程恢复运行");
                if (State_Hp_List.Count == 0)
                {
                    Console.WriteLine("回血线程开始停滞");
                    State_Hp_Manager.WaitOne();
                }
                else
                {
                    lock (State_Hp_List)
                    {
                        foreach (State item in State_Hp_List.ToArray())
                        {
                            if (!item.Is_Expire())
                            {
                                if (item.Effect_mp != 0)
                                {
                                    item.Direct.Add_Hp(item.Effect_mp);
                                }
                                if (item.Direct.Is_Death)
                                {
                                    item.Direct.OnDeath(this,new EventArgsModel.DeathEventArgs(item.Owner, item.Direct));
                                }
                            }
                            else
                            {
                                State_Hp_List.Remove(item);
                                Console.WriteLine("BUFF到时间了");
                            }
                        }
                    }
                }
                Thread.Sleep(5000);
            }
        }
        public void State_Mp()
        {
            while (true)
            {
                if (State_Mp_List.Count == 0) State_Mp_Manager.WaitOne();
                else
                {
                    lock (State_Mp_List)
                    {
                        foreach (State item in State_Mp_List.ToArray())
                        {
                            if (!item.Is_Expire())
                            {
                                if (item.Effect_mp != 0)
                                {
                                    item.Direct.Add_Mp(item.Effect_mp);
                                }
                            }
                            else
                            {
                                State_Hp_List.Remove(item);
                            }
                        }
                    }
                }
                Thread.Sleep(5000);
            }
        }
        public void Map_Mp()
        {
            while (true)
            {
                Console.WriteLine("回魔线程运行");
                if (Players.Count == 0)
                {
                    Console.WriteLine("回魔线程开始停滞");
                    Map_Mp_Manager.WaitOne();
                }
                else
                {
                    lock (Players)
                    {
                        foreach (Player item in Players.Values)
                        {
                            item.Add_Mp(3);
                        }
                    }
                }
                Thread.Sleep(10000);
            }
        }
        #endregion
    }
}
