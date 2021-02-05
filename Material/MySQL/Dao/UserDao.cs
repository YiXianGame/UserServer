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
        public async Task<bool> Insert_User(string username, string nickname, string password)
        {
            GetConnection(out MySqlConnection connection);
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local); // 当地时区
            long timeStamp = (long)(DateTime.Now - startTime).TotalSeconds; // 相差秒数
            try
            {
                int result = await MySqlHelper.ExecuteNonQueryAsync(connection, $"INSERT INTO users(username,nickname,password,register_date,attribute_update,skill_card_update,head_image_update,active) VALUES ('{username}','{nickname}','{password}','{timeStamp}','{timeStamp}','{timeStamp}','{timeStamp}','{UserBase.State.Offline}')");
                if (result == 1) return true;
                else return false;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<UserBase> Query_UserAttributeByUsername(string username)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT id,username,nickname,upgrade_num,create_num,money,personal_signature," +
                    $"battle_count,exp,lv,title,active,kills,deaths,register_date,attribute_update,skill_card_update,head_image_update FROM users WHERE username='{username}'");
                UserBase user = null;
                if (reader.Read())
                {
                    user = new UserBase();
                    user.id = reader.GetInt64("id");
                    user.username = reader.GetString("username");
                    user.nickname = reader.GetString("nickname");
                    user.upgrade_num = reader.GetInt32("upgrade_num");
                    user.create_num = reader.GetInt32("create_num");
                    user.money = reader.GetInt32("money");
                    user.personalSignature = reader.GetString("personal_signature");
                    user.battleCount = reader.GetInt32("battle_count");
                    user.exp = reader.GetInt64("exp");
                    user.lv = reader.GetInt32("lv");
                    user.title = reader.GetString("title");
                    user.active = (UserBase.State)Enum.Parse(typeof(UserBase.State), reader.GetString("active"));
                    user.kills = reader.GetInt32("kills");
                    user.deaths = reader.GetInt32("deaths");
                    user.registerDate = reader.GetInt64("register_date");
                    user.attribute_update = reader.GetInt64("attribute_update");
                    user.skillCard_update = reader.GetInt64("skill_card_update");
                    user.headImage_update = reader.GetInt64("head_image_update");
                }
                return user;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<UserBase> Query_UserAttributeByID(long id,bool has_password = false)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT id,username,nickname,password,upgrade_num,create_num,money,personal_signature," +
                    $"battle_count,exp,lv,title,active,kills,deaths,register_date,attribute_update,skill_card_update,head_image_update FROM users WHERE id='{id}'");
                UserBase user = null;
                if (reader.Read())
                {
                    user = new UserBase();
                    user.id = reader.GetInt64("id");
                    user.username = reader.GetString("username");
                    user.nickname = reader.GetString("nickname");
                    if(has_password)user.password = reader.GetString("password");
                    user.upgrade_num = reader.GetInt32("upgrade_num");
                    user.create_num = reader.GetInt32("create_num");
                    user.money = reader.GetInt32("money");
                    user.personalSignature = reader.GetString("personal_signature");
                    user.battleCount = reader.GetInt32("battle_count");
                    user.exp = reader.GetInt64("exp");
                    user.lv = reader.GetInt32("lv");
                    user.title = reader.GetString("title");
                    user.active = (UserBase.State)Enum.Parse(typeof(UserBase.State), reader.GetString("active"));
                    user.kills = reader.GetInt32("kills");
                    user.deaths = reader.GetInt32("deaths");
                    user.registerDate = reader.GetInt64("register_date");
                    user.attribute_update = reader.GetInt64("attribute_update");
                    user.skillCard_update = reader.GetInt64("skill_card_update");
                    user.headImage_update = reader.GetInt64("head_image_update");
                }
                return user;
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
                MySqlDataReader result = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT password,id,attribute_update,skill_card_update,head_image_update FROM users WHERE id='{id}'");
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
