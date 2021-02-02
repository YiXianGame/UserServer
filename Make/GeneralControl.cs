using Make.MODEL;
using Make.MODEL.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;

namespace Make
{
    /// <summary>
    /// 总控
    /// </summary>
    public static class Core
    {
        public static string Directory;//游戏数据文档路径
        public static int MaxLevel = 5;   //技能卡的最大等级
        public static int MaxStates = 8; //当前状态的最大量
        public static bool LazyLoad_SkillCards = true; //是否惰性加载UI卡片
        private static DateTime skill_Card_Date;//技能卡版本
        private static DateTime adventure_Date;//奇遇版本
        public static List<User> Queue_Solo = new List<User>();//单挑匹配队列
        public static List<User> Queue_Team = new List<User>();//团战匹配队列
        public static List<User> Queue_Battle_Royale = new List<User>();//大逃杀匹配队列
        /// <summary>
        /// 仙域地图
        /// </summary>
        public static XianYu_Map Map; //仙域地图
        /// <summary>
        /// 技能卡MODEL
        /// </summary>
        public static List<SkillCard> Skill_Cards = new List<SkillCard>();//总技能卡 //总引用,但UI层还有一层引用，删掉的同时记得删掉UI层
        /// <summary>
        /// ID技能卡
        /// </summary>
        public static Dictionary<long, SkillCard> Skill_Card_ID_Skllcard = new Dictionary<long, SkillCard>();
        /// <summary>
        /// 名字技能卡
        /// </summary>
        public static Dictionary<string, SkillCard> Skill_Card_Name_Skllcard = new Dictionary<string, SkillCard>();
        /// <summary>
        /// 总奇遇
        /// </summary>
        public static List<Adventure> Adventures = new  List<Adventure>();//总引用,但UI层还有一层引用，删掉的同时记得删掉UI层
        /// <summary>
        /// 奇遇字典
        /// </summary>
        public static Dictionary<long, Adventure> Adventures_ID = new Dictionary<long, Adventure>();
        /// <summary>
        /// 总状态
        /// </summary>
        public static Dictionary<long, State> States = new Dictionary<long, State>();
        /// <summary>
        /// 总房间
        /// </summary>
        public static List<Room> Rooms = new List<Room>();
        /// <summary>
        /// 总玩家
        /// </summary>       
        public static Dictionary<long, Player> Players = new Dictionary<long, Player>();

        public static Material.Repository.Repository Repository;

        public static UserRequest UserRequest;
        public static DateTime Skill_Card_Date 
        { 
            get => skill_Card_Date; 
            set
            {
                skill_Card_Date = value;
                Material.Ini.Write("游戏配置", "Skill_Card_Date", skill_Card_Date.ToString(), Core.Directory + @"\游戏配置\GeneralControl.ini");
            }
        }

        public static DateTime Adventure_Date
        {
            get => adventure_Date;
            set
            {
                adventure_Date = value;
                Material.Ini.Write("游戏配置", "Adventure_Date", adventure_Date.ToString(), Core.Directory + @"\游戏配置\GeneralControl.ini");
            }
        }

