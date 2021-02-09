using Make.RPC.Request;
using Material.Entity;
using System.Collections.Generic;

namespace Make
{
    public static class Core
    {
        #region --字段--
        private static Config config;
        private static List<User> queue_Solo = new List<User>();//单挑匹配队列
        private static List<User> queue_Team = new List<User>();//团战匹配队列
        private static List<User> queue_Battle_Royale = new List<User>();//大逃杀匹配队列
        private static Dictionary<long, SkillCard> skill_Card_ID_Skllcard = new Dictionary<long, SkillCard>();
        private static Model.Repository repository;
        private static UserClient userRequest;
        #endregion

        #region --属性--
        public static List<User> Queue_Solo { get => queue_Solo; set => queue_Solo = value; }
        public static List<User> Queue_Team { get => queue_Team; set => queue_Team = value; }
        public static List<User> Queue_Battle_Royale { get => queue_Battle_Royale; set => queue_Battle_Royale = value; }
        public static Dictionary<long, SkillCard> Skill_Card_ID_Skllcard { get => skill_Card_ID_Skllcard; set => skill_Card_ID_Skllcard = value; }
        public static Model.Repository Repository { get => repository; set => repository = value; }
        public static UserClient UserRequest { get => userRequest; set => userRequest = value; }
        public static Config Config { get => config; set => config = value; }
        #endregion

    }
}
