using Material.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.MySQL.Dao.Interface
{
    public interface ICardRepositoryDao
    {
        Task<bool> Insert(CardRepositoryBase item);
        Task<bool> Update(CardRepositoryBase item);
        Task<bool> Delete(long owner_id,long item_id);
        Task<CardRepositoryBase> Query(long owner_id,long item_id);
    }
}
