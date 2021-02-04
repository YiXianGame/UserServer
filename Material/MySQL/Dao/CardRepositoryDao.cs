using Material.MySQL.Dao.Interface;
using MySql.Data.MySqlClient;
using StackExchange.Redis;

namespace Material.MySQL.Dao
{
    public class CardRepositoryDao : ICardRepositoryDao
    {
        string ConnectionString;
        public CardRepositoryDao(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
