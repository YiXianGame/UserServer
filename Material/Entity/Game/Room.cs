using Make.Model.GameModel;
using System;
using System.Collections.Generic;

namespace Material.Entity.Game
{
    public abstract class Room
    {
        public enum RoomType { Solo, Team, Battle_Royale };
        public enum RoomStage { Wait, Raise, Action, Result };
        #region --字段--
        protected int round = 0;//房间回合
        protected List<Player> players = new List<Player>();//房间内的玩家
        protected RoomStage stage = RoomStage.Wait;//房间阶段
        protected RoomType type = RoomType.Solo;//房间类型
        protected int deaths = 0;//死亡总数
        protected DateTime latest_Date = DateTime.Now;//房间最新时间
        protected int max_players = 10;//最大玩家数
        protected int min_players = 2;//最少玩家数
        #endregion

        #region --属性--
        public int Round { get => round; set => round = value; }
        public List<Player> Players { get => players; set => players = value; }
        public RoomStage Stage { get => stage; set => stage = value; }
        public RoomType Type { get => type; set => type = value; }
        public int Deaths { get => deaths; set => deaths = value; }
        public DateTime Latest_Date { get => latest_Date; set => latest_Date = value; }
        public int Max_players { get => max_players; set => max_players = value; }
        public int Min_players { get => min_players; set => min_players = value; }
        #endregion

        #region --方法--
        public abstract void SendMessages(string msg, object bound = null);
        public abstract void Start();
        public abstract void Enter(User user);
        public abstract void Leave(Player player);
        public abstract void Force_Close();
        public abstract void Raise_Stage();
        public abstract void Result_Stage();
        public abstract void Action_Stage(Player player, string[] messages);
        #endregion
    }
}
