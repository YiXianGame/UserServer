using Material.Entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Material.Entity.User;

namespace Material.MySQL.Dao.Interface
{
    public interface IUserDao
    {
        public Task<long> Insert(string username, string nickname, string password);
        public Task<bool> Update_NickName(long id,string nickname);
        public Task<bool> Update_Password(long id, string password);
        public Task<bool> Update_CardGroups(long id, List<CardGroup> cardGroup, long timestamp);
        public Task<bool> Update_State(long id,UserState state, long timestamp);
        public Task<User> Query_UserByUsername(string username);
        public Task<User> Query_UserByID(long id,bool has_password = false);
        public Task<long> Query_IdByUsername(string username);
    }
}
