using Material.Entity;
using Material.ExceptionModel;
using Material.MySQL;
using Material.Redis;
using Material.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Make.Repository
{
    public class UserRepository
    {
        #region --字段--
        private Redis redis;
        private MySQL mySQL;
        #endregion

        #region --方法--
        public UserRepository(Redis redis,MySQL mySQL)
        {
            this.redis = redis;
            this.mySQL = mySQL;
        }
        public async Task<long> Login(long id, string username, string password)
        {
            //-1账户不存在 -2密码错误
            //有ID数据
            if (id != 0)
            {
                //在Redis中验证一下密码的正确性
                long result = await redis.userDao.ValidPerson(id, password);
                //Redis中不存在
                if (result == -1)
                {
                    //在Mysql中用ID查询验证一下密码
                    User user = await mySQL.userDao.Query_AttributeByID(id, true);
                    if (user == null) return -1;//账户不存在
                    if (user.Password.Equals(password))
                    {
                        //将账户信息存至Redis
                        redis.userDao.SetAccount(user);
                        return user.Id;//返回验证值
                    }
                    else return -2;//账户密码错误
                }
                else return result;//账户存在,返回验证值
            }
            else //首次登录，无ID数据
            {
                return await mySQL.userDao.Valid(username, password);
            }
        }
        public async Task<long> Register(string username, string nickname, string password)
        {
            long id = await mySQL.userDao.Query_IdByUsername(username);
            if (id == -1)
            {
                long result = await mySQL.userDao.Insert(username, nickname, password);
                if (result != -1)
                {
                    User user = await mySQL.userDao.Query_AttributeByID(result);
                    redis.userDao.SetAccount(user);
                    return user.Id;
                }
                else return -1;
            }
            else return -2;
        }
        //从Mysql缓存到Redis
        public async Task<User> Cache(long id)
        {
            //这里要到了密码，用来同步，但是切记要及时置null
            User user = await mySQL.userDao.Query_AttributeByID(id, true);
            if (user != null)//Mysql中有此用户的数据
            {
                redis.userDao.SetAccount(user);
                return user;
            }
            else return null;
        }
        public async Task<User> Sync_Attribute(long id, long timestamp)
        {
            //先从Redis里面取更新信息    
            User user = await redis.userDao.Query_Attribute(id);
            if (user == null)//Redis不存在该用户
            {
                user = await Cache(id);//缓存该用户
            }
            if (user.Attribute_update != timestamp)//说明需要更新了
            {
                return user;
            }
            else return null;
        }
        public async Task<List<Friend>> Sync_Friend(long id, long timestamp)
        {
            //先从Redis里面取更新信息    
            User user = await redis.userDao.Query_Attribute(id);
            if (user == null)//Redis不存在该用户
            {
                user = await Cache(id);//缓存该用户
            }
            if (user.Friend_update != timestamp)//说明需要更新了
            {
                return await mySQL.friendDao.QueryAllById(id);
            }
            else return null;
        }
        public async Task<long> Update_CardGroups(long id, User user)
        {
            long timestamp = TimeStamp.Now();
            bool result = await mySQL.userDao.Update_CardGroups(id, user.CardGroups,timestamp);
            if (result)
            {
                redis.userDao.SetCardGroups(id, user.CardGroups, timestamp);
                return timestamp;
            }
            else return -1;

        }
        public async Task<User> Query_AttributeById(long id)
        {
            User user = await redis.userDao.Query_Attribute(id);
            if (user == null)
            {
                user = await Cache(id);
            }
            return user;
        }
        public async Task<long> Query_SkillCardUpdateById(long id)
        {
            long db_timestamp = await redis.userDao.Query_SkillCardUpdate(id);
            if (db_timestamp == -1)
            {
                User user = await Cache(id);
                if (user == null) throw new UserException(UserException.ErrorCode.DataNotFound, "处理同步用户卡牌数据时，Mysql无法查到数据.");
                db_timestamp = user.SkillCard_update;
            }
            return db_timestamp;
        }
        public async Task<long> Query_FriendUpdateById(long id)
        {
            long db_timestamp = await redis.userDao.Query_SkillCardUpdate(id);
            if (db_timestamp == -1)
            {
                User user = await Cache(id);
                if (user == null) throw new UserException(UserException.ErrorCode.DataNotFound, "处理同步用户好友数据时，Mysql无法查到数据.");
                db_timestamp = user.Friend_update;
            }
            return db_timestamp;
        }
        public async Task<List<CardItem>> Sync_UserSkillCards(long id,long timestamp)
        {
            long db_timestamp = await redis.userDao.Query_SkillCardUpdate(id);
            if (db_timestamp == -1)
            {
                User user = await Cache(id);
                if (user == null) throw new UserException( UserException.ErrorCode.DataNotFound,"处理同步用户卡牌数据时，Mysql无法查到数据.");
                db_timestamp = user.SkillCard_update;
            }
            if (db_timestamp != timestamp)
            {
                return await mySQL.cardRepositoryDao.QueryByUserId(id);
            }
            else return null;
        }
        #endregion
    }

}
