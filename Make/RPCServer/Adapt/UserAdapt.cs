using Make.Model;
using Material.Entity;
using Material.Entity.Game;
using Material.Entity.Match;
using Material.RPCServer.Annotation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Make.RPCServer.Adapt
{
    public class UserAdapt
    {
        [RPCMethod]
        public long RegisterUser(User user, string username, string nickname, string password)
        {
            Task<long> task = Core.Repository.UserRepository.Register(username, nickname, password);
            task.Wait();
            return task.Result;
        }
        [RPCMethod]
        public long LoginUser(User user, long id, string username, string password)
        {
            Task<User> task = Core.Repository.UserRepository.Login(id, username, password);
            task.Wait();
            if (task.Result == null) return -1;//账户不存在
            else if (task.Result.Id != -2)
            {
                user.SetAttribute(task.Result);
                if (!user.AddIntoTokens())
                {
                    return -3;//用户已登录
                }
                else return task.Result.Id;
            }
            return task.Result.Id;
        }
        [RPCMethod]
        public User Sync_Attribute(User user, long date)
        {
            Task<User> task = Core.Repository.UserRepository.Sync_Attribute(user.Id, date);
            task.Wait();
            return task.Result;
        }
        [RPCMethod]
        public List<User> Sync_CardGroups(User user, List<User> users)
        {
            List<User> result = new List<User>();
            foreach (User item in users)
            {
                Task<User> task = Core.Repository.UserRepository.Sync_CardGroups(item.Id, item.CardGroups_update);
                task.Wait();
                result.Add(task.Result);
            }
            return result;
        }
        [RPCMethod]
        public List<User> Sync_Attribute(User user, List<User> dates)
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
        [RPCMethod]
        public List<Friend> Sync_Friend(User user, long date)
        {
            Task<List<Friend>> task = Core.Repository.UserRepository.Sync_Friend(user.Id, date);
            task.Wait();

            Task<long> update_task = Core.Repository.UserRepository.Query_FriendUpdateById(user.Id);
            update_task.Wait();
            if (task.Result != null) Core.UserRequest.SetFriendUpdate(user, update_task.Result);
            return task.Result;
        }
        [RPCMethod]
        public long Update_CardGroups(User user, User userWithCardGroups)
        {
            Task<long> task = Core.Repository.UserRepository.Update_CardGroups(user.Id, userWithCardGroups);
            task.Wait();
            return task.Result;
        }
        [RPCMethod]
        public User Query_UserAttributeById(User user, long id)
        {
            Task<User> task = Core.Repository.UserRepository.Query_AttributeById(id);
            task.Wait();
            return task.Result;
        }
        [RPCMethod]
        public List<CardItem> Sync_CardRepository(User user, long id, long date)
        {
            Task<List<CardItem>> task = Core.Repository.UserRepository.Sync_CardRepository(id, date);
            task.Wait();
            if (task.Result != null)
            {
                Task<long> update_task = Core.Repository.UserRepository.Query_CardRepositoryUpdateById(id);
                update_task.Wait();
                Core.UserRequest.SetCardRepositoryUpdate(user, update_task.Result);
            }
            return task.Result;
        }
        [RPCMethod]
        public string CreateSquad(User user,string roomType)
        {
            if(Enum.TryParse<Room.RoomType>(roomType, true, out Room.RoomType type))
            {
                if (type == Room.RoomType.Round_Solo)
                {
                    user.Squad = new Squad(Material.Utils.SecretKey.Generate(20), type);
                    user.Squad.Captain = user;
                    if (user.Squad.Add(user)) return user.Squad.SecretKey;
                    else return "-1";
                }
                else return "-1";
            }
            else return "-1";
        }
        [RPCMethod]
        public List<User> EnterSquad(User user,long id,string secretKey)
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
                            Core.ReadyRequest.RefreshSquad(item,users);
                        }
                        return users;
                    }
                    else return null;
                }
                else return null;
            }
            else return null;
        }
        [RPCMethod]
        public bool InviteSquad(User user,long id)
        {
            if(user.Squad != null)
            {
                if (user.GetToken(id, out User invited))
                {
                    Core.ReadyRequest.InviteSquad(invited, user, user.Squad.SecretKey);
                    return true;
                }
                else return false;
            }
            else return false;
        }
        [RPCMethod]
        public void StartMatch(User user)
        {
            if (user.Squad != null)
            {
                if(user.Squad.Captain == user && user.Squad.RoomType == Room.RoomType.Round_Solo)
                {
                    if (Core.SoloMatchSystem.Enter(user.Squad))
                    {
                        foreach (User player in user.Squad.Items)
                        {
                            Core.ReadyRequest.StartMatch(player);
                        }
                    }
                }
            }
        }
    }
}
