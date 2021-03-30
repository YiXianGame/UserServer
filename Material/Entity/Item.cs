using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Material.Entity
{
    [JsonObject(MemberSerialization = MemberSerialization.OptOut)]
    public class Item
    {
        #region --Enum--
        [JsonConverter(typeof(StringEnumConverter))]
        public enum CardRepositoryCategory { SkillCard }
        #endregion

        #region --字段--
        long ownerId;
        long itemId;
        CardRepositoryCategory category;
        #endregion

        #region --属性--
        public long OwnerId { get => ownerId; set => ownerId = value; }
        public long ItemId { get => itemId; set => itemId = value; }
        public CardRepositoryCategory Category { get => category; set => category = value; }
        #endregion
    }
}
