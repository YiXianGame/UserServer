using Material.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.Redis.Dao.Interface
{
    public interface ISkillCardDao
    {
        void Set(SkillCard skillCard);
        void Delete(long id);
        Task<SkillCard> Query(long id);
    }
}
