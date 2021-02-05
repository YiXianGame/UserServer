using Material.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.Redis.Dao.Interface
{
    public interface IUserDao
    {
        public void SetAccount(UserBase user);
        public void SetAccount(string username, string password, long id,long attribute_update,long skill_card_update,long head_image_update);
        public Task<long> ValidPerson(long id,string password);
        public Task<UserBase> Query_UserAttribute(long id, bool has_password = false);
    }
}
