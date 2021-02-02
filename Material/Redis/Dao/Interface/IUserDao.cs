using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.Redis.Dao.Interface
{
    public interface IUserDao
    {
        public void SetUserAccount(string username, string password, long id);
        public Task<long> ValidUser(string username, string password);
    }
}
