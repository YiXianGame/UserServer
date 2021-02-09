using Material.Entity;
using Material.MySQL.Dao.Interface;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Material.MySQL.Dao
{
    public class CardRepositoryDao : ICardRepositoryDao
    {
        string ConnectionString;
        public CardRepositoryDao(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        public MySqlConnection GetConnection(out MySqlConnection connection)
        {
            connection = new MySqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }


        public async Task<bool> Insert(CardRepository item)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "INSERT INTO card_repository(owner_id,item_id,solution) VALUES(@owner_id,@item_id,@solution)";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@owner_id", item.OwnerId));
                parameters.Add(new MySqlParameter("@item_id", item.ItemId));
                parameters.Add(new MySqlParameter("@solution", item.Category));
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
        public async Task<bool> Delete(long owner_id, long item_id)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "DELETE FROM card_repository WHERE owner_id=@owner_id,item_id=@item_id";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@owner_id", owner_id));
                parameters.Add(new MySqlParameter("@item_id", item_id));
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
        public async Task<bool> Update(CardRepository item)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "UPDATE card_repository SET solution=@solution WHERE owner_id=@owner_id,item_id=@item_id";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@owner_id", item.OwnerId));
                parameters.Add(new MySqlParameter("@item_id", item.ItemId));
                parameters.Add(new MySqlParameter("@solution", item.Category));
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
        public async Task<CardRepository> Query(long owner_id, long item_id)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "SELECT solution FROM card_repository WHERE owner_id=@owner_id,item_id=@item_id";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@owner_id", owner_id));
                parameters.Add(new MySqlParameter("@item_id", item_id));
                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, sqlcommand, parameters.ToArray());
                if (reader.Read())
                {
                    CardRepository repository = new CardRepository();
                    repository.OwnerId = owner_id;
                    repository.ItemId = item_id;
                    repository.Category = (CardRepository.CardRepositoryCategory)Enum.Parse(typeof(CardRepository.CardRepositoryCategory), reader.GetString("solution"));
                    return repository;
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
