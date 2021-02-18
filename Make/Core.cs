using Make.RPC.Request;
using Material.Entity;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Make
{
    public static class Core
    {
        #region --字段--
        private static Config config;
        private static ConcurrentQueue<User> queue_Solo = new ConcurrentQueue<User>();//单挑匹配队列
        private static ConcurrentQueue<User> queue_Team = new ConcurrentQueue<User>();//团战匹配队列
        private static ConcurrentQueue<User> queue_Battle_Royale = new ConcurrentQueue<User>();//大逃杀匹配队列
        private static Dictionary<long, SkillCard> skill_Card_ID_Skllcard = new Dictionary<long, SkillCard>();
        private static Model.Repository repository;
        private static UserRequest userRequest;
        private static SkillCardRequest skillCardClient;
        #endregion

        #region --属性--
        public static ConcurrentQueue<User> Queue_Solo { get => queue_Solo; set => queue_Solo = value; }
        public static ConcurrentQueue<User> Queue_Team { get => queue_Team; set => queue_Team = value; }
        public static ConcurrentQueue<User> Queue_Battle_Royale { get => queue_Battle_Royale; set => queue_Battle_Royale = value; }
        public static Dictionary<long, SkillCard> SkillCardByID { get => skill_Card_ID_Skllcard; set => skill_Card_ID_Skllcard = value; }
        public static Model.Repository Repository { get => repository; set => repository = value; }
        public static UserRequest UserRequest { get => userRequest; set => userRequest = value; }

        public static SkillCardRequest SkillCardRequest{ get => skillCardClient; set => skillCardClient = value; }
        public static Config Config { get => config; set => config = value; }
        #endregion

    }
}
