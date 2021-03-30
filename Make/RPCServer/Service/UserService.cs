using Material.Entity;
using Material.Entity.Match;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Material.EtherealS.Annotation;
using Material.EtherealS.Extension.Authority;

namespace Make.RPCServer.Service
{
    public class UserService : IAuthoritable
    {
        public object Authority { get => 1; set { } }

        [RPCService(authority = 0)]
        public long RegisterUser(User user, string username, string nickname, string password)
        {
            Task<long> task = Core.Repository.UserRepository.Register(username, nickname, password);
            task.Wait();
            return task.Result;
        }
        [RPCService(authority = 0)]
        public long LoginUser(User user, long id, string username, string password)
        {
            Task<User> task = Core.Repository.UserRepository.Login(id, username, password);
            task.Wait();
            if (task.Result == null) return -1;//账户不存在
            else if (task.Result.Id != -1)
            {
                user.SetAttribute(task.Result);
                if (!user.AddIntoTokens())
                {
                    return -2;//用户已登录
                }
                else
                {
                    user.Authority = 1;
                    //Core.Repository.UserRepository.Update_CardRepositoryUpdate(user.Id, Material.Utils.TimeStamp.Now());
                    //foreach (SkillCard skillCard in Core.SkillCards.Values)
                    //{
                    //    CardItem item = new CardItem();
                    //    item.Category = Item.CardRepositoryCategory.SkillCard;
                    //    item.ItemId = skillCard.Id;
                    //    item.OwnerId = user.Id;
                    //    Core.Repository.UserRepository.Insert_CardRepository(item);
                    //}
                    return task.Result.Id;
                }
            }
            return task.Result.Id;
        }
        [RPCService]
        public User Sync_Attribute(User user, long date)
        {
            Task<User> task = Core.Repository.UserRepository.Sync_Attribute(user.Id, date);
            task.Wait();
            return task.Result;
        }
        [RPCService]
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
        [RPCService]
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
        [RPCService]
        public List<Friend> Sync_Friend(User user, long date)
        {
            Task<List<Friend>> task = Core.Repository.UserRepository.Sync_Friend(user.Id, date);
            task.Wait();

            Task<long> update_task = Core.Repository.UserRepository.Query_FriendUpdateById(user.Id);
            update_task.Wait();
            if (task.Result != null) Core.UserRequest.SetFriendUpdate(user, update_task.Result);
            return task.Result;
        }
        [RPCService]
        public long Update_CardGroups(User user, User userWithCardGroups)
        {
            Task<long> task = Core.Repository.UserRepository.Update_CardGroups(user.Id, userWithCardGroups);
            task.Wait();
            return task.Result;
        }
        [RPCService]
        public User Query_UserAttributeById(User user, long id)
        {
            Task<User> task = Core.Repository.UserRepository.Query_AttributeById(id);
            task.Wait();
            return task.Result;
        }
        [RPCService]
        public void Sync_CardRepository(User user,long skillCardDate, long repositoryDate)
        {
            Task<List<CardItem>> task = Core.Repository.UserRepository.Sync_CardRepository(user.Id, repositoryDate);
            List<SkillCard> skillCards = null;
            task.Wait();
            if(skillCardDate != Core.Config.SkillCardUpdate)
            {
                skillCards = new List<SkillCard>();
                if (task.Result != null)
                {
                    task = Core.Repository.UserRepository.Query_CardRepositoryById(user.Id);
                    task.Wait();
                }
                foreach(CardItem item in task.Result)
                {
                    Core.SkillCards.TryGetValue(item.ItemId, out SkillCard skillCard);
                    if (skillCard != null)
                    {
                        skillCards.Add(skillCard);
                    }
                }
            }
            Core.UserRequest.RefreshRepositorySkillCards(user, Core.Config.SkillCardUpdate, skillCards,task.Result);
        }
        [RPCService]
        public string CreateSquad(User user, string roomType)
        {
            if (Enum.TryParse(roomType, true, out Room.RoomType type))
            {
                if (type == Room.RoomType.RealTimeSolo)
                {
                    user.Squad = new MatchSquad(Material.Utils.SecretKey.Generate(20), type);
                    user.Squad.Captain = user;
                    if (user.Squad.Add(user)) return user.Squad.SecretKey;
                    else return "-1";
                }
                else return "-1";
            }
            else return "-1";
        }
    }
}
