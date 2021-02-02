using Material.MySQL.Dao.Interface;
using MySql.Data.MySqlClient;
using StackExchange.Redis;

namespace Material.MySQL.Dao
{
    public class CardRepositoryDao : ICardRepositoryDao
    {
        MySqlConnection connection;
        public CardRepositoryDao(MySqlConnection connection)
        {
            this.connection = connection;
        }
    }
}
