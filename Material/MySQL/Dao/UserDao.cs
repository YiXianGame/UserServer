using Material.Entity;
using Material.MySQL.Dao.Interface;
using MySql.Data.MySqlClient;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Material.MySQL.Dao
{
    public class UserDao : IUserDao
    {
        string ConnectionString;
        public UserDao(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        public MySqlConnection GetConnection(out MySqlConnection connection)
        {
            connection = new MySqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
        public async Task<bool> Insert_Person(string username, string nickname, string password)
        {
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1),TimeZoneInfo.Local); // 当地时区
            long timeStamp = (long)(DateTime.Now - startTime).TotalSeconds; // 相差秒数
            int result = await MySqlHelper.ExecuteNonQueryAsync(GetConnection(out MySqlConnection connection),$"INSERT INTO users(username,nickname,password,register_date,attribute_update,skill_card_update,head_image_update) VALUES ('{username}','{nickname}','{password}','{timeStamp}','{timeStamp}','{timeStamp}','{timeStamp}')");
            if (result == 1) return true;
            else return false;
        }

        public async Task<MySqlDataReader> Query_UserByUsername(string username)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                MySqlDataReader result = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT id,username,nickname,upgrade_num,create_num,money,personal_signature," +
                    $"battle_count,exp,lv,title,active,kills,deaths,register_date,attribute_update,skillCard_update,headImage_update FROM users WHERE username='{username}'");
                return result;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<MySqlDataReader> Query_UserByID(long id)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                MySqlDataReader result = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT id,username,nickname,upgrade_num,create_num,money,personal_signature," +
                    $"battle_count,exp,lv,title,active,kills,deaths,register_date,attribute_update,skillCard_update,headImage_update FROM users WHERE id='{id}'");
                return result;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<bool> Update_NickName(long id,string nickname)
        {
            int result = await MySqlHelper.ExecuteNonQueryAsync(GetConnection(out MySqlConnection connection), $"UPDATE users SET nickname='{nickname}' WHERE id='{id}'");
            if (result == 1) return true;
            else return false;
        }

        public async Task<bool> Update_Password(long id, string password)
        {
            int result = await MySqlHelper.ExecuteNonQueryAsync(GetConnection(out MySqlConnection connection), $"UPDATE users SET password='{password}' WHERE id='{id}'");
            if (result == 1) return true;
            else return false;
        }
        public async Task<UserBase> ValidUser(long id, string password)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                MySqlDataReader result = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT password,id,attribute_update,skillCard_update,headImage_update FROM users WHERE id='{id}'");
                UserBase user = new UserBase();
                if (result.Read())
                {
                    if (password.Equals(result.GetString(0)))
                    {
                        user.id = result.GetInt64(1);
                        user.attribute_update = result.GetInt64("attribute_update");
                        user.skillCard_update = result.GetInt64("skill_card_update");
                        user.headImage_update = result.GetInt64("head_image_update");
                    }
                    else user.id = -2;
                }
                else user.id = -1;
                return user;
            }
            finally
            {
                connection.Close();
            } 
        }
        public async Task<long> ValidUser(string username, string password)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                MySqlDataReader result = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT id,password FROM users WHERE username='{username}'");
                if (result.Read())
                {
                    if (password.Equals(result.GetString(1)))
                    {
                        return result.GetInt64(0);
                    }
                    else return -2;
                }
                else return -1;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
