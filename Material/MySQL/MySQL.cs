using Material.MySQL.Dao;
using Material.MySQL.Dao.Interface;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.MySQL
{
    public class MySQL
    {
        public string dbConnStr;
        private const int MaxPool = 100;//最大连接数
        private const int MinPool = 5;//最小连接数
        private const bool Asyn_Process = true;//设置异步访问数据库
        private const int Conn_Timeout = 15;//设置连接等待时间
        private const int Conn_Lifetime = 30;//设置连接的生命周期
        public IUserDao userDao;
        public ICardRepositoryDao cardRepositoryDao;
        public IFriendDao friendDao;
        private string GetConnString(string ip, string port, string db, string username, string password)
        {
            return $"server={ip};"
            + $"port={port};"
            + $"uid={username};"
            + $"pwd={password};"
            + $"database={db};"
            + $"MinimumPoolSize=10";
        }
        public MySQL(string ip,string port,string db,string username,string password)
        {
            dbConnStr = GetConnString(ip, port, db,username,password);
            userDao = new UserDao(dbConnStr);
            cardRepositoryDao = new CardRepositoryDao(dbConnStr);
            friendDao = new FriendDao(dbConnStr);
        }

    }
}
