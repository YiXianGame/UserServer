using Material.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.Redis.Dao.Interface
{
    public interface ISkillCardDao
    {
        void Set(SkillCardBase skillCard);
        void Delete(long id);
        Task<SkillCardBase> Query(long id);
    }
}
