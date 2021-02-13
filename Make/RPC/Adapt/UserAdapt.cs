using Make.MODEL;
using Material.Entity;
using Material.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Make.RPC.Adapt
{
    public class UserAdapt
    {
        public static long RegisterUser(Token token, string username, string nickname, string password)
        {
            Task<long> task = Core.Repository.UserRepository.Register(username, nickname, password);
            task.Wait();
            return task.Result;
        }
        public static long LoginUser(Token token, long id, string username, string password)
        {
            Task<long> task = Core.Repository.UserRepository.Login(id, username, password);
            task.Wait();
            token.Id = task.Result;
            return token.Id;
        }
        public static User Sync_UserAttribute(Token token, long date)
        {
            Task<User> task = Core.Repository.UserRepository.Sync_Attribute(token.Id, date);
            task.Wait();
            return task.Result;
        }
        public static User Query_UserAttributeById(Token token, long id)
        {
            Task<User> task = Core.Repository.UserRepository.Query_AttributeById(id);
            return null;
        }

    }
}
