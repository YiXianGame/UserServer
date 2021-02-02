using Material.MySQL.Dao.Interface;
using MySql.Data.MySqlClient;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Material.MySQL.Dao
{
    public class UserDao : IUserDao
    {
        MySqlConnection connection;
        public UserDao(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task<bool> Insert_User(string username, string nickname, string password)
        {
            int result = await MySqlHelper.ExecuteNonQueryAsync(connection,$"INSERT INTO users(username,nickname,password) VALUES ('{username}','{nickname}','{password}')");
            if (result == 1) return true;
            else return false;
        }

        public async Task<long> Query_IdByUsername(string username)
        {
            MySqlDataReader result = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT id FROM users WHERE username='{username}'");
            if (result.Read()) return result.GetInt64(0);
            else return -1;
        }

        public async Task<bool> Update_NickName(long id,string nickname)
        {
            int result = await MySqlHelper.ExecuteNonQueryAsync(connection, $"UPDATE users SET nickname='{nickname}' WHERE id='{id}'");
            if (result == 1) return true;
            else return false;
        }

        public async Task<bool> Update_Password(long id, string password)
        {
            int result = await MySqlHelper.ExecuteNonQueryAsync(connection, $"UPDATE users SET password='{password}' WHERE id='{id}'");
            if (result == 1) return true;
            else return false;
        }
        public async Task<long> ValidUser(string username, string password)
        {
            MySqlDataReader result = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT password,id FROM users WHERE username='{username}'");
            if (result.Read())
            {
                if (password.Equals(result.GetString(0)))
                {
                    return result.GetInt64(1);
                }
                else return -2;
            }
            else return -1;
        }
    }
}
