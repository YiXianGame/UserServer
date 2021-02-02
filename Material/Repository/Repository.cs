using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.Repository
{

    public class Repository
    {
        Redis.Redis redis;
        MySQL.MySQL mySQL;
        public Repository(Redis.Redis redis,MySQL.MySQL mySQL)
        {
            this.redis = redis;
            this.mySQL = mySQL;
        }
        public async Task<long> ValidUser(string username, string password)
        {
            long id = await redis.userDao.ValidUser(username, password);
            if(id == -1)
            {
                id = await mySQL.userDao.ValidUser(username, password);
                if (id != -1 && id != -2)
                {
                    redis.userDao.SetUserAccount(username, password, id);
                    return id;
                }
            }
            return id;
        }
        public async Task<long> Register(string username,string password,string nickname)
        {
            long id = await redis.userDao.ValidUser(username, password);
            if(id == -1)
            {
                bool result = await mySQL.userDao.Insert_User(username, nickname, password);
                if (result)
                {
                    id = await mySQL.userDao.Query_IdByUsername(username);
                    redis.userDao.SetUserAccount(username, password, id);
                    return id;
                }
            }
            return -1;
        }
    }
}
