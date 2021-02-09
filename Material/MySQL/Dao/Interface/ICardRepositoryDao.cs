using Material.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.MySQL.Dao.Interface
{
    public interface ICardRepositoryDao
    {
        Task<bool> Insert(CardRepository item);
        Task<bool> Update(CardRepository item);
        Task<bool> Delete(long owner_id,long item_id);
        Task<CardRepository> Query(long owner_id,long item_id);
    }
}
