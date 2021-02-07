using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Material.Entity
{
    public class CoreBase
    {
        #region --Enum--
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ConfigCategory { LowServer, StandardServer, HighServer, LowUserSystem }
        #endregion


        #region --字段--
        private ConfigCategory category = ConfigCategory.StandardServer;
        private long skillCardUpdate;
        #endregion

        #region --字段--
        public ConfigCategory Category { get => category; set => category = value; }
        public long SkillCardUpdate { get => skillCardUpdate; set => skillCardUpdate = value; }
        #endregion
    }
}
