using Material.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.Repository
{
    public class SkillCardRepository
    {
        #region --字段--
        private Redis.Redis redis;
        private MySQL.MySQL mySQL;
        #endregion

        #region --方法--
        public SkillCardRepository(Redis.Redis redis, MySQL.MySQL mySQL)
        {
            this.redis = redis;
            this.mySQL = mySQL;
        }
        public async Task<long> Insert(SkillCardBase skillCard)
        {
            long result = await mySQL.skillCardDao.Insert(skillCard);
            if (result != 0)
            {
                skillCard.Id = result;
                redis.skillCardDao.Set(skillCard);
                return result;
            }
            else return -1;
        }
        public async Task<bool> Update(SkillCardBase skillCard)
        {
            bool result = await mySQL.skillCardDao.Update(skillCard);
            if (result)
            {
                redis.skillCardDao.Set(skillCard);
                return true;
            }
            else return false;
        }
        public async Task<SkillCardBase> Query(long id)
        {
            SkillCardBase skillCard = await redis.skillCardDao.Query(id);
            if (skillCard == null)
            {
                skillCard = await mySQL.skillCardDao.Query(id);
                if (skillCard != null)
                {
                    redis.skillCardDao.Set(skillCard);
                    return skillCard;
                }
                else return null;
            }
            else return skillCard;
        }
        public async Task<bool> Delete(long id)
        {
            bool result = await mySQL.skillCardDao.Delete(id);
            if (result)
            {
                redis.skillCardDao.Delete(id);
                return true;
            }
            else return false;
        }
        #endregion
    }
}
