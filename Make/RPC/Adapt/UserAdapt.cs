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
            token.UserId = task.Result;
            return token.UserId;
        }
        public static User Sync_UserAttribute(Token token, long date)
        {
            Task<User> task = Core.Repository.UserRepository.Sync_Attribute(token.UserId, date);
            task.Wait();
            return task.Result;
        }
        public static long Update_CardGroups(Token token, User user)
        {
            Task<long> task = Core.Repository.UserRepository.Update_CardGroups(token.UserId, user);
            task.Wait();
            return task.Result;
        }
        public static User Query_UserAttributeById(Token token, long id)
        {
            Task<User> task = Core.Repository.UserRepository.Query_AttributeById(id);
            task.Wait();
            return task.Result;
        }
        public static List<CardItem> Sync_UserSkillCards(Token token,long id, long date)
        {
            Task<List<CardItem>> task = Core.Repository.UserRepository.Sync_UserSkillCards(id, date);
            task.Wait();
            Task<long> update_task = Core.Repository.UserRepository.Query_SkillCardUpdateById(id);
            update_task.Wait();
            if(id == token.UserId)Core.UserClient.SyncSkillCardUpdate(token, update_task.Result);
            return task.Result;
        }
    }
}