        static Core()
        {

            if(File.Exists(Core.Directory + @"\游戏配置\GeneralControl.ini"))
            {
                MaxLevel = int.Parse(Material.Ini.Read("游戏配置", "MaxLevel", "5", Core.Directory + @"\游戏配置\GeneralControl.ini"));
                MaxStates = int.Parse(Material.Ini.Read("游戏配置", "MaxStates", "9", Core.Directory + @"\游戏配置\GeneralControl.ini"));
                LazyLoad_SkillCards = bool.Parse(Material.Ini.Read("游戏配置", "LazyLoad_SkillCards", "9", Core.Directory + @"\游戏配置\GeneralControl.ini"));
                Skill_Card_Date = DateTime.Parse(Material.Ini.Read("游戏配置", "Skill_Card_Date", "", Core.Directory + @"\游戏配置\GeneralControl.ini"));
                Adventure_Date = DateTime.Parse(Material.Ini.Read("游戏配置", "Adventure_Date", "", Core.Directory + @"\游戏配置\GeneralControl.ini"));
            }
            else
            {
                Material.Ini.Write("游戏配置", "MaxLevel", MaxLevel.ToString(), Core.Directory + @"\游戏配置\GeneralControl.ini");
                Material.Ini.Write("游戏配置", "MaxStates", MaxStates.ToString(), Core.Directory + @"\游戏配置\GeneralControl.ini");
                Material.Ini.Write("游戏配置", "LazyLoad_SkillCards", LazyLoad_SkillCards.ToString(), Core.Directory + @"\游戏配置\GeneralControl.ini");
                Material.Ini.Write("游戏配置", "Skill_Card_Date", Skill_Card_Date.ToString(), Core.Directory + @"\游戏配置\GeneralControl.ini");
                Material.Ini.Write("游戏配置", "Adventure_Date", Adventure_Date.ToString(), Core.Directory + @"\游戏配置\GeneralControl.ini");
            }
        }
        public static void Save()
        {
            Material.Ini.Write("游戏配置", "MaxLevel", MaxLevel.ToString(), Core.Directory + @"\游戏配置\GeneralControl.ini");
            Material.Ini.Write("游戏配置", "MaxStates", MaxStates.ToString(), Core.Directory + @"\游戏配置\GeneralControl.ini");
            Material.Ini.Write("游戏配置", "LazyLoad_SkillCards", LazyLoad_SkillCards.ToString(), Core.Directory + @"\游戏配置\GeneralControl.ini");
            Material.Ini.Write("游戏配置", "Skill_Card_Date", Skill_Card_Date.ToString(), Core.Directory + @"\游戏配置\GeneralControl.ini");
            Material.Ini.Write("游戏配置", "Adventure_Date", Adventure_Date.ToString(), Core.Directory + @"\游戏配置\GeneralControl.ini");
            File.WriteAllText(Core.Directory + @"\游戏配置\Menu_Skill_Cards_Class.json", JsonConvert.SerializeObject(Core.Menu_Skill_Cards_Class.Instance));
            File.WriteAllText(Core.Directory + @"\游戏配置\Menu_Adventure_Cards_Class.json", JsonConvert.SerializeObject(Core.Menu_Adventure_Cards_Class.Instance));
            File.WriteAllText(Core.Directory + @"\游戏配置\Menu_GameControl_Class.json", JsonConvert.SerializeObject(Core.Menu_GameControl_Class.Instance));
            File.WriteAllText(Core.Directory + @"\游戏配置\Menu_Data_Monitor_Class.json", JsonConvert.SerializeObject(Core.Menu_Data_Monitor_Class.Instance));
            File.WriteAllText(Core.Directory + @"\游戏配置\Menu_Person_Informations_Class.json", JsonConvert.SerializeObject(Core.Menu_Person_Information_Class.Instance));
            File.WriteAllText(Core.Directory + @"\游戏配置\Menu_Command_Class.json", JsonConvert.SerializeObject(Core.Menu_Command_Class.Instance));
            File.WriteAllText(Core.Directory + @"\游戏配置\Menu_Lience_Class.json", JsonConvert.SerializeObject(Core.Menu_Lience_Class.Instance));
            File.WriteAllText(Core.Directory + @"\游戏配置\Menu_Version_Informations_Class.json", JsonConvert.SerializeObject(Core.Menu_Version_Informations_Class.Instance));
        }
        public class Menu_Skill_Cards_Class
        {
            private Menu_Skill_Cards_Class()
            {

            }
            private static readonly Lazy<Menu_Skill_Cards_Class> lazy = new Lazy<Menu_Skill_Cards_Class>(() => 
            {
                if (File.Exists(Core.Directory + @"\游戏配置\Menu_Skill_Cards_Class.json"))
                {
                    return JsonConvert.DeserializeObject<Menu_Skill_Cards_Class>(File.ReadAllText(Core.Directory + @"\游戏配置\Menu_Skill_Cards_Class.json"));
                }
                else return new Menu_Skill_Cards_Class();
            });
            public static Menu_Skill_Cards_Class Instance { get { return lazy.Value; } }
            public void Save()
            {
                File.WriteAllText(Core.Directory + @"\游戏配置\Menu_Skill_Cards_Class.json", JsonConvert.SerializeObject(Core.Menu_Skill_Cards_Class.Instance));
            }
        }
        public class Menu_Adventure_Cards_Class
        {
            private Menu_Adventure_Cards_Class()
            {

            }
            private static readonly Lazy<Menu_Adventure_Cards_Class> lazy = new Lazy<Menu_Adventure_Cards_Class>(() =>
            {
                if (File.Exists(Core.Directory + @"\游戏配置\Menu_Adventure_Cards_Class.json"))
                {
                    return JsonConvert.DeserializeObject<Menu_Adventure_Cards_Class>(File.ReadAllText(Core.Directory + @"\游戏配置\Menu_Adventure_Cards_Class.json"));
                }
                else return new Menu_Adventure_Cards_Class();
            });
            public static Menu_Adventure_Cards_Class Instance { get { return lazy.Value; } }
            public void Save()
            {
                File.WriteAllText(Core.Directory + @"\游戏配置\Menu_Adventure_Cards_Class.json", JsonConvert.SerializeObject(Core.Menu_Adventure_Cards_Class.Instance));
            }
        }
        public class Menu_GameControl_Class
        {
            private Menu_GameControl_Class()
            {

            }
            private static readonly Lazy<Menu_GameControl_Class> lazy = new Lazy<Menu_GameControl_Class>(() =>
            {
                if (File.Exists(Core.Directory + @"\游戏配置\Menu_GameControl_Class.json"))
                {
                    return JsonConvert.DeserializeObject<Menu_GameControl_Class>(File.ReadAllText(Core.Directory + @"\游戏配置\Menu_GameControl_Class.json"));
                }
                else return new Menu_GameControl_Class();
            });
            public static Menu_GameControl_Class Instance { get { return lazy.Value; } }
            public void Save()
            {
                File.WriteAllText(Core.Directory + @"\游戏配置\Menu_GameControl_Class.json", JsonConvert.SerializeObject(Core.Menu_GameControl_Class.Instance));
            }
            public int Adventure_Probability { get; set; } = 50;
            public int Buy_Card_Coast { get; set; } = 10;
            public int Draw_Card_Coast { get; set; } = 100;
            public int Upgrade_Card_Coast { get; set; } = 100;
            public int Sell_Card_Coast { get; set; } = 5;
            public int Battle_Max { get; set; } = 20;
            public int Repository_Max { get; set; } = 100;
            public bool Adventure_Concurrent { get; set; }
            public int Room_Hp_Max { get; set; } = 100;
            public int Room_Mp_Max { get; set; } = 50;
            public int Immediate_To_Round { get; set; } = 10;

