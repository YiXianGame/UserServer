using Material.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.MySQL.Dao.Interface
{
    public interface ICardRepositoryDao
    {
        Task<bool> Insert(CardItem item);
        Task<bool> Update(CardItem item);
        Task<bool> Delete(long owner_id,long item_id);
        Task<CardItem> Query(long owner_id,long item_id);
        Task<List<CardItem>> QueryByUserId(long owner_id);
    }
}
