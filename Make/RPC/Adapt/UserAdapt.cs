using Make.Model;
using Make.MODEL;
using Material.Entity;
using Material.RPC;
using Material.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Make.RPC.Adapt
{
    public class UserAdapt
    {
        public static long RegisterUser(UserToken token, string username, string nickname, string password)
        {
            Task<long> task = Core.Repository.UserRepository.Register(username, nickname, password);
            task.Wait();
            return task.Result;
        }
        public static long LoginUser(UserToken token, long id, string username, string password)
        {
            Task<long> task = Core.Repository.UserRepository.Login(id, username, password);
            task.Wait();
            if(task.Result!=-1 && task.Result != -2)
            {
                token.UserId = task.Result;
                if (!token.AddIntoTokens())
                {
                    return -3;//用户已登录
                }
                else return task.Result;
            }
            return token.UserId;
        }
        public static User Sync_Attribute(UserToken token, long date)
        {
            Task<User> task = Core.Repository.UserRepository.Sync_Attribute(token.UserId, date);
            task.Wait();
            return task.Result;
        }
        public static List<User> Sync_CardGroups(UserToken token, List<User> users)
        {
            List<User> result = new List<User>();
            foreach (User item in users)
            {
                Task<User> task = Core.Repository.UserRepository.Sync_CardGroups(item.Id, item.CardGroups_update);
                task.Wait();
                users.Add(task.Result);
            }
            return result;
        }
        public static List<User> Sync_Attribute(UserToken token, List<User> dates)
        {
            List<User> users = new List<User>();
            foreach(User item in dates)
            {
                Task<User> task = Core.Repository.UserRepository.Sync_Attribute(item.Id,item.Attribute_update);
                task.Wait();
                users.Add(task.Result);
            }
            return users;
        }
        public static List<Friend> Sync_Friend(UserToken token, long date)
        {
            Task<List<Friend>> task = Core.Repository.UserRepository.Sync_Friend(token.UserId, date);
            task.Wait();

            Task<long> update_task = Core.Repository.UserRepository.Query_FriendUpdateById(token.UserId);
            update_task.Wait();
            if(task.Result!=null)Core.UserRequest.SetFriendUpdate(token, update_task.Result);
            return task.Result;
        }
        public static long Update_CardGroups(UserToken token, User user)
        {
            Task<long> task = Core.Repository.UserRepository.Update_CardGroups(token.UserId, user);
            task.Wait();
            return task.Result;
        }
        public static User Query_UserAttributeById(UserToken token, long id)
        {
            Task<User> task = Core.Repository.UserRepository.Query_AttributeById(id);
            task.Wait();
            return task.Result;
        }
        public static List<CardItem> Sync_SkillCards(UserToken token,long id, long date)
        {
            Task<List<CardItem>> task = Core.Repository.UserRepository.Sync_UserSkillCards(id, date);
            task.Wait();

            Task<long> update_task = Core.Repository.UserRepository.Query_SkillCardUpdateById(id);
            update_task.Wait();
            if (task.Result != null) Core.UserRequest.SetSkillCardUpdate(token, update_task.Result);
            return task.Result;
        }

    }
}
