using Material.Redis.Dao.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Redis.Dao
{
    public class FriendDao : IFriendDao
    {
        IDatabase db;
        public FriendDao(IDatabase db)
        {
            this.db = db;
        }
    }
}
