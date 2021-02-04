using Material.Redis.Dao.Interface;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Material.Redis.Dao
{
    public class UserDao : IUserDao
    {
        IDatabase db;

        public UserDao(IDatabase db)
        {
            this.db = db;
            
        }

        public void SetAccount(string username, string password, long id,long attribute_update, long skill_card_update, long head_image_update)
        {
            db.HashSetAsync("UA" + id,new HashEntry[]{ new HashEntry("username",username),new HashEntry("password",password),new HashEntry("id",id),new HashEntry("attribute_update",attribute_update),
                new HashEntry("skill_card_update", skill_card_update),new HashEntry("head_image_update",head_image_update)  });
        }

        public async Task<long> ValidPerson(long id, string password)
        {
            RedisValue[] values = await db.HashGetAsync("UA" + id, new RedisValue[] { "password"});
            if(!values[0].IsNull)
            {
                if (password.Equals(values[0]))
                {
                    return id;
                }
                else return -2;
            }
            return -1;
        }
        public async void Query_UserAttributeUpdate(long id,long date)
        {
            RedisValue[] values = await db.HashGetAsync("Account", new RedisValue[] { "password", "id" });
        }
    }
}
