using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Material.Entity
{
    [JsonObject(MemberSerialization.OptOut)]
    public class UserBase
    {
        #region --Enum--
        [JsonConverter(typeof(StringEnumConverter))]
        public enum State { Leisure, Ready, Queue, Gaming, Offline };
        #endregion

        #region --字段--

        protected long id;

        protected string username;

        protected string password;

        protected byte[] headImage;

        protected string nickname;

        protected int upgrade_num = 0;

        protected int create_num = 0;

        protected long money = 0;

        protected string personalSignature;

        protected int battleCount;//战斗场次

        protected long exp;//经验

        protected int lv = 1;//等级

        protected string title = "炼气";//称号

        protected State active = State.Offline;//玩家当前游戏状态

        protected int kills;//击杀数

        protected int deaths;//死亡数

        protected long registerDate;//注册日期

        protected long attribute_update;//个人信息更新日期

        protected long skillCard_update;//卡牌更新日期

        protected long headImage_update;//头像更新日期

        #endregion

        #region --属性--

        public long Id { get => id; set => id = value; }
        public string Username { get => username; set => username = value; }

        public string Password { get => password; set => password = value; }
        public byte[] HeadImage { get => headImage; set => headImage = value; }
        public string Nickname { get => nickname; set => nickname = value; }
        public int Upgrade_num { get => upgrade_num; set => upgrade_num = value; }
        public int Create_num { get => create_num; set => create_num = value; }
        public long Money { get => money; set => money = value; }
        public string PersonalSignature { get => personalSignature; set => personalSignature = value; }
        public int BattleCount { get => battleCount; set => battleCount = value; }
        public long Exp { get => exp; set => exp = value; }
        public int Lv { get => lv; set => lv = value; }
        public string Title { get => title; set => title = value; }
        public State Active { get => active; set => active = value; }
        public int Kills { get => kills; set => kills = value; }
        public int Deaths { get => deaths; set => deaths = value; }
        public long RegisterDate { get => registerDate; set => registerDate = value; }
        public long Attribute_update { get => attribute_update; set => attribute_update = value; }
        public long SkillCard_update { get => skillCard_update; set => skillCard_update = value; }
        public long HeadImage_update { get => headImage_update; set => headImage_update = value; }
        #endregion
    }
}
