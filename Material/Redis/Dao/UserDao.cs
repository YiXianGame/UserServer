using Material.Entity;
using Material.Redis.Dao.Interface;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public void SetAccount(string username, string password, long id,long attribute_update, long card_repository_update, long head_image_update)
        {
            db.HashSetAsync("UA" + id,new HashEntry[]{ new HashEntry("username",username),new HashEntry("password",password)});
        }
        public void SetAccount(User user)
        {
            db.HashSetAsync("UA" + user.Id, new HashEntry[]{ new HashEntry("username",user.Username),new HashEntry("nickname",user.Nickname),new HashEntry("password",user.Password),
                new HashEntry("upgrade_num",user.Upgrade_num),new HashEntry("create_num",user.Create_num),new HashEntry("money",user.Money),new HashEntry("personal_signature",user.PersonalSignature),
                new HashEntry("battle_count",user.BattleCount),new HashEntry("exp",user.Exp),new HashEntry("lv",user.Lv),new HashEntry("title",user.Title),new HashEntry("state",user.State.ToString()),
                new HashEntry("kills",user.Kills),new HashEntry("death",user.Deaths),new HashEntry("register_date",user.RegisterDate),new HashEntry("card_groups",JsonConvert.SerializeObject(user.CardGroups)),
                new HashEntry("attribute_update",user.Attribute_update),new HashEntry("card_repository_update", user.CardRepository_update),new HashEntry("head_image_update",user.HeadImage_update),new HashEntry("friend_update",user.Friend_update),new HashEntry("card_groups_update",user.CardGroups_update)});
        }
        public void SetCardGroups(long id,List<CardGroup> cardGroups,long timestamp)
        {
            db.HashSetAsync("UA" + id, new HashEntry[]{ new HashEntry("card_groups", JsonConvert.SerializeObject(cardGroups)), new HashEntry("card_groups_update", timestamp)});
        }
        public async Task<User> Query_User(long id, bool has_password = false)
        {
            RedisValue[] values = await db.HashGetAsync("UA" + id, new RedisValue[] { "username", "nickname","password", "upgrade_num", "create_num", "money", "personal_signature", "battle_count",
                "exp", "lv", "title","state","kills","deaths","register_date","attribute_update", "card_repository_update" , "head_image_update","card_groups","friend_update","card_groups_update" });
            User user = null;
            if (!values[0].IsNullOrEmpty)
            {
                user = new User();
                user.Id = id;
                user.Username = values[0];
                user.Nickname = values[1];
                if(has_password)user.Password = values[2];
                user.Upgrade_num = (int)values[3];
                user.Create_num = (int)values[4];
                user.Money = (long)values[5];
                user.PersonalSignature = values[6];
                user.BattleCount = (int)values[7];
                user.Exp = (long)values[8];
                user.Lv = (int)values[9];
                user.Title = values[10];
                user.State = (User.UserState)Enum.Parse(typeof(User.UserState),values[11]);
                user.Kills = (int)values[12];
                user.Deaths = (int)values[13];
                user.RegisterDate = (int)values[14];
                user.Attribute_update = (long)values[15];
                user.CardRepository_update = (long)values[16];
                user.HeadImage_update = (long)values[17];
                user.Friend_update = (long)values[19];
                user.CardGroups_update = (long)values[20];
                user.CardGroups = JsonConvert.DeserializeObject<List<CardGroup>>(values[18]);
                if (user.CardGroups == null) user.CardGroups = new List<CardGroup>();
            }
            return user;
        }

        public async Task<long> Query_CardRepositoryUpdate(long id)
        {
            RedisValue[] values = await db.HashGetAsync("UA" + id, new RedisValue[] { "card_repository_update" });
            if (!values[0].IsNull)
            {
                return (long)values[0];
            }
            else return -1;
        }

        public async Task<long> Query_FriendUpdate(long id)
        {
            RedisValue[] values = await db.HashGetAsync("UA" + id, new RedisValue[] { "friend_update" });
            if (!values[0].IsNull)

            {
                return (long)values[0];
            }
            else return -1;
        }

        public void SetState(long id, User.UserState state, long timestamp)
        {
            db.HashSetAsync("UA" + id, new HashEntry[] { new HashEntry("state", state.ToString()), new HashEntry("attribute_update", timestamp) });
        }

        public async Task<long> Query_CardGroupsUpdate(long id)
        {
            RedisValue[] values = await db.HashGetAsync("UA" + id, new RedisValue[] { "card_groups_update" });
            if (!values[0].IsNull)

            {
                return (long)values[0];
            }
            else return -1;
        }
        public async Task<List<CardGroup>> Query_CardGroups(long id)
        {
            RedisValue[] values = await db.HashGetAsync("UA" + id, new RedisValue[] { "card_groups" });
            if (!values[0].IsNull)
            {
                return JsonConvert.DeserializeObject<List<CardGroup>>(values[0]);
            }
            else return null;
        }

        public async Task<User> Query_UserAttribute(long id)
        {
            RedisValue[] values = await db.HashGetAsync("UA" + id, new RedisValue[] { "username", "nickname","password", "upgrade_num", "create_num", "money", "personal_signature", "battle_count",
                "exp", "lv", "title","state","kills","deaths","register_date","attribute_update"});
            User user = null;
            if (!values[0].IsNullOrEmpty)
            {
                user = new User();
                user.Id = id;
                user.Username = values[0];
                user.Nickname = values[1];
                user.Password = values[2];
                user.Upgrade_num = (int)values[3];
                user.Create_num = (int)values[4];
                user.Money = (long)values[5];
                user.PersonalSignature = values[6];
                user.BattleCount = (int)values[7];
                user.Exp = (long)values[8];
                user.Lv = (int)values[9];
                user.Title = values[10];
                user.State = (User.UserState)Enum.Parse(typeof(User.UserState), values[11]);
                user.Kills = (int)values[12];
                user.Deaths = (int)values[13];
                user.RegisterDate = (int)values[14];
                user.Attribute_update = (long)values[15];
            }
            return user;
        }
    }
}
