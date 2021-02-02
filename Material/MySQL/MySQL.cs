using Material.MySQL.Dao;
using Material.MySQL.Dao.Interface;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Material.MySQL
{
    public class MySQL
    {
        public string dbConnStr;
        private MySqlConnection dbConn = null;
        private MySqlCommand dbCmd = null;
        private MySqlDataReader dbDataReader = null;
        private const int MaxPool = 100;//最大连接数
        private const int MinPool = 5;//最小连接数
        private const bool Asyn_Process = true;//设置异步访问数据库
        private const int Conn_Timeout = 15;//设置连接等待时间
        private const int Conn_Lifetime = 30;//设置连接的生命周期
        public IUserDao userDao;
        public ICardRepositoryDao cardRepositoryDao;
        public IFriendDao friendDao;
        private string GetConnString(string ip, string db, string username, string password)
        {
            return $"server={ip};"
            + $"uid={username};"
            + $"pwd={password};"
            + $"database={db};";
        }
        public MySQL(string ip,string db,string username,string password)
        {
            
            dbConnStr = GetConnString(ip,db,username,password);
            dbConn = new MySqlConnection(dbConnStr);
            dbCmd = new MySqlCommand();
            dbCmd.Connection = dbConn;
            userDao = new UserDao(dbConn);
            cardRepositoryDao = new CardRepositoryDao(dbConn);
            friendDao = new FriendDao(dbConn);
        }
    }
}
