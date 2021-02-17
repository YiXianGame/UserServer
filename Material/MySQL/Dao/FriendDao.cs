using Material.Entity;
using Material.MySQL.Dao.Interface;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.MySQL.Dao
{
    public class FriendDao : IFriendDao
    {
        string ConnectionString;
        public FriendDao(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        public MySqlConnection GetConnection(out MySqlConnection connection)
        {
            connection = new MySqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public async Task<bool> Insert(Friend friend)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "INSERT INTO friend(user_1,user_2,category) VALUES(@user_1,@user_2,@category)";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@user_1", friend.User_1));
                parameters.Add(new MySqlParameter("@user_2", friend.User_2));
                parameters.Add(new MySqlParameter("@category", friend.Category.ToString()));
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

        public async Task<Friend> Query(long user_1, long user_2)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "SELECT category FROM friend WHERE (user_1=@user_1 AND user_2=@user_2) OR (user_1=@user_2 AND user_2=@user_1)";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@user_1", user_1));
                parameters.Add(new MySqlParameter("@user_2", user_2));
                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, sqlcommand, parameters.ToArray());
                if (reader.Read())
                {
                    Friend result = new Friend();
                    result.User_1 = user_1;
                    result.User_2 = user_2;
                    result.Category = (Friend.FriendCategory)Enum.Parse(typeof(Friend.FriendCategory), reader.GetString("category"));
                    return result;
                }
                else return null;
            }
            finally
            {
                connection.Close();
            }
        }
        public async Task<List<Friend>> QueryAllById(long user)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "SELECT user_1,user_2,category FROM friend WHERE user_1=@user_1 OR user_2=@user_1";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@user_1", user));
                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, sqlcommand, parameters.ToArray());
                List<Friend> friends = new List<Friend>();
                while (reader.Read())
                {
                    Friend friend = new Friend();
                    friend.User_1 = reader.GetInt64("user_1");
                    friend.User_2 = reader.GetInt64("user_2");
                    friend.Category = (Friend.FriendCategory)Enum.Parse(typeof(Friend.FriendCategory), reader.GetString("category"));
                    friends.Add(friend);
                }
                return friends;
            }
            finally
            {
                connection.Close();
            }
        }
        public async Task<bool> Update(Friend friend)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "UPDATE config SET category=@category WHERE (user_1=@user_1 AND user_2=@user_2) OR (user_1=@user_2 AND user_2=@user_1)";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@user_1", friend.User_1));
                parameters.Add(new MySqlParameter("@user_2", friend.User_2));
                parameters.Add(new MySqlParameter("@category", friend.Category.ToString()));
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
        public async Task<bool> Delete(long user_1, long user_2)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "DELETE FROM friend WHERE (user_1=@user_1 AND user_2=@user_2) OR (user_1=@user_2 AND user_2=@user_1)";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@user_1", user_1));
                parameters.Add(new MySqlParameter("@user_1", user_2));
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

    }
}
