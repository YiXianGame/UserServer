using Material.MySQL.Dao;
using Material.MySQL.Dao.Interface;

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
        public IConfigDao coreDao;
        public IUserDao userDao;
        public ISkillCardDao skillCardDao;
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
            skillCardDao = new SkillCardDao(dbConnStr);
            cardRepositoryDao = new CardRepositoryDao(dbConnStr);
            coreDao = new ConfigDao(dbConnStr);
            friendDao = new FriendDao(dbConnStr);
        }
    }
}
