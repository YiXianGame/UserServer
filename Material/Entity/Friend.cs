using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Material.Entity
{
    public class Friend
    {
        #region --Enum--
        [JsonConverter(typeof(StringEnumConverter))]
        public enum FriendCategory { Friend }
        #endregion

        #region --字段--
        long user_1;
        long user_2;
        FriendCategory category;
        #endregion

        #region --属性--

        public FriendCategory Category { get => category; set => category = value; }
        public long User_1 { get => user_1; set => user_1 = value; }
        public long User_2 { get => user_2; set => user_2 = value; }
        #endregion
    }
}
