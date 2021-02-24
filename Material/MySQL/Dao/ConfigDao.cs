using Material.Entity;
using Material.Entity.Config;
using Material.MySQL.Dao.Interface;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Material.MySQL.Dao
{
    public class ConfigDao : IConfigDao
    {
        string ConnectionString;
        public ConfigDao(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        public MySqlConnection GetConnection(out MySqlConnection connection)
        {
            connection = new MySqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }


        public async Task<bool> Insert(UserServerConfig config)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "INSERT INTO user_server(category,skill_card_update,max_buff) VALUES(@category,@skill_card_update,@max_buff)";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@category", config.Category.ToString()));
                parameters.Add(new MySqlParameter("@skill_card_update", config.SkillCardUpdate));
                parameters.Add(new MySqlParameter("@max_buff", config.MaxBuff));
                int result = await MySqlHelper.ExecuteNonQueryAsync(connection, sqlcommand, parameters.ToArray());
                if (result == 1)
                {
                    return true;
                }
                else return false;
            }
            finally
            {
                connection.Close();
            }
        }
        public async Task<bool> Delete(UserServerConfig.UserServerCategory category)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "DELETE FROM user_server WHERE category=@category";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@category", category.ToString()));
                int result = await MySqlHelper.ExecuteNonQueryAsync(connection, sqlcommand, parameters.ToArray());
                if (result == 1)
                {
                    return true;
                }
                else return false;
            }
            finally
            {
                connection.Close();
            }
        }
        public async Task<bool> Update(UserServerConfig config)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "UPDATE user_server SET skill_card_update=@skill_card_update,max_buff=@max_buff WHERE category=@category";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@category", config.Category.ToString()));
                parameters.Add(new MySqlParameter("@skill_card_update", config.SkillCardUpdate));
                parameters.Add(new MySqlParameter("@max_buff", config.MaxBuff));
                int result = await MySqlHelper.ExecuteNonQueryAsync(connection, sqlcommand, parameters.ToArray());
                if (result == 1)
                {
                    return true;
                }
                else return false;
            }
            finally
            {
                connection.Close();
            }
        }
        public async Task<UserServerConfig> Query(UserServerConfig.UserServerCategory category)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "SELECT skill_card_update,max_buff FROM user_server WHERE category=@category";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@category", category.ToString()));

                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, sqlcommand, parameters.ToArray());
                if (reader.Read())
                {
                    UserServerConfig config = new UserServerConfig();
                    config.Category = category;
                    config.SkillCardUpdate = reader.GetInt64("skill_card_update");
                    config.MaxBuff = reader.GetInt32("max_buff");
                    return config;
                }
                else return null;
            }
            finally
            {
                connection.Close();
            }
        }
        public async Task<PlayerServerConfig> Query(PlayerServerConfig.PlayerServerCategory category)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "SELECT ip,port,secret_key FROM player_server WHERE category=@category";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@category", category.ToString()));

                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, sqlcommand, parameters.ToArray());
                if (reader.Read())
                {
                    PlayerServerConfig config = new PlayerServerConfig();
                    config.Category = category;
                    config.Ip = reader.GetString("ip");
                    config.Port = reader.GetString("port");
                    config.SecretKey = reader.GetString("secret_key");
                    return config;
                }
                else return null;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
