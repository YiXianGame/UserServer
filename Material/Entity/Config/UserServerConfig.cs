using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Material.Entity.Config
{
    public class UserServerConfig
    {
        #region --Enum--
        [JsonConverter(typeof(StringEnumConverter))]
        public enum UserServerCategory { LowUserServer, StandardUserServer, HighUserServer }
        #endregion

        #region --字段--
        private long skillCardUpdate;
        private UserServerCategory category = UserServerCategory.LowUserServer;
        private int maxBuff = 8; //当前状态的最大量
        private PlayerServerConfig playerServerConfig;
        #endregion

        #region --属性--
        public long SkillCardUpdate { get => skillCardUpdate; set => skillCardUpdate = value; }
        public UserServerCategory Category { get => category; set => category = value; }
        public int MaxBuff { get => maxBuff; set => maxBuff = value; }
        public PlayerServerConfig PlayerServerConfig { get => playerServerConfig; set => playerServerConfig = value; }


        #endregion
    }
}
