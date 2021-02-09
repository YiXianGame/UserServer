using Material.Entity;
using Material.Redis.Dao.Interface;
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

        public void SetAccount(string username, string password, long id,long attribute_update, long skill_card_update, long head_image_update)
        {
            db.HashSetAsync("UA" + id,new HashEntry[]{ new HashEntry("username",username),new HashEntry("password",password),new HashEntry("attribute_update",attribute_update),
                new HashEntry("skill_card_update", skill_card_update),new HashEntry("head_image_update",head_image_update)  });
        }
        public void SetAccount(User user)
        {
            db.HashSetAsync("UA" + user.Id, new HashEntry[]{ new HashEntry("username",user.Username),new HashEntry("nickname",user.Nickname),new HashEntry("password",user.Password),
                new HashEntry("upgrade_num",user.Upgrade_num),new HashEntry("create_num",user.Create_num),new HashEntry("money",user.Money),new HashEntry("personal_signature",user.PersonalSignature),
                new HashEntry("battle_count",user.BattleCount),new HashEntry("exp",user.Exp),new HashEntry("lv",user.Lv),new HashEntry("title",user.Title),new HashEntry("active",user.Active.ToString()),
                new HashEntry("kills",user.Kills),new HashEntry("death",user.Deaths),new HashEntry("register_date",user.RegisterDate),
                new HashEntry("attribute_update",user.Attribute_update),new HashEntry("skill_card_update", user.SkillCard_update),new HashEntry("head_image_update",user.HeadImage_update) });
        }

        public async Task<long> ValidPerson(long id, string password)
        {
            RedisValue[] values = await db.HashGetAsync("UA" + id, new RedisValue[] { "password"});
            if(!values[0].IsNull)
            {
                if (password.Equals(values[0]))
                {
                    return id;
                }
                else return -2;
            }
            return -1;
        }
        public async Task<User> Query_UserAttribute(long id, bool has_password = false)
        {
            RedisValue[] values = await db.HashGetAsync("UA" + id, new RedisValue[] { "username", "nickname","password", "upgrade_num", "create_num", "money", "personal_signature", "battle_count",
                "exp", "lv", "title","active","kills","deaths","register_date","attribute_update", "skill_card_update" , "head_image_update" });
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
                user.Active = (User.State)Enum.Parse(typeof(User.State),values[11]);
                user.Kills = (int)values[12];
                user.Deaths = (int)values[13];
                user.RegisterDate = (int)values[14];
                user.Attribute_update = (long)values[15];
                user.SkillCard_update = (long)values[16];
                user.HeadImage_update = (long)values[17];
            }
            return user;
        }
    }
}
