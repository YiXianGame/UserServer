using Material.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Make.MODEL.Server
{
    public class UserServer
    {
        public static User RegisterUser(Token token,User user)
        {
            Task<long> task = Core.Repository.Register(user.UserName, user.Passwords, user.NickName);
            task.Wait(30000);
            user.UserName = "啦啦啦";
            return user;
        }
        public static User test(Token token, User user)
        {
            user.NickName = "沫";
            return user;
        }
        public static void  UpdateUser(Token token,User user)
        {

        }
        public static User QueryUserByUserName(Token token,String userName)
        {
            return null;
        }
        public static User QueryUserById(Token token, long id)
        {
            return null;
        }
        public static List<User> QueryAllFriendUsers(Token token, long user_id)
        {
            return null;
        }
    }
}
