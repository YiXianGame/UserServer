
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Material.Entity
{
    [JsonObject(MemberSerialization.OptOut)]
    public class SkillCard
    {

        #region --Enum--
        [JsonConverter(typeof(StringEnumConverter))]
        public enum SkillCardCategory { Physics, Magic, Attack, Cure, Eternal };
        #endregion

        #region --字段--
        protected long id;
        protected string name = "";//技能卡名称
        protected string description = "";//技能介绍
        protected int mp;//所需能量
        protected int probability;//概率
        protected int auxiliaryHp;
        protected int auxiliaryMp;
        protected int enemyHp;
        protected int enemyMp;
        protected int maxEnemy;//最大锁定敌人数
        protected int maxAuxiliary;
        protected long authorId;
        protected long registerDate;
        protected long attributeUpdate;
        protected List<Buff> auxiliaryBuff = new List<Buff>();//状态
        protected List<Buff> enemyBuff = new List<Buff>();//状态
        protected List<SkillCardCategory> category = new List<SkillCardCategory>();
        List<User> enemy = new List<User>();
        List<User> auxiliary = new List<User>();
        #endregion

        #region --属性--
        public long Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }
        public int Mp { get => mp; set => mp = value; }
        public int Probability { get => probability; set => probability = value; }
        public int AuxiliaryHp { get => auxiliaryHp; set => auxiliaryHp = value; }
        public int AuxiliaryMp { get => auxiliaryMp; set => auxiliaryMp = value; }
        public int EnemyHp { get => enemyHp; set => enemyHp = value; }
        public int EnemyMp { get => enemyMp; set => enemyMp = value; }
        public int MaxEnemy { get => maxEnemy; set => maxEnemy = value; }
        public int MaxAuxiliary { get => maxAuxiliary; set => maxAuxiliary = value; }
        public long AuthorId { get => authorId; set => authorId = value; }
        public long RegisterDate { get => registerDate; set => registerDate = value; }
        public long AttributeUpdate { get => attributeUpdate; set => attributeUpdate = value; }
        public List<Buff> AuxiliaryBuff { get => auxiliaryBuff; set => auxiliaryBuff = value; }
        public List<Buff> EnemyBuff { get => enemyBuff; set => enemyBuff = value; }
        public List<SkillCardCategory> Category { get => category; set => category = value; }

        [JsonIgnore]
        public List<User> Enemy { get => enemy; set => enemy = value; }
        [JsonIgnore]
        public List<User> Auxiliary { get => auxiliary; set => auxiliary = value; }
        #endregion
    }
}
