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
        public Task<long> Insert(string username, string nickname, string password);
        public Task<bool> Update_NickName(long id,string nickname);
        public Task<bool> Update_Password(long id, string password);
        public Task<long> Valid(string username, string password);
        public Task<UserBase> Query_AttributeByUsername(string username);
        public Task<UserBase> Query_AttributeByID(long id,bool has_password = false);

        public Task<long> Query_IdByUsername(string username);
    }
}
