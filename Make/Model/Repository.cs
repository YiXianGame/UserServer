using Make.Repository;
using Material.MySQL;
using Material.Redis;

namespace Make.Model
{
    public class Repository
    {
        #region --字段--
        private Redis redis;
        private MySQL mySQL;
        private UserRepository userRepository;
        private SkillCardRepository skillCardRepository;
        private ConfigRepository configRepository;
        #endregion

        #region --属性--
        public UserRepository UserRepository { get => userRepository; set => userRepository = value; }
        public SkillCardRepository SkillCardRepository { get => skillCardRepository; set => skillCardRepository = value; }
        public ConfigRepository ConfigRepository { get => configRepository; set => configRepository = value; }
        #endregion

        #region --方法--
        public Repository(Redis redis, MySQL mySQL)
        {
            this.redis = redis;
            this.mySQL = mySQL;
            UserRepository = new UserRepository(redis, mySQL);
            SkillCardRepository = new SkillCardRepository(redis, mySQL);
            ConfigRepository = new ConfigRepository(redis, mySQL);
        }
        #endregion
    }
}
