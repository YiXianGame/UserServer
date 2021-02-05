using Make.MODEL;
using Material.Entity;
using Material.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Make.BLL.Server
{
    public class UserServer
    {
        public static long RegisterUser(Token token,string username,string nickname,string password)
        {
            Task<long> task = Core.Repository.Register(username, nickname, password);
            task.Wait();
            return task.Result;
        }
        public static long LoginUser(Token token,long id, string username,string password)
        {
            Task<long> task = Core.Repository.LoginUser(id,username, password);
            task.Wait();
            token.Id = task.Result;
            return token.Id;
        }
        public static UserBase Sync_UserAttribute(Token token,long date)
        {
            Task<UserBase> task = Core.Repository.Sync_UserAttribute(token.Id, date);
            task.Wait();
            return task.Result;
        }
        public static User Query_UserAttributeById(Token token, long id)
        {
            Task<UserBase> task = Core.Repository.Query_UserAttributeById(id);
            return null;
        }
    }
}
