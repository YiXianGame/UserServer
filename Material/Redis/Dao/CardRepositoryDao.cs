using Material.Entity;
using Material.Redis.Dao.Interface;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Material.Redis.Dao
{
    public class CardRepositoryDao : ICardRepositoryDao
    {
        IDatabase db;
        public CardRepositoryDao(IDatabase db)
        {
            this.db = db;
        }

        public void Delete(long id)
        {
            throw new System.NotImplementedException();
        }

        public Task<CardItem> Query(long id)
        {
            throw new System.NotImplementedException();
        }

        public void Set(CardItem cardRepository)
        {
            throw new System.NotImplementedException();
        }
    }
}