            public bool Map_Enable { get; set; } = true;
            public int Map_Enter_Coast { get; set; } = 100;
            public int Map_Players_Max { get; set; } = 100;
            public int Map_Hp_Max { get; set; } = 100;
            public int Map_Mp_Max { get; set; } = 50;
            public bool Map_Enter_Is_Coast { get; set; } = true;
            private int map_Resources_Adventure = 5 * 5;
            private int map_Resources_SkillCard = 5 * 5;
            private int map_Resources_State = 5 * 5;
            private int map_Resources_Monster = 5 * 5;
            public int TP_Coast { get; set; } = 30;
            public bool Can_Buy { get; set; } = true;
            public bool Can_Sell { get; set; } = true;
            public int Map_Size { get; set; } = 100;
            public string Owner_QQ { get; set; } = "839336369|3028394801";//主人QQ

            public int Map_Resources_Adventure
            {
                get => map_Resources_Adventure;
                set
                {   
                    if (Map_Size * Map_Size <= value + Map_Resources_Monster + Map_Resources_SkillCard + Map_Resources_State + Map_Players_Max)
                    {
                        return;
                    }
                    map_Resources_Adventure = value;
                }
            }

            public int Map_Resources_SkillCard
            { 
                get => map_Resources_SkillCard; 
                set
                {
                    if (Map_Size * Map_Size <= Map_Resources_Adventure + Map_Resources_Monster + value + Map_Resources_State + Map_Players_Max)
                    {
                        return;
                    }
                    map_Resources_SkillCard = value;
                }
            }
            public int Map_Resources_State 
            {
                get => map_Resources_State;

                set 
                {
                    if (Map_Size * Map_Size <= Map_Resources_Adventure + Map_Resources_Monster + Map_Resources_SkillCard + value + Map_Players_Max)
                    {
                        return;
                    }
                    map_Resources_State = value; 

                }
            }
            public int Map_Resources_Monster 
            {
                get => map_Resources_Monster;
                set
                {
                    if (Map_Size * Map_Size <= Map_Resources_Adventure + value + Map_Resources_SkillCard + Map_Resources_State + Map_Players_Max)
                    {
                        return;
                    }
                    map_Resources_Monster = value;
                }
            }
        }
        public class Menu_Version_Informations_Class
        {
            public static Menu_Version_Informations_Class Instance { get { return lazy.Value; } }
            private Menu_Version_Informations_Class()
            {

            }
            private static readonly Lazy<Menu_Version_Informations_Class> lazy = new Lazy<Menu_Version_Informations_Class>(() =>
            {
                if (File.Exists(Core.Directory + @"\游戏配置\Menu_Version_Informations_Class.json"))
                {
                    return JsonConvert.DeserializeObject<Menu_Version_Informations_Class>(File.ReadAllText(Core.Directory + @"\游戏配置\Menu_Version_Informations_Class.json"));
                }
                else return new Menu_Version_Informations_Class();
            });
            public string Expiration_Date { get; set; } = DateTime.Now.ToString();
            public void Save()
            {
                File.WriteAllText(Core.Directory + @"\游戏配置\Menu_Version_Informations_Class.json", JsonConvert.SerializeObject(Core.Menu_Version_Informations_Class.Instance));
            }
        }
        public class Menu_Data_Monitor_Class : INotifyPropertyChanged
        {
            public static Menu_Data_Monitor_Class Instance { get { return lazy.Value; } }
            private string players = "当前玩家0人";
            private string map_Players = "当前地图玩家0人";
            private string room_Players = "当前房间玩家0人";
            private string rooms = "当前房间总数0间";
            private string sockets = "当前代理总数0个";
            private Menu_Data_Monitor_Class()
            {

            }
            private static readonly Lazy<Menu_Data_Monitor_Class> lazy = new Lazy<Menu_Data_Monitor_Class>(() =>
            {
                if (File.Exists(Core.Directory + @"\游戏配置\Menu_Data_Monitor_Class.json"))
                {
                    return JsonConvert.DeserializeObject<Menu_Data_Monitor_Class>(File.ReadAllText(Core.Directory + @"\游戏配置\Menu_Data_Monitor_Class.json"));
                }
                else return new Menu_Data_Monitor_Class();
            });
            public string Expiration_Date { get; set; } = DateTime.Now.ToString();
            private ObservableCollection<Adventure> pubmit_Adventures = new ObservableCollection<Adventure>();
            private ObservableCollection<SkillCard> pubmit_SkillCardsModel = new ObservableCollection<SkillCard>();

