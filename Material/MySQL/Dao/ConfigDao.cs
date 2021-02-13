using Material.Entity;
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


        public async Task<bool> Insert(Config config)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "INSERT INTO config(category,skill_card_update,max_buff) VALUES(@category,@skill_card_update,@max_buff)";
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
        public async Task<bool> Delete(Config.ConfigCategory category)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "DELETE FROM config WHERE category=@category";
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
        public async Task<bool> Update(Config config)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "UPDATE config SET skill_card_update=@skill_card_update,max_buff=@max_buff WHERE category=@category";
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
        public async Task<Config> Query(Config.ConfigCategory category)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "SELECT skill_card_update,max_buff FROM config WHERE category=@category";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@category", category.ToString()));

                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, sqlcommand, parameters.ToArray());
                if (reader.Read())
                {
                    Config config = new Config();
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
    }
}
