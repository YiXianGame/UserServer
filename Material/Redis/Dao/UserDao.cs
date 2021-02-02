using Material.Redis.Dao.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.Redis.Dao
{
    public class UserDao : IUserDao
    {
        IDatabase db;

        public UserDao(IDatabase db)
        {
            this.db = db;
            
        }

        public void SetUserAccount(string username, string password, long id)
        {
            db.HashSetAsync("UserAccount" + username,new HashEntry[]{ new HashEntry("password",password),new HashEntry("id",id)});
        }

        public async Task<long> ValidUser(string username, string password)
        {
            RedisValue[] values = await db.HashGetAsync("UserAccount", new RedisValue[] { "password", "id" });
            if(values[0] != "(nil)" && values[1] != "(nil)")
            {
                if (password.Equals(values[0]))
                {
                    return (long)values[1];
                }
                else return -2;
            }
            return -1;
        }
    }
}
