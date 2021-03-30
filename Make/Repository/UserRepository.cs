using Material.Entity;
using Material.ExceptionModel;
using Material.MySQL;
using Material.Redis;
using Material.Utils;
using System.Collections.Generic;
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
        public UserRepository(Redis redis, MySQL mySQL)
        {
            this.redis = redis;
            this.mySQL = mySQL;
        }
        public async Task<User> Login(long id, string username, string password)
        {
            // null 账户不存在 -1密码错误
            User user;
            if (id <= 0) user = await mySQL.userDao.Query_UserByUsername(username);
            else
            {
                user = await redis.userDao.Query_User(id, true);
                //Redis中不存在
                if (user == null)
                {
                    user = await Cache(id);
                    //缓存之后还是没有
                    if (user == null)
                    {
                        return null;//账户不存在
                    }
                }
            }

            if (!user.Password.Equals(password))
            {
                user.Id = -1;//密码错误
                return user;
            }
            else
            {
                return user;
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
                    User user = await mySQL.userDao.Query_UserByID(result);
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
            //这里要到了密码!!注意密码的去向，User内部的密码序列化已经忽略。
            User user = await mySQL.userDao.Query_UserByID(id, true);
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
            User user = await redis.userDao.Query_UserAttribute(id);
            if (user == null)//Redis不存在该用户
            {
                user = await Cache(id);//缓存该用户
            }
            if (user != null && user.Attribute_update != timestamp)//说明需要更新了
            {
                return user;
            }
            else return null;
        }
        public async Task<User> Sync_CardGroups(long id, long timestamp)
        {
            //先从Redis里面取更新信息    
            User user = await redis.userDao.Query_User(id);
            if (user == null)//Redis不存在该用户
            {
                user = await Cache(id);//缓存该用户
            }
            foreach(CardGroup cardGroup in user.CardGroups)
            {
                foreach(long skillCard in cardGroup.Cards.ToArray())
                {
                    if (!Core.SkillCards.ContainsKey(skillCard))
                    {
                        cardGroup.Cards.Remove(skillCard);
                        user.CardGroups_update = TimeStamp.Now();
                    }
                }
            }
            if (user.CardGroups_update != timestamp)//说明需要更新了
            {
                return user;
            }
            else return null;
        }

        public async Task<List<Friend>> Sync_Friend(long id, long timestamp)
        {
            //先从Redis里面取更新信息    
            User user = await redis.userDao.Query_User(id);
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
            bool result = await mySQL.userDao.Update_CardGroups(id, user.CardGroups, timestamp);
            if (result)
            {
                redis.userDao.SetCardGroups(id, user.CardGroups, timestamp);
                return timestamp;
            }
            else return -1;
        }
        public async Task<bool> Update_CardRepositoryUpdate(long id,long timestamp)
        {
            return await mySQL.userDao.Update_CardRepositoryUpdate(id,timestamp);
        }
        public async Task<long> Update_State(long id, User.UserState state)
        {
            long timestamp = TimeStamp.Now();
            bool result = await mySQL.userDao.Update_State(id, state, timestamp);
            if (result)
            {
                redis.userDao.SetState(id, state, timestamp);
                return timestamp;
            }
            else return -1;
        }
        public async Task<User> Query_AttributeById(long id)
        {
            User user = await redis.userDao.Query_User(id);
            if (user == null)
            {
                user = await Cache(id);
            }
            return user;
        }
        public async Task<long> Query_CardRepositoryUpdateById(long id)
        {
            long db_timestamp = await redis.userDao.Query_CardRepositoryUpdate(id);
            if (db_timestamp == -1)
            {
                User user = await Cache(id);
                if (user == null) throw new UserException(UserException.ErrorCode.DataNotFound, "处理同步用户卡牌数据时，Mysql无法查到数据.");
                db_timestamp = user.CardRepository_update;
            }
            return db_timestamp;
        }
        public async Task<long> Query_FriendUpdateById(long id)
        {
            long db_timestamp = await redis.userDao.Query_FriendUpdate(id);
            if (db_timestamp == -1)
            {
                User user = await Cache(id);
                if (user == null) throw new UserException(UserException.ErrorCode.DataNotFound, "处理同步用户好友数据时，Mysql无法查到数据.");
                db_timestamp = user.Friend_update;
            }
            return db_timestamp;
        }
        public async Task<List<CardItem>> Sync_CardRepository(long id, long timestamp)
        {
            long db_timestamp = await redis.userDao.Query_CardRepositoryUpdate(id);
            if (db_timestamp == -1)
            {
                User user = await Cache(id);
                if (user == null) throw new UserException(UserException.ErrorCode.DataNotFound, "处理同步用户卡牌数据时，Mysql无法查到数据.");
                db_timestamp = user.CardRepository_update;
            }
            if (db_timestamp != timestamp)
            {
                return await mySQL.cardRepositoryDao.QueryByUserId(id);
            }
            else return null;
        }
        public async Task<List<CardItem>> Query_CardRepositoryById(long id)
        {
            return await mySQL.cardRepositoryDao.QueryByUserId(id);
        }
        public async Task<bool> Insert_CardRepository(CardItem item)
        {
            return await mySQL.cardRepositoryDao.Insert(item);
        }
        #endregion
    }

}
