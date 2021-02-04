using System;
using System.Threading.Tasks;
using Material.Entity;
using MySql.Data.MySqlClient;

namespace Material.Repository
{

    public class Repository
    {
        Redis.Redis redis;
        MySQL.MySQL mySQL;
        public Repository(Redis.Redis redis, MySQL.MySQL mySQL)
        {
            this.redis = redis;
            this.mySQL = mySQL;
        }
        public async Task<long> LoginUser(long id,string username,string password)
        {
            //-1账户不存在 -2密码错误
            //有ID数据
            if(id != 0)
            {
                //在Redis中验证一下密码的正确性
                long result = await redis.userDao.ValidPerson(id, password);
                //Redis中不存在
                if (result == -1)
                {
                    //在Mysql中用ID查询验证一下密码
                    UserBase user = await mySQL.userDao.ValidUser(id, password);
                    //说明密码正确
                    if (user.id != -1 && user.id != -2)
                    {
                        //将账户信息存至Redis
                        redis.userDao.SetAccount(username, password, user.id, user.attribute_update, user.skillCard_update, user.skillCard_update);
                        return user.id;//返回验证值
                    }
                    else return user.id;//密码不正确，返回原因
                }
                else return result;//账户存在,返回验证值
            }
            else //首次登录，无ID数据
            {
                return await mySQL.userDao.ValidUser(username, password);
            }
        }
        public async Task<long> Register(string username,string nickname,string password)
        {
            bool result = await mySQL.userDao.Insert_Person(username, nickname, password);
            if (result)
            {
                MySqlDataReader reader = await mySQL.userDao.Query_UserByUsername(username);
                if (reader.Read())
                {
                    redis.userDao.SetAccount(username, password, reader.GetInt64("id"), reader.GetInt64("attribute_update"), reader.GetInt64("skill_card_update"), reader.GetInt64("head_image_update"));
                    return reader.GetInt64("id");
                }
                else return -1;
            }
            else return -1;
        }
        public async Task<UserBase> Sync_UserAttribute(long id,long date)
        {
            MySqlDataReader reader = await mySQL.userDao.Query_UserByID(id);
            UserBase user = null;
            if (reader.Read() && reader.GetInt64("attribute_update") != date)
            {
                user = new UserBase();
                user.id = reader.GetInt64("id");
                user.username = reader.GetString("username");
                user.nickname = reader.GetString("nickname");
                user.upgrade_num = reader.GetInt32("upgrade_num");
                user.create_num = reader.GetInt32("create_num");
                user.money = reader.GetInt32("money");
                user.personalSignature = reader.GetString("personal_signature");
                user.battleCount = reader.GetInt32("battle_count");
                user.exp = reader.GetInt64("exp");
                user.lv = reader.GetInt32("lv");
                user.title = reader.GetString("title");
                user.active = (UserBase.State)Enum.Parse(typeof(UserBase.State), reader.GetString("active"));
                user.kills = reader.GetInt32("kills");
                user.deaths = reader.GetInt32("deaths");
                user.registerDate = reader.GetInt64("register_date");
                user.attribute_update = reader.GetInt64("attribute_update");
                user.skillCard_update = reader.GetInt64("skill_card_update");
                user.headImage_update = reader.GetInt64("head_image_update");
            }
            return user;
        }
        public async Task<UserBase> Query_UserAttributeById(long id)
        {
            MySqlDataReader reader = await mySQL.userDao.Query_UserByID(id);
            UserBase user = null;
            if (reader.Read())
            {
                user = new UserBase();
                user.id = reader.GetInt64("id");
                user.username = reader.GetString("username");
                user.nickname = reader.GetString("nickname");
                user.upgrade_num = reader.GetInt32("upgrade_num");
                user.create_num = reader.GetInt32("create_num");
                user.money = reader.GetInt32("money");
                user.personalSignature = reader.GetString("personal_signature");
                user.battleCount = reader.GetInt32("battle_count");
                user.exp = reader.GetInt64("exp");
                user.lv = reader.GetInt32("lv");
                user.title = reader.GetString("title");
                user.active = (UserBase.State)Enum.Parse(typeof(UserBase.State), reader.GetString("active"));
                user.kills = reader.GetInt32("kills");
                user.deaths = reader.GetInt32("deaths");
                user.registerDate = reader.GetInt64("register_date");
                user.attribute_update = reader.GetInt64("attribute_update");
                user.skillCard_update = reader.GetInt64("skill_card_update");
                user.headImage_update = reader.GetInt64("head_image_update");
            }
            return user;
        }
    }
}
