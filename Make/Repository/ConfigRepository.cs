using Material.Entity;
using Material.Entity.Config;
using Material.MySQL;
using Material.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Make.Repository
{
    public class ConfigRepository
    {
        #region --字段--
        private Redis redis;
        private MySQL mySQL;
        #endregion

        #region --方法--
        public ConfigRepository(Redis redis, MySQL mySQL)
        {
            this.redis = redis;
            this.mySQL = mySQL;
        }
        public async Task<bool> Insert(UserServerConfig config)
        {
            return await mySQL.configDao.Insert(config);
        }
        public async Task<bool> Update(UserServerConfig config)
        {
            return await mySQL.configDao.Update(config);
        }
        public async Task<UserServerConfig> Query(UserServerConfig.UserServerCategory category)
        {
            return await mySQL.configDao.Query(category);
        }
        public async Task<bool> Delete(UserServerConfig.UserServerCategory category)
        {
            return await mySQL.configDao.Delete(category);
        }
        #endregion

    }
}
