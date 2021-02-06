using Material.Entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.MySQL.Dao.Interface
{
    public interface IUserDao
    {
        public Task<bool> Insert_User(string username, string nickname, string password);
        public Task<bool> Update_NickName(long id,string nickname);
        public Task<bool> Update_Password(long id, string password);
        public Task<long> ValidUser(string username, string password);
        public Task<UserBase> Query_UserAttributeByUsername(string username);
        public Task<UserBase> Query_UserAttributeByID(long id,bool has_password);

        public Task<long> Query_IdByUsername(string username);
    }
}
