using Material.Entity;
using Material.MySQL.Dao.Interface;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Material.MySQL.Dao
{
    public class CoreDao : ICoreDao
    {
        string ConnectionString;
        public CoreDao(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        public MySqlConnection GetConnection(out MySqlConnection connection)
        {
            connection = new MySqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }


        public async Task<bool> Insert(CoreBase item)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "INSERT INTO core(category,skill_card_update) VALUES(@category,@skill_card_update)";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@category", item.Category.ToString()));
                parameters.Add(new MySqlParameter("@skill_card_update", item.SkillCardUpdate));
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
        public async Task<bool> Delete(CoreBase.ConfigCategory category)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "DELETE FROM core WHERE category=@category";
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
        public async Task<bool> Update(CoreBase core)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "UPDATE core SET skill_card_update=@skill_card_update WHERE category=@category";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@category", core.Category.ToString()));
                parameters.Add(new MySqlParameter("@skill_card_update", core.SkillCardUpdate));
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
        public async Task<CoreBase> Query(CoreBase.ConfigCategory category)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "SELECT skill_card_update FROM core WHERE category=@category";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@category", category.ToString()));
                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, sqlcommand, parameters.ToArray());
                if (reader.Read())
                {
                    CoreBase core = new CoreBase();
                    core.Category = category;
                    core.SkillCardUpdate = reader.GetInt64("skill_card_update");
                    return core;
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
