using Material.MySQL.Dao.Interface;
using MySql.Data.MySqlClient;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Material.MySQL.Dao
{
    public class FriendDao : IFriendDao
    {
        MySqlConnection connection;
        public FriendDao(MySqlConnection connection)
        {
            this.connection = connection;
        }
    }
}
