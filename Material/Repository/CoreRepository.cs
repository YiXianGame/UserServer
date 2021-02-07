using Material.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.Repository
{
    public class CoreRepository
    {
        #region --字段--
        private Redis.Redis redis;
        private MySQL.MySQL mySQL;
        #endregion

        #region --方法--
        public CoreRepository(Redis.Redis redis, MySQL.MySQL mySQL)
        {
            this.redis = redis;
            this.mySQL = mySQL;
        }
        public async Task<bool> Insert(CoreBase core)
        {
            return await mySQL.coreDao.Insert(core);
        }
        public async Task<bool> Update(CoreBase core)
        {
            return await mySQL.coreDao.Update(core);
        }
        public async Task<CoreBase> Query(CoreBase.ConfigCategory category)
        {
            return await mySQL.coreDao.Query(category);
        }
        public async Task<bool> Delete(CoreBase.ConfigCategory category)
        {
            return await mySQL.coreDao.Delete(category);
        }
        #endregion

    }
}
