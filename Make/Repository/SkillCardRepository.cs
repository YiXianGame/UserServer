using Material.Entity;
using Material.MySQL;
using Material.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Make.Repository
{
    public class SkillCardRepository
    {
        #region --字段--
        private MySQL mySQL;
        #endregion

        #region --方法--
        public SkillCardRepository(Redis redis, MySQL mySQL)
        {
            this.mySQL = mySQL;
        }
        public async Task<long> Insert(SkillCard skillCard)
        {
            return await mySQL.skillCardDao.Insert(skillCard);
        }
        public async Task<bool> Update(SkillCard skillCard)
        {
            return await mySQL.skillCardDao.Update(skillCard);
        }
        public async Task<SkillCard> Query(long id)
        {
            return await mySQL.skillCardDao.Query(id);
        }
        public SkillCard QuerySync(long id)
        {
            return mySQL.skillCardDao.QuerySync(id);
        }
        public async Task<List<SkillCard>> Query_All()
        {
            return await mySQL.skillCardDao.Query_All();
        }
        public async Task<bool> Delete(long id)
        {
            return await mySQL.skillCardDao.Delete(id);
        }
        #endregion
    }
}
