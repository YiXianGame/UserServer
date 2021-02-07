using System;
using System.Threading.Tasks;
using Material.Entity;
using Material.ExceptionModel;
using MySql.Data.MySqlClient;

namespace Material.Repository
{

    public class Repository
    {
        #region --字段--
        private Redis.Redis redis;
        private MySQL.MySQL mySQL;
        private UserRepository userRepository;
        private SkillCardRepository skillCardRepository;
        private CoreRepository coreRepository;
        #endregion

        #region --属性--
        public UserRepository UserRepository { get => userRepository; set => userRepository = value; }
        public SkillCardRepository SkillCardRepository { get => skillCardRepository; set => skillCardRepository = value; }
        public CoreRepository CoreRepository { get => coreRepository; set => coreRepository = value; }
        #endregion

        #region --方法--
        public Repository(Redis.Redis redis, MySQL.MySQL mySQL)
        {
            this.redis = redis;
            this.mySQL = mySQL;
            UserRepository = new UserRepository(redis, mySQL);
            SkillCardRepository = new SkillCardRepository(redis, mySQL);
            CoreRepository = new CoreRepository(redis, mySQL);
        }
        #endregion
    }
}
