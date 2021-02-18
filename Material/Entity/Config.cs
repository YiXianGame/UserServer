using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Material.Entity
{
    public class Config
    {
        #region --Enum--
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ConfigCategory { LowUserServer, StandardUserServer, HighUserServer }
        #endregion

        #region --字段--
        private long skillCardUpdate;
        private ConfigCategory category = ConfigCategory.LowUserServer;
        private int maxBuff = 8; //当前状态的最大量
        #endregion

        #region --属性--
        public long SkillCardUpdate { get => skillCardUpdate; set => skillCardUpdate = value; }
        public ConfigCategory Category { get => category; set => category = value; }
        public int MaxBuff { get => maxBuff; set => maxBuff = value; }


        #endregion
    }
}
