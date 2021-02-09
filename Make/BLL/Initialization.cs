using Make.Model;
using Make.RPC.Request;
using Make.RPC.Server;
using Material.Entity;
using Material.MySQL;
using Material.Redis;
using Material.RPC;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

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
            type.Add<List<SkillCard>>("skillCards", obj => ((JObject)obj).ToObject<List<SkillCard>>());
            //适配远程客户端服务
            RPCAdaptFactory.Register<UserServer>("UserServer", "192.168.0.105", "28015", type);
            //注册远程服务
            Core.UserRequest = RPCRequestProxyFactory.Register<UserClient>("UserClient", "192.168.0.105", "28015", type);
            Redis redis = new Redis("127.0.0.1:6379");
            MySQL mySQL = new MySQL("127.0.0.1", "3306", "yixian", "root", "root");
            Model.Repository repository = new Model.Repository(redis, mySQL);
            Core.Repository = repository;
            CoreInit(Config.ConfigCategory.StandardServer);
            SkillCardInit();
            AdventuresInit();
            if (typeof(List<SkillCard>).Equals(typeof(List<SkillCard>)))
            {
                Console.WriteLine("相同");
            }
            else Console.WriteLine("不相同");
        }

        private async void CoreInit(Config.ConfigCategory category)
        {
            Console.WriteLine("Core Loading....");
            //全局静态，查询以后会将Core静态属性全部设置好.
            bool result = await Core.Repository.ConfigRepository.Query(category);
            //如果没找到，就执行默认配置
            if (!result)
            {
                Core.Config = new Config();
                Core.Config.Category = category;
                Core.Config.SkillCardUpdate = 0;
                Core.Config.MaxBuff = 8;
                if(!(await Core.Repository.ConfigRepository.Insert(Core.Config)))
                {
                    Console.WriteLine("Core Load Fail!");
                }
            }
            Console.WriteLine("Core Load Sucess!");
        }

        public async void SkillCardInit()
        {
            Console.WriteLine("SkillCard Loading....");
            int first = 0;
            if (first == 1)
            {
                Random random = new Random();
                DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local); // 当地时区
                long timeStamp = (long)(DateTime.Now - startTime).TotalSeconds; // 相差秒数
                for (int i = 0; i < 100; i++)
                {
                    SkillCard skillCard = new SkillCard();
                    skillCard.Name = $"第{i}张卡牌";
                    skillCard.AuthorId = 839336369;
                    skillCard.AuxiliaryHp = random.Next()%100;
                    skillCard.AuxiliaryMp = random.Next()%100;
                    skillCard.Description = "这是一段描述";
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
                        buff.Power = random.Next() % 100;
                        buff.Category = Material.Entity.Buff.BuffCategory.Freeze;
                        buff.Duration = random.Next() % 100;
                        skillCard.AuxiliaryBuff.Add(buff);
                    }
                    for (int j = 0; j < random.Next() % 5; j++)
                    {
                        Buff buff = new Buff();
                        buff.Power = random.Next() % 100;
                        buff.Category = Material.Entity.Buff.BuffCategory.Freeze;
                        buff.Duration = random.Next() % 100;
                        skillCard.EnemyBuff.Add(buff);
                    }
                    for (int j = 0; j < random.Next() % 5; j++)
                    {
                        if (random.Next() % 2 == 1) skillCard.Category.Add(Material.Entity.SkillCard.SkillCardCategory.Attack);
                        if (random.Next() % 2 == 1) skillCard.Category.Add(Material.Entity.SkillCard.SkillCardCategory.Cure);
                        if (random.Next() % 2 == 1) skillCard.Category.Add(Material.Entity.SkillCard.SkillCardCategory.Eternal);
                        if (random.Next() % 2 == 1) skillCard.Category.Add(Material.Entity.SkillCard.SkillCardCategory.Magic);
                        if (random.Next() % 2 == 1) skillCard.Category.Add(Material.Entity.SkillCard.SkillCardCategory.Physics);
                    }
                    long result = await Core.Repository.SkillCardRepository.Insert(skillCard);
                    if (result == -1)
                    {
                        Console.WriteLine("Core Load Fail!");
                        break;
                    }
                }
            }
            Console.WriteLine("SkillCard Load Sucess!");
        }
        public void AdventuresInit()
        {

        }
    }
}
