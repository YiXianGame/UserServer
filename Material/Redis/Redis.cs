﻿using Material.Redis.Dao;
using Material.Redis.Dao.Interface;
using StackExchange.Redis;

namespace Material.Redis
{
    public class Redis
    {
        private ConnectionMultiplexer redis;//连接到redis
        public IUserDao userDao;
        public ICardRepositoryDao cardRepositoryDao;
        public IFriendDao friendDao;
        public IDatabase db;
        public Redis(string config)
        {
            redis = ConnectionMultiplexer.Connect(config);
            //0-用户库
            userDao = new UserDao(redis.GetDatabase(0));
            //2-仓库
            cardRepositoryDao = new CardRepositoryDao(redis.GetDatabase(2));
            //2-好友
            friendDao = new FriendDao(redis.GetDatabase(3));
        }
    }
}
