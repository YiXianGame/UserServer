using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Make.MODEL
{
    public abstract class Room
    {
        #region --字段--
        private int round = 0;//房间回合
        private List<Player> players = new List<Player>();//房间内的玩家
        private Enums.Room stage = Enums.Room.Wait;//房间阶段
        private Enums.Room_Type type = Enums.Room_Type.Solo;//房间类型
        private int deaths = 0;//死亡总数
        private DateTime latest_Date = DateTime.Now;//房间最新时间
        private int max_Personals;//最大玩家数
        private int min_Personals;//最少玩家数
        #endregion

        #region --属性--
        public int Round { get => round; set => round = value; }
        public List<Player> Players { get => players; set => players = value; }
        [JsonConverter(typeof(String))]
        public Enums.Room Stage { get => stage; set => stage = value; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Enums.Room_Type Type { get => type; set => type = value; }
        public int Deaths { get => deaths; set => deaths = value; }
        public DateTime Latest_Date { get => latest_Date; set => latest_Date = value; }
        public int Max_Personals { get => max_Personals; set => max_Personals = value; }
        public int Min_Personals { get => min_Personals; set => min_Personals = value; }
        #endregion

        #region --方法--
        public abstract void SendMessages(string msg, object bound = null);
        public abstract void Start();
        public abstract void Enter(User user);
        public abstract void Enter(Player robot);
        public abstract void Leave(Player player);
        public abstract void Force_Close();
        public abstract void Raise_Stage();
        public abstract void Result_Stage();
        public abstract void Action_Stage(Player player, string[] messages);
        #endregion
    }
}
