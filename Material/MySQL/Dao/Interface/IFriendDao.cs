using Material.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.MySQL.Dao.Interface
{
    public interface IFriendDao
    {
        Task<bool> Insert(Friend friend);
        Task<bool> Delete(long user_1, long user_2);
        Task<bool> Update(Friend friend);
        Task<Friend> Query(long user_1, long user_2);
        Task<List<Friend>> QueryAllById(long user);
    }
}
