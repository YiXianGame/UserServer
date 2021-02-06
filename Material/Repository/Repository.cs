using System;
using System.Threading.Tasks;
using Material.Entity;
using MySql.Data.MySqlClient;

namespace Material.Repository
{

    public class Repository
    {
        Redis.Redis redis;
        MySQL.MySQL mySQL;
        public Repository(Redis.Redis redis, MySQL.MySQL mySQL)
        {
            this.redis = redis;
            this.mySQL = mySQL;
        }
        public async Task<long> LoginUser(long id,string username,string password)
        {
            //-1账户不存在 -2密码错误
            //有ID数据
            if(id != 0)
            {
                //在Redis中验证一下密码的正确性
                long result = await redis.userDao.ValidPerson(id, password);
                //Redis中不存在
                if (result == -1)
                {
                    //在Mysql中用ID查询验证一下密码
                    UserBase user = await mySQL.userDao.Query_UserAttributeByID(id,true);
                    if (user == null) return -1;//账户不存在
                    if (user.password.Equals(password))
                    {
                        //将账户信息存至Redis
                        redis.userDao.SetAccount(user);
                        return user.id;//返回验证值
                    }
                    else return -2;//账户密码错误
                }
                else return result;//账户存在,返回验证值
            }
            else //首次登录，无ID数据
            {
                return await mySQL.userDao.ValidUser(username, password);
            }
        }
        public async Task<long> Register(string username,string nickname,string password)
        {
            long id = await mySQL.userDao.Query_IdByUsername(username);
            if (id == -1)
            {
                bool result = await mySQL.userDao.Insert_User(username, nickname, password);
                if (result)
                {
                    UserBase user = await mySQL.userDao.Query_UserAttributeByUsername(username);
                    redis.userDao.SetAccount(user);
                    return user.id;
                }
                else return -1;
            }
            else return -2;
        }
        //从Mysql缓存到Redis
        public async Task<UserBase> CacheUser(long id)
        {
            //这里要到了密码，用来同步，但是切记要及时置null
            UserBase user = await mySQL.userDao.Query_UserAttributeByID(id, true);
            if (user != null)//Mysql中有此用户的数据
            {
                redis.userDao.SetAccount(user);
                return user;
            }
            else return null;
        }
        public async Task<UserBase> Sync_UserAttribute(long id,long timestamp)
        {
            //先从Redis里面取更新信息    
            UserBase user = await redis.userDao.Query_UserAttribute(id);
            if (user == null)//Redis不存在该用户
            {
                user = await CacheUser(id);//缓存该用户
            }
            if(user.attribute_update != timestamp)//说明需要更新了
            {
                return user;
            }
            else return null;
        }
        public async Task<UserBase> Query_UserAttributeById(long id)
        {
            UserBase user = await redis.userDao.Query_UserAttribute(id);
            if(user == null)
            {
                user = await CacheUser(id);
            }
            return user;
        }
    }
}
