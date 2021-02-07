using Make.BLL.Server;
using Make.MODEL;
using Material;
using Material.MySQL;
using Material.Redis;
using Material.Repository;
using Material.RPC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Make.BLL
{
    public class Initialization
    {
        public Initialization()
        {
            RPCType type = new RPCType();
            type.Add<int>("int");
            type.Add<string>("string");
            type.Add<bool>("bool");
            type.Add<long>("long");
            type.Add<User>("user", obj => ((JObject)obj).ToObject<User>());
            type.Add<SkillCard>("skillCard", obj => ((JObject)obj).ToObject<User>());
            //适配远程客户端服务
            RPCAdaptFactory.Register<UserServer>("UserServer", "192.168.0.105", "28015", type);
            //注册远程服务
            Core.UserRequest = RPCRequestProxyFactory.Register<UserClient>("UserClient", "192.168.0.105", "28015", type);
            Redis redis = new Redis("127.0.0.1:6379");
            MySQL mySQL = new MySQL("127.0.0.1", "3306", "yixian", "root", "root");
            Repository repository = new Repository(redis, mySQL);
            Core.Repository = repository;
            Skill_Cards_Load();
            Adventures_Load();
        }
        public void Skill_Cards_Load()
        {
            int first = 1;
            if (first == 1)
            {
                Random random = new Random();
                DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local); // 当地时区
                long timeStamp = (long)(DateTime.Now - startTime).TotalSeconds; // 相差秒数
                for (int i = 0; i < 100; i++)
                {
                    SkillCard skillCard = new SkillCard();
                    skillCard.Name = $"{i}";
                    skillCard.AuthorId = 839336369;
                    skillCard.AuxiliaryHp = random.Next()%100;
                    skillCard.AuxiliaryMp = random.Next()%100;
                    skillCard.Description = "123";
                    skillCard.EnemyHp = random.Next()%100;
                    skillCard.EnemyMp = random.Next()%100;
                    skillCard.MaxAuxiliary = random.Next()%5;
                    skillCard.MaxEnemy = random.Next() % 5;
                    skillCard.Mp = random.Next() % 100;
                    skillCard.Probability = random.Next() % 100;
                    skillCard.RegisterDate = random.Next();
                    skillCard.AttributeUpdate = timeStamp;
                    for (int j = 0; j < random.Next() % 5; j++)
                    {
                        Buff buff = new Buff();
                        buff.Can_Effect(random.Next() % 100);
                        buff.Category = Material.Entity.BuffBase.BuffCategory.Freeze;
                        buff.Duration = random.Next() % 100;
                        skillCard.AuxiliaryBuff.Add(buff);
                    }
                    for (int j = 0; j < random.Next() % 5; j++)
                    {
                        Buff buff = new Buff();
                        buff.Can_Effect(random.Next() % 100);
                        buff.Category = Material.Entity.BuffBase.BuffCategory.Freeze;
                        buff.Duration = random.Next() % 100;
                        skillCard.EnemyBuff.Add(buff);
                    }
                    for (int j = 0; j < random.Next() % 5; j++)
                    {
                        if (random.Next() % 2 == 1) skillCard.Category.Add(Material.Entity.SkillCardBase.SkillCardCategory.Attack);
                        if (random.Next() % 2 == 1) skillCard.Category.Add(Material.Entity.SkillCardBase.SkillCardCategory.Cure);
                        if (random.Next() % 2 == 1) skillCard.Category.Add(Material.Entity.SkillCardBase.SkillCardCategory.Eternal);
                        if (random.Next() % 2 == 1) skillCard.Category.Add(Material.Entity.SkillCardBase.SkillCardCategory.Magic);
                        if (random.Next() % 2 == 1) skillCard.Category.Add(Material.Entity.SkillCardBase.SkillCardCategory.Physics);
                    }
                    Core.Repository.SkillCardRepository.Insert(skillCard).Wait();
                }
            }
        }
        public void Adventures_Load()
        {

        }
    }
}
