using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Material.Entity
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Buff
    {
        #region --Enum--
        [JsonConverter(typeof(StringEnumConverter))]
        public enum BuffCategory { Freeze };
        #endregion

        #region --字段--
        private int duration;//持续时长
        private int power;//作用范围
        private BuffCategory category = BuffCategory.Freeze;
        private User owner;//Buff拥有者
        private long expire;//到期时间
        private bool isExpire;//到期时间
        #endregion

        #region --属性--
        public int Duration { get => duration; set => duration = value; }
        public int Power { get => power; set => power = value; }
        public BuffCategory Category { get => category; set => category = value; }
        [JsonIgnore]
        public User Owner { get => owner; set => owner = value; }
        [JsonIgnore]
        public long Expire { get => expire; set => expire = value; }
        [JsonIgnore]
        public bool IsExpire { get => isExpire; set => isExpire = value; }
        #endregion

    }
}
