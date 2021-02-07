using Material.Entity;
using Material.ExceptionModel;
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
        public async Task<long> Insert(string username, string nickname, string password)
        {
            GetConnection(out MySqlConnection connection);
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local); // 当地时区
            long timeStamp = (long)(DateTime.Now - startTime).TotalSeconds; // 相差秒数
            try
            {
                int result = await MySqlHelper.ExecuteNonQueryAsync(connection, $"INSERT INTO users(username,nickname,password,register_date,attribute_update,skill_card_update,head_image_update,active) VALUES ('{username}','{nickname}','{password}','{timeStamp}','{timeStamp}','{timeStamp}','{timeStamp}','{UserBase.State.Offline}')");
                if (result == 1)
                {
                    return await Query_LastInsertId(connection);
                }
                else return -1;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<UserBase> Query_AttributeByUsername(string username)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT id,nickname,password,upgrade_num,create_num,money,personal_signature," +
                    $"battle_count,exp,lv,title,active,kills,deaths,register_date,attribute_update,skill_card_update,head_image_update FROM users WHERE username='{username}'");
                UserBase user = null;
                if (reader.Read())
                {
                    user = new UserBase();
                    user.Id = reader.GetInt64("id");
                    user.Username= username;
                    user.Nickname = reader.GetString("nickname");
                    user.Password = reader.GetString("password");
                    user.Upgrade_num = reader.GetInt32("upgrade_num");
                    user.Create_num = reader.GetInt32("create_num");
                    user.Money = reader.GetInt32("money");
                    user.PersonalSignature= reader.GetString("personal_signature");
                    user.BattleCount = reader.GetInt32("battle_count");
                    user.Exp = reader.GetInt64("exp");
                    user.Lv  = reader.GetInt32("lv");
                    user.Title = reader.GetString("title");
                    user.Active = (UserBase.State)Enum.Parse(typeof(UserBase.State), reader.GetString("active"));
                    user.Kills = reader.GetInt32("kills");
                    user.Deaths = reader.GetInt32("deaths");
                    user.RegisterDate= reader.GetInt64("register_date");
                    user.Attribute_update= reader.GetInt64("attribute_update");
                    user.SkillCard_update = reader.GetInt64("skill_card_update");
                    user.HeadImage_update = reader.GetInt64("head_image_update");
                }
                return user;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<UserBase> Query_AttributeByID(long id,bool has_password = false)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT username,nickname,password,upgrade_num,create_num,money,personal_signature," +
                    $"battle_count,exp,lv,title,active,kills,deaths,register_date,attribute_update,skill_card_update,head_image_update FROM users WHERE id={id}");
                UserBase user = null;
                if (reader.Read())
                {
                    user = new UserBase();
                    user.Id = id;
                    user.Username = reader.GetString("username");
                    user.Nickname = reader.GetString("nickname");
                    user.Password = reader.GetString("password");
                    user.Upgrade_num = reader.GetInt32("upgrade_num");
                    user.Create_num = reader.GetInt32("create_num");
                    user.Money = reader.GetInt32("money");
                    user.PersonalSignature = reader.GetString("personal_signature");
                    user.BattleCount = reader.GetInt32("battle_count");
                    user.Exp = reader.GetInt64("exp");
                    user.Lv = reader.GetInt32("lv");
                    user.Title = reader.GetString("title");
                    user.Active = (UserBase.State)Enum.Parse(typeof(UserBase.State), reader.GetString("active"));
                    user.Kills = reader.GetInt32("kills");
                    user.Deaths = reader.GetInt32("deaths");
                    user.RegisterDate = reader.GetInt64("register_date");
                    user.Attribute_update = reader.GetInt64("attribute_update");
                    user.SkillCard_update = reader.GetInt64("skill_card_update");
                    user.HeadImage_update = reader.GetInt64("head_image_update");
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

        public async Task<long> Valid(string username, string password)
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

        public async Task<long> Query_IdByUsername(string username)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                MySqlDataReader result = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT id FROM users WHERE username='{username}'");
                if (result.Read())
                {
                    return result.GetInt64(0);
                }
                else return -1;
            }
            finally
            {
                connection.Close();
            }
        }
        private async Task<long> Query_LastInsertId(MySqlConnection connection)
        {
            try
            {
                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT LAST_INSERT_ID()");
                if (reader.Read())
                {
                    return reader.GetInt64(0);
                }
                throw new UserException(UserException.ErrorCode.NotFoundLastIndex, "找不到自增主键的ID值");
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
