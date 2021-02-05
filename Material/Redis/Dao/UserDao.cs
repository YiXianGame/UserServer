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
        public void SetAccount(UserBase user)
        {
            db.HashSetAsync("UA" + user.id, new HashEntry[]{ new HashEntry("username",user.username),new HashEntry("nickname",user.nickname),
                new HashEntry("upgrade_num",user.upgrade_num),new HashEntry("create_num",user.create_num),new HashEntry("money",user.money),new HashEntry("personal_signature",user.personalSignature),
                new HashEntry("battle_count",user.battleCount),new HashEntry("exp",user.exp),new HashEntry("lv",user.lv),new HashEntry("title",user.title),new HashEntry("active",user.active.ToString()),
                new HashEntry("exp",user.deaths),new HashEntry("lv",user.registerDate),
                new HashEntry("attribute_update",user.attribute_update),new HashEntry("skill_card_update", user.skillCard_update),new HashEntry("head_image_update",user.headImage_update) });
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
        public async Task<UserBase> Query_UserAttribute(long id, bool has_password = false)
        {
            RedisValue[] values = await db.HashGetAsync("UA" + id, new RedisValue[] { "username", "nickname","password", "upgrade_num", "create_num", "money", "personal_signature", "battle_count",
                "exp", "lv", "title","active","kills","deaths","register_date","attribute_update", "skill_card_update" , "head_image_update" });
            UserBase user = null;
            if (!values[0].IsNull)
            {
                user = new UserBase();
                user.username = values[0];
                user.nickname = values[1];
                if(has_password)user.password = values[2];
                user.upgrade_num = (int)values[3];
                user.create_num = (int)values[4];
                user.money = (long)values[5];
                user.personalSignature = values[6];
                user.battleCount = (int)values[7];
                user.exp = (long)values[8];
                user.lv = (int)values[9];
                user.title = values[10];
                user.active = (UserBase.State)Enum.Parse(typeof(UserBase.State), values[11]);
                user.kills = (int)values[12];
                user.deaths = (int)values[13];
                user.registerDate = (int)values[14];
                user.attribute_update = (long)values[15];
                user.skillCard_update = (long)values[16];
                user.headImage_update = (long)values[17];
            }
            return user;
        }
    }
}
