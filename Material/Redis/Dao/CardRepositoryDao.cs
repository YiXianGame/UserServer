using Material.Redis.Dao.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Redis.Dao
{
    public class CardRepositoryDao : ICardRepositoryDao
    {
        IDatabase db;
        public CardRepositoryDao(IDatabase db)
        {
            this.db = db;
        }
    }
}