            public event PropertyChangedEventHandler PropertyChanged;

            public ObservableCollection<SkillCard> Pubmit_SkillCardsModel { get => pubmit_SkillCardsModel; set => pubmit_SkillCardsModel = value; }
            public ObservableCollection<Adventure> Pubmit_Adventures { get => pubmit_Adventures; set => pubmit_Adventures = value; }
            public string Players { get => players; set { players = value; OnPropertyChanged("Players"); } }
            public string Map_Players { get => map_Players; set { map_Players = value; OnPropertyChanged("map_Players"); } }
            public string Room_Players { get => room_Players; set { room_Players = value; OnPropertyChanged("room_Players"); } }
            public string Rooms { get => rooms; set { rooms = value; OnPropertyChanged("rooms"); } }
            public string Sockets { get => sockets; set { sockets = value; OnPropertyChanged("sockets"); } }
            public void Save()
            {
                File.WriteAllText(Core.Directory + @"\游戏配置\Menu_Data_Monitor_Class.json", JsonConvert.SerializeObject(Core.Menu_Data_Monitor_Class.Instance));
            }
            public virtual void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
        public class Menu_Person_Information_Class
        {
            private static readonly Lazy<Menu_Person_Information_Class> lazy = new Lazy<Menu_Person_Information_Class>(() =>
            {
                if (File.Exists(Core.Directory + @"\游戏配置\Menu_Person_Informations_Class.json"))
                {
                    return JsonConvert.DeserializeObject<Menu_Person_Information_Class>(File.ReadAllText(Core.Directory + @"\游戏配置\Menu_Person_Informations_Class.json"));
                }
                else
                {
                    Menu_Person_Information_Class menu_Person_Informations_Class = new Menu_Person_Information_Class();
                    menu_Person_Informations_Class.User.UserName = "839336369";
                    menu_Person_Informations_Class.User.NickName = "剑仙";  
                    menu_Person_Informations_Class.User.Information = "个性签名";
                    menu_Person_Informations_Class.User.Id = 839336369;
                    menu_Person_Informations_Class.User.Save();
                    return menu_Person_Informations_Class;
                }
            });
            public static Menu_Person_Information_Class Instance { get { return lazy.Value; } }
            public event PropertyChangedEventHandler PropertyChanged = delegate { };
            private User user = new User();
            public User User
            {
                get { return user; }
                set
                {
                    if (User != value)
                    {
                        user = value;
                    }
                }
            }
            public void Save()
            {
                File.WriteAllText(Core.Directory + @"\游戏配置\Menu_Person_Informations_Class.json", JsonConvert.SerializeObject(Core.Menu_Person_Information_Class.Instance));
            }
        }
        public class Menu_Command_Class
        {
            public string Chat { get; set; } = "聊天";
            public string GameMenu { get; set; } = "游戏菜单";
            public string Room_Condition { get; set; } = "房间信息";
            public string Create_Room { get; set; } = "创建房间";
            public string Create_User { get; set; } = "创建用户";
            public string Join_Room { get; set; } = "加入房间";
            public string Surrender { get; set; } = "投降";
            public string Leave_Room { get; set; } = "离开房间";
            public string Personal_Information { get; set; } = "个人信息";
            public string Start_Match { get; set; } = "开始匹配";
            public string Stop_Match { get; set; } = "终止匹配";
            public string Version_Information { get; set; } = "版本详情";
            public string Action_Information { get; set; } = "出招查询";
            public string Compulsory_Stop { get; set; } = "强行终止";
            public string Skill_Query { get; set; } = "查询";
            public string Draw_Card { get; set; } = "抽卡";
            public string Buy_Card { get; set; } = "购卡";
            public string View_Repository { get; set; } = "查看库存";
            public string View_Battle { get; set; } = "查看战备";
            public string Upgrade_Card { get; set; } = "升级技能卡";
            public string Sell_Card { get; set; } = "出售";
            public string Add_Card_To_Repository { get; set; } = "加入库存";
            public string Add_Card_To_Battle { get; set; } = "加入战备";
            public string Emoji_Description { get; set; } = "表情介绍";
            public string Map_Condition { get; set; } = "地图信息";
            public string Game_Total { get; set; } = "全局统计";
            public string Review { get; set; } = "探知";
            public string Join_Map { get; set; } = "加入地图";
            public string View_Hand { get; set; } = "查看手牌";
            public string Add_Balances { get; set; } = "充值仙域币";
            public string Leave_Map { get; set; } = "离开地图";
            public string TP { get; set; } = "传送";
            public string Direct { get; set; } = "魂命";
            public string Cancel_Direct { get; set; } = "消魂";
            public string Friend { get; set; } = "灵命";
            public string Cancel_Friend { get; set; } = "消灵";
            public string View_Balances { get; set; } = "查看金额";
            public string AI_Bot { get; set; } = "机器人";
            public string Change_Name { get; set; } = "更改昵称";


