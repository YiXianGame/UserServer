using Material.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.Redis.Dao.Interface
{
    public interface ICardRepositoryDao
    {
        void Set(CardItem cardRepository);
        void Delete(long id);
        Task<CardItem> Query(long id);
    }
}
