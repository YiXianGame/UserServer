using Material.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.MySQL.Dao.Interface
{
    public interface ISkillCArdDao
    {
        Task<long> Insert(SkillCardBase skillCard);
        Task<bool> Update(SkillCardBase skillCard);
        Task<bool> Delete(long id);
        Task<SkillCardBase> Query(long id);
    }
}