            public string View_Player { get; set; } = "清识";
            public static Menu_Command_Class Instance { get { return lazy.Value; } }
            private Menu_Command_Class()
            {

            }
            private static readonly Lazy<Menu_Command_Class> lazy = new Lazy<Menu_Command_Class>(() =>
            {
                if (File.Exists(Core.Directory + @"\游戏配置\Menu_Command_Class.json"))
                {
                    return JsonConvert.DeserializeObject<Menu_Command_Class>(File.ReadAllText(Core.Directory + @"\游戏配置\Menu_Command_Class.json"));
                }
                else return new Menu_Command_Class();
            });
            public void Save()
            {
                File.WriteAllText(Core.Directory + @"\游戏配置\Menu_Command_Class.json", JsonConvert.SerializeObject(Core.Menu_Command_Class.Instance));
            }

        }

        public class Menu_Lience_Class
        {
            public static Menu_Lience_Class Instance { get { return lazy.Value; } }
            private Menu_Lience_Class()
            {

            }
            private static readonly Lazy<Menu_Lience_Class> lazy = new Lazy<Menu_Lience_Class>(() =>
            {
                if (File.Exists(Core.Directory + @"\游戏配置\Menu_Lience_Class.json"))
                {
                    return JsonConvert.DeserializeObject<Menu_Lience_Class>(File.ReadAllText(Core.Directory + @"\游戏配置\Menu_Lience_Class.json"));
                }
                else return new Menu_Lience_Class();
            });
            public void Save()
            {
                File.WriteAllText(Core.Directory + @"\游戏配置\Menu_Lience_Class.json", JsonConvert.SerializeObject(Core.Menu_Lience_Class.Instance));
            }
        }
    }
}
