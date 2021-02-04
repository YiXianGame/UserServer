using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Make.MODEL
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Player
    {
        #region --委托--
        public delegate void DeathDelegate(Object sender, EventArgsModel.DeathEventArgs args);
        #endregion

        #region --事件--
        public event DeathDelegate DeathEvent;
        #endregion

        #region --事件发生器--
        public void OnDeath(Object Sender, EventArgsModel.DeathEventArgs args) => DeathEvent(Sender, args);
        #endregion

        #region --字段--
        private long id ;//玩家ID也是QQ号（-1时为机器人）
        private string userName;//用户名
        private string nickName;//玩家昵称
        private int hp;//血量
        private int mp;//能量
        private int hp_max;//血量上限
        private int mp_max;//仙气上限
        private string title;
        private Room room;//房间
        private Enums.Race race;//种族
        private int team;//所属队伍
        private Pos pos;//地标
        private Map map;//地图
        private List<Player> enemies = new List<Player>();//玩家锁定
        private List<Player> enemiesed = new List<Player>();
        private List<Player> friends = new List<Player>();//队友
        private List<Player> friendsed = new List<Player>();
        private List<State> states = new List<State>();//战斗状态
        private bool is_Death;//是否死亡
        private int kills;//击杀数
        private int deaths;//死亡数
        private int death_Round;//死亡回合
        private bool action;//是否出招
        private Dictionary<string, SkillCard> hand_SkillCards = new Dictionary<string, SkillCard>();//手中的技能卡
        private SkillCard action_Skill;//释放的技能
        private long from_Group=-1;//QQ群号
        private DateTime leisure;
        private bool is_Robot=false;
        private Enums.Player_Active active;
        #endregion

        #region --属性--

        public int Hp 
        { 

            get => hp;
            set 
            { 
                hp = value;
                if (hp <= 0)
                {                    
                    Is_Death = true;
                }
                else 
                { 
                    Is_Death = false;
                }
            }
        }
        public int Mp { get => mp; set => mp = value; }
        public Dictionary<string, SkillCard> Hand_SkillCards { get => hand_SkillCards; set => hand_SkillCards = value; }
        public Room Room { get => room; set => room = value; }
        public SkillCard Action_Skill { get => action_Skill; set => action_Skill = value; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Enums.Race Race { get => race; set => race = value; }
        public bool Action { get => action; set => action = value; }
        public int Team { get => team; set => team = value; }
        public long From_Group { get => from_Group; set => from_Group = value; }
        [JsonIgnore]
        public Pos Pos { get => pos; set => pos = value; }
        [JsonIgnore]
        public Map Map { get => map; set => map = value; }
        public List<Player> Enemiesed { get => enemiesed; set => enemiesed = value; }
        public List<Player> Enemies { get => enemies; set => enemies = value; }
        public bool Is_Robot { get => is_Robot; set => is_Robot = value; }
        public DateTime Leisure { get => leisure; set => leisure = value; }
        public List<Player> Friends { get => friends; set => friends = value; }
        public List<Player> Friendsed { get => friendsed; set => friendsed = value; }
        public bool Is_Death { get => is_Death; set => is_Death = value; }
        public int Kills { get => kills; set => kills = value; }
        public int Deaths { get => deaths; set => deaths = value; }
        public int Death_Round { get => death_Round; set => death_Round = value; }
        public List<State> States { get => states; set => states = value; }
        public int Hp_max { get => hp_max; set => hp_max = value; }
        public int Mp_max { get => mp_max; set => mp_max = value; }
        public string Title { get => title; set => title = value; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Enums.Player_Active Active { get => active; set => active = value; }
        public long ID { get => id; set => id = value; }
        public string NickName { get => nickName; set => nickName = value; }
        public string UserName { get => userName; set => userName = value; }

        #endregion

        #region --方法--

        public void Send(string msg,object bound = null)
        {
            //token.Send(Enums.Msg_Server_Type.Game,msg, bound);
        }
        public Player()
        {

        }
        public Player(User user)
        {
            NickName = user.NickName;
            Title = user.Title;

        }
        public void Add_States(State state)
        {
            if (active == Enums.Player_Active.Round)
            {
                state.Expire_Immediate = DateTime.Now.AddSeconds(state.Duration_Immediate);
            }
            else
            {
                state.Expire_Round = Room.Round += state.Duration_Round;
            }
            //messages += Name + "获得了" + state.Duration_Immediate + "秒的【" + state.Name + "】BUFF\n";        
            States.Add(state);
            if (state.Name == "恒血")
            {
                Map.State_Hp_List.Add(state);
                if (Map.State_Hp_List.Count() <= 1) Map.State_Hp_Manager.Set();
            }
            if (state.Name == "恒气")
            {
                Map.State_Mp_List.Add(state);
                if (Map.State_Hp_List.Count() <= 1) Map.State_Mp_Manager.Set();
            }
            return;
        }
        /// <summary>
        /// 增减能量
        /// </summary>
        /// <param name="value">增减量</param>
        /// <param name="reason">理由，事情执行的前缀</param>
        /// <returns></returns>
        public int Add_Mp(int value)
        {
            //**这里写RPC之类的
            if (value<0)
            {
                Mp += value;
                return 0;
            }
            else
            {
                State state = State_Is_Exist("封穴");
                value -= state.Effect_mp;
                if (value > 0)
                {
                    Mp += value;
                    return 1;
                }
                return 2;
            }
        }
        /// <summary>
        /// 增减血量
        /// </summary>
        /// <param name="value">增减量</param>
        /// <param name="reason">理由</param>
        /// <returns></returns>
        public int Add_Hp(int value)
        {
            //**这里写RPC之类的
            if (value<0 && State_Is_Exist("无敌", Math.Abs(value))!=null)
            {
                return 1;
            }
            else
            {
                Hp += value;
                return 0;
            }
        }

        /// <summary>
        /// 展示该玩家当前手牌信息
        /// </summary>
        /// <returns></returns>
        public string Show_Skills_Hand_Round()
        {
            string messages="";
            foreach(SkillCard skillCard in hand_SkillCards.Values)
            {
                string state_temp="";
                if (skillCard.Buff.Count > 0) state_temp = " 状态 ";
                foreach(State state in skillCard.Buff)
                {
                    state_temp += "（" + state.Name + ")能力 " + state.Effect_mp + " 回合 " + state.Duration_Round;
                }
                messages += "【" + skillCard.Name + "】"+state_temp + "\n"; 
            }
            return messages;
        }
        /// <summary>
        /// 刷新状态（回合）
        /// </summary>
        public void Refresh_State()
        {
            foreach(State state in States)
            {
                state.Is_Expire();
            }
        }
        public void OnDeath_Room(Object Sender, EventArgsModel.DeathEventArgs args)
        {
            args.Killed.Is_Death = true;
        }
        public void OnDeath_Map(Object Sender,EventArgsModel.DeathEventArgs args)
        {
            Player killer = args.Killer;
            Player killed = args.Killed;
            User killer_User = User.Load(killer.ID);
            User killed_User = User.Load(killed.ID);
            long exp = killed_User.Exp / 10;
            Map.Players.Values.ToList().ForEach((Player item) => item.Send($"【{killer_User.Title}】{NickName}陨落!击毙者【{killer_User.Title}】{killer.NickName}"));
            foreach (SkillCard item in Hand_SkillCards.Values)
            {
                item.Owner_ID = killer.ID;
                killer.Hand_Skill_Add(item);
            }
            killer_User.Exp += exp;
            killed_User.Exp -= exp;
            killer.Send("击杀", args);
            Send($"死亡#您已被{killer.NickName}击杀!!!\n损失了{exp}点经验");
            foreach (Player item in Enemiesed.Union(Friends))
            {
                item.Send($"死亡广播#{NickName}已被击杀,陨落于仙域了");
            }
            Map.Leave(this);
        }
        /// <summary>
        /// 该mp作用效果之下的状态是否存在
        /// </summary>
        /// <param name="state_Name">状态</param>
        /// <param name="mp">作用范围</param>
        /// <returns></returns>
        public State State_Is_Exist(string state_Name,int mp)
        {
            State max = null;
            foreach(State state in States)
            {
                if (state.Is_Expire()) continue ;
                if (state.Name == state_Name && state.Effect_mp >= mp && (max !=null || state.Effect_mp >= max.Effect_mp))
                {
                    max = state;
                }
            }
            return max;
        }
        /// <summary>
        /// 状态是否存在
        /// </summary>
        /// <param name="state_Name"></param>
        /// <returns>返回该状态的有效范围,不存在返回-1</returns>
        public State State_Is_Exist(string state_Name)
        {
            State max = null;
            foreach (State state in States)
            {
                if (state.Is_Expire()) continue;
                if (state.Name == state_Name && state.Effect_mp >= mp && (max != null || state.Effect_mp >= max.Effect_mp))
                {
                    max = state;
                }
            }
            return max;
        }

        /// <summary>
        /// 添加技能卡
        /// </summary>
        /// <param name="add_Skill_Card"></param>
        public bool Hand_Skill_Add(SkillCard add_Skill_Card)
        {
            if (Hand_SkillCards.TryGetValue(add_Skill_Card.Name, out SkillCard skillCard))
            {

            }
            else
            {

            }
            return true;
        }
        

        
       /// <summary>
       /// 初始化
       /// </summary>
        public void Init()
        {
                Action = false;
                Hp = Core.Menu_GameControl_Class.Instance.Room_Hp_Max;
                Mp = 0;
                Action_Skill = null;
                Action = false;
                Room = null;
                Map = null;
                Pos = null;
                Team = -1;
                Deaths = 0;
                Enemies.Clear();
                Enemiesed.Clear();
                Friends.Clear();
                Friendsed.Clear();
                Hand_SkillCards.Clear();
                States.Clear();
                Action = false;
                Action_Skill = null;
                Kills = 0;
                Death_Round = 0;
        }

        //Map操作
        public void Move(Pos pos)
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
                if (dis_X + dis_Y <= Mp)
                {
                    if (Pos.Item != null) Pos.Init();
                    Pos = pos;
                    Pos.Effect(this);
                    Pos.Init();
                    pos.Add(this);
                    Add_Mp(-(dis_X + dis_Y));
                    foreach (Player player in Enemiesed)
                    {
                        player.Send($"魂命:{NickName}已移动至[{Pos.X},{Pos.Y}]\n与您相距[{Pos.X - player.Pos.X},{player.Pos.Y - Pos.Y}]");
                    }
                    foreach (Player player in Friendsed)
                    {
                        player.Send($"灵命:{NickName}已移动至[{Pos.X},{Pos.Y}]\n与您相距[{Pos.X - player.Pos.X},{player.Pos.Y - Pos.Y}]");
                    }
                    Review();
                    Send($"您当前的地理位置 X:{Pos.X} Y:{Pos.Y}");
                }
                else Send("仙气不足，无法移动至此地点，需要" + (dis_Y + dis_X) + "点仙气");
            }
        }

        public void Review()
        {
            string result = "";
            int w = 5, h = 5;
            for (int y = Pos.Y - h; y <= Pos.Y + h && y <= Map.Height; y++)
            {
                if (y < 0) continue;
                for (int x = Pos.X - w; x <= Pos.X + w && x <= Map.Width; x++)
                {
                    if (x < 0) continue;
                    if (x != Pos.X || y != Pos.Y) result += Map.Pos_Map[x, y].Name;
                    else result += "👁";
                    if (Map.Pos_Map[x, y].Item is Monster && !(Map.Pos_Map[x, y].Item as Monster).Enemies.Contains(this))
                    {
                        (Map.Pos_Map[x, y].Item as Monster).Attack(this);
                    }
                }
                if(y!=Pos.Y + h)result += "\n";
            }
            Send(result);
        }
        public void Add_Enemy(Player directed)
        {
            Enemies.Add(directed);
            directed.Enemiesed.Add(this);
            Send(directed.NickName + "锁定成功");
            directed.Send($"您已被{NickName}魂命锁定,化为世仇 对方当前位置:[{Pos.X},{Pos.Y}]");
        }
        public void Cancel_Direct(Player directed)
        {
            Enemies.Remove(directed);
            directed.Enemiesed.Remove(this);
            directed.Send(NickName + "已将您的魂命取消");
            Send(directed.NickName + "已成功取消魂命");
        }
        public void Add_Friend(Player friended)
        {
            Friends.Add(friended);
            friended.Friendsed.Add(this);
            Send(friended.NickName + "锁定成功");
            friended.Send($"您已被{NickName}灵命锁定,结为盟友 对方当前位置:[{Pos.X},{Pos.Y}]");
        }
        public void Cancel_Friend(Player friended)
        {
            Friends.Remove(friended);
            friended.Friendsed.Remove(this);
            friended.Send(NickName + "已将您的灵命取消");
            Send(friended.NickName + "已成功取消灵命");
        }
        public int Distance(Player player)
        {
            return Math.Abs(player.Pos.X - Pos.X) + Math.Abs(player.Pos.Y - Pos.Y);
        }
        #endregion
    }
}
