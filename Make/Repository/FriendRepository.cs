using Material.Entity;
using Material.MySQL;
using Material.Redis;
using System.Threading.Tasks;

namespace Make.Repository
{
    public class FriendRepository
    {
        #region --字段--
        private Redis redis;
        private MySQL mySQL;
        #endregion

        #region --方法--
        public FriendRepository(Redis redis, MySQL mySQL)
        {
            this.redis = redis;
            this.mySQL = mySQL;
        }
        public async Task<bool> Insert(Friend friend)
        {
            return await mySQL.friendDao.Insert(friend);
        }
        public async Task<bool> Update(Friend friend)
        {
            return await mySQL.friendDao.Update(friend);
        }
        public async Task<Friend> Query(long user_1, long user_2)
        {
            return await mySQL.friendDao.Query(user_1, user_2);
        }
        public async Task<bool> Delete(long user_1, long user_2)
        {
            return await mySQL.friendDao.Delete(user_1, user_2);
        }
        #endregion
    }
}
