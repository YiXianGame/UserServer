using Make.Model;
using Material.Entity;
using Material.Entity.Game;
using Material.Entity.Match;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Make.RPCServer.Adapt
{
    public class UserAdapt
    {
        public static long RegisterUser(User user, string username, string nickname, string password)
        {
            Task<long> task = Core.Repository.UserRepository.Register(username, nickname, password);
            task.Wait();
            return task.Result;
        }
        public static long LoginUser(User user, long id, string username, string password)
        {
            Task<long> task = Core.Repository.UserRepository.Login(id, username, password);
            task.Wait();
            if (task.Result != -1 && task.Result != -2)
            {
                user.Id = task.Result;
                Task<User> userTask = Core.Repository.UserRepository.Query_AttributeById(user.Id);
                userTask.Wait();
                user.SetAttribute(userTask.Result);
                if (!user.AddIntoTokens())
                {
                    return -3;//用户已登录
                }
                else return task.Result;
            }
            return user.Id;
        }
        public static User Sync_Attribute(User user, long date)
        {
            Task<User> task = Core.Repository.UserRepository.Sync_Attribute(user.Id, date);
            task.Wait();
            return task.Result;
        }
        public static List<User> Sync_CardGroups(User user, List<User> users)
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
        public static List<User> Sync_Attribute(User user, List<User> dates)
        {
            List<User> users = new List<User>();
            foreach (User item in dates)
            {
                Task<User> task = Core.Repository.UserRepository.Sync_Attribute(item.Id, item.Attribute_update);
                task.Wait();
                users.Add(task.Result);
            }
            return users;
        }
        public static List<Friend> Sync_Friend(User user, long date)
        {
            Task<List<Friend>> task = Core.Repository.UserRepository.Sync_Friend(user.Id, date);
            task.Wait();

            Task<long> update_task = Core.Repository.UserRepository.Query_FriendUpdateById(user.Id);
            update_task.Wait();
            if (task.Result != null) Core.UserRequest.SetFriendUpdate(user, update_task.Result);
            return task.Result;
        }
        public static long Update_CardGroups(User user, User userWithCardGroups)
        {
            Task<long> task = Core.Repository.UserRepository.Update_CardGroups(user.Id, userWithCardGroups);
            task.Wait();
            return task.Result;
        }
        public static User Query_UserAttributeById(User user, long id)
        {
            Task<User> task = Core.Repository.UserRepository.Query_AttributeById(id);
            task.Wait();
            return task.Result;
        }
        public static List<CardItem> Sync_SkillCards(User user, long id, long date)
        {
            Task<List<CardItem>> task = Core.Repository.UserRepository.Sync_UserSkillCards(id, date);
            task.Wait();

            Task<long> update_task = Core.Repository.UserRepository.Query_SkillCardUpdateById(id);
            update_task.Wait();
            if (task.Result != null) Core.UserRequest.SetSkillCardUpdate(user, update_task.Result);
            return task.Result;
        }

        public static string CreateSquad(User user,string roomType)
        {
            Room.RoomType type = (Room.RoomType)Enum.Parse(typeof(Room.RoomType), roomType);
            if(user.Rank == -1)
            {
                Task<User> task = Core.Repository.UserRepository.Query_AttributeById(user.Id);
                task.Wait();
                user.Rank = task.Result.Lv;
            }
            if (type == Room.RoomType.Solo)
            {
                user.Squad = new Squad(Material.Utils.SecretKey.Generate(20),type);
                user.Squad.Captain = user;
                if(user.Squad.Add(user))return user.Squad.SecretKey;
                else return "-1";
            }
            else return "-1";
        }
        public static List<User> EnterSquad(User user,long id,string secretKey)
        {
            if (user.GetToken(id, out User host))
            {
                if (host.Squad != null)
                {
                    if (user.Rank == -1)
                    {
                        Task<User> task = Core.Repository.UserRepository.Query_AttributeById(user.Id);
                        task.Wait();
                        user.Rank = task.Result.Lv;
                    }
                    
                    if (host.Squad.SecretKey.Equals(secretKey) && host.Squad.Add(user))
                    {
                        List<User> users = new List<User>(host.Squad.Items);
                        foreach (User item in host.Squad.Items)
                        {
                            Core.UserRequest.RefreshSquad(item,users);
                        }
                        return users;
                    }
                    else return null;
                }
                else return null;
            }
            else return null;
        }
        public static bool InviteSquad(User user,long id)
        {
            if(user.Squad != null)
            {
                if (user.GetToken(id, out User invited))
                {
                    Core.UserRequest.InviteSquad(invited, user, user.Squad.SecretKey);
                    return true;
                }
                else return false;
            }
            else return false;
        }

        public static bool StartMatch(User user)
        {
            if (user.Squad != null)
            {
                if(user.Squad.Captain == user && user.Squad.RoomType == Room.RoomType.Solo)
                {
                    Core.SoloMatchSystem.Enter(user.Squad);
                    return true;
                }
                else return false;
            }
            else return false;
        }
    }
}
