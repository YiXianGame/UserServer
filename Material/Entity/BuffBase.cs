using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Material.Entity
{
    [JsonObject(MemberSerialization.OptOut)]
    public class BuffBase
    {
        #region --Enum--
        [JsonConverter(typeof(StringEnumConverter))]
        public enum BuffCategory { Freeze };
        #endregion

        #region --字段--
        private int duration;//持续时长
        private int power;//作用范围
        private BuffCategory category = BuffCategory.Freeze;
        #endregion

        #region --属性--
        public int Duration { get => duration; set => duration = value; }
        public int Power { get => power; set => power = value; }
        public BuffCategory Category { get => category; set => category = value; }
        #endregion

    }
}
