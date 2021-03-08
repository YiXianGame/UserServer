using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Material.Entity.Config
{
    public class UserServerConfig
    {
        #region --Enum--
        [JsonConverter(typeof(StringEnumConverter))]
        public enum UserServerCategory { LowServer, StandardServer, HighServer }
        #endregion

        #region --字段--
        private long skillCardUpdate;
        private UserServerCategory category = UserServerCategory.LowServer;
        private int maxBuff = 8; //当前状态的最大量
        private PlayerServerConfig playerServerConfig;
        private string ip;
        private string port;
        #endregion

        #region --属性--
        public long SkillCardUpdate { get => skillCardUpdate; set => skillCardUpdate = value; }
        public UserServerCategory Category { get => category; set => category = value; }
        public int MaxBuff { get => maxBuff; set => maxBuff = value; }
        public PlayerServerConfig PlayerServerConfig { get => playerServerConfig; set => playerServerConfig = value; }
        public string Ip { get => ip; set => ip = value; }
        public string Port { get => port; set => port = value; }


        #endregion
    }
}
