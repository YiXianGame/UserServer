using Make.Model;
using Make.RPCClient.Request;
using Make.RPCServer.Adapt;
using Make.RPCServer.Request;
using Material.Entity;
using Material.Entity.Config;
using Material.Entity.Match;
using Material.MySQL;
using Material.Redis;
using Material.RPCServer;
using System;
using System.Collections.Generic;

namespace Make.BLL
{
    public class Initialization
    {
        public Initialization()
        {
            Console.WriteLine("Initialization....");
            Redis redis = new Redis("127.0.0.1:6379");
            MySQL mySQL = new MySQL("127.0.0.1", "3306", "yixian", "root", "root");
            Core.Repository = new Model.Repository(redis, mySQL);
            CoreInit(UserServerConfig.UserServerCategory.StandardServer,PlayerServerConfig.PlayerServerCategory.StandardServer);
            Core.SoloMatchSystem.MatchPiplineOut += Core.SoloGroupMatchSystem.PiplineIn;
            Core.SoloGroupMatchSystem.MatchSuccessEvent += MatchSystemHelper.SoloGroupMatchSystem_MatchSuccessEvent;

            #region --RPCServer--
            RPCType serverType = new RPCType();
            serverType.Add<int>("int");
            serverType.Add<string>("string");
            serverType.Add<bool>("bool");
            serverType.Add<long>("long");
            serverType.Add<User>("user");
            serverType.Add<SkillCard>("skillCard");
            serverType.Add<List<long>>("longs");
            serverType.Add<List<SkillCard>>("skillCards");
            serverType.Add<List<CardItem>>("cardItem");
            serverType.Add<List<CardGroup>>("cardGroups");
            serverType.Add<List<Friend>>("friends");
            serverType.Add<List<User>>("users");
            //适配Server远程客户端服务
            RPCAdaptFactory.Register(new UserAdapt(),"UserServer", "192.168.0.105", "28015", serverType);
            RPCAdaptFactory.Register(new SkillCardAdapt(),"SkillCardServer", "192.168.0.105", "28015", serverType);
            //注册Server远程服务
            Core.UserRequest = RPCRequestProxyFactory.Register<UserRequest>("UserClient", "192.168.0.105", "28015", serverType);
            Core.SkillCardRequest = RPCRequestProxyFactory.Register<SkillCardRequest>("SkillCardClient", "192.168.0.105", "28015", serverType);
            Core.ReadyRequest = RPCRequestProxyFactory.Register<ReadyRequest>("ReadyClient", "192.168.0.105", "28015", serverType);
            //启动Server服务
            RPCNetServerFactory.StartServer("192.168.0.105", "28015", () => new User());
            #endregion

            #region --RPCClient--
            Material.RPCClient.RPCType clientType = new Material.RPCClient.RPCType();
            clientType.Add<int>("int");
            clientType.Add<string>("string");
            clientType.Add<bool>("bool");
            clientType.Add<long>("long");
            clientType.Add<User>("user");
            clientType.Add<SkillCard>("skillCard");
            clientType.Add<List<SkillCard>>("skillCards");
            clientType.Add<List<CardItem>>("cardItem");
            clientType.Add<List<CardGroup>>("cardGroups");
            clientType.Add<List<Friend>>("friends");
            clientType.Add<List<User>>("users");
            //注册Client远程服务
            Core.PlayerServerRequest = Material.RPCClient.RPCRequestProxyFactory.Register<PlayerServerRequest>("PlayerServer", "192.168.0.105", "28016", clientType);
            //启动Client服务
            //RPCNetClientFactory.StartClient("192.168.0.105", "28015");
            #endregion
            Random random = new Random();
            for (int i = 0; i < 1; i++)
            {
                Squad squad = new Squad(Material.Utils.SecretKey.Generate(10),Material.Entity.Game.Room.RoomType.Round_Solo);
                for (int j = 0; j < 1; j++)
                {
                    User user = new User();
                    user.Rank = random.Next(1, 9);
                    user.AverageRank = user.Rank;
                    user.Count = 1;
                    user.StartMatchTime = Material.Utils.TimeStamp.Now();
                    squad.Add(user);
                }
                Core.SoloMatchSystem.Enter(squad);
            }
            Core.SoloMatchSystem.StartPolling(0,5000);
            SkillCardInit();
            AdventuresInit();
            Console.WriteLine("Initialization Success!");
        }
        private async void CoreInit(UserServerConfig.UserServerCategory category, PlayerServerConfig.PlayerServerCategory playerServerCategory)
        {
            Console.WriteLine("Core Loading....");
            //全局静态，查询以后会将Core静态属性全部设置好.
            UserServerConfig config = await Core.Repository.ConfigRepository.Query(category);
            //如果没找到，就执行默认配置
            if (config == null)
            {
                Core.Config = new UserServerConfig();
                Core.Config.Category = category;
                Core.Config.SkillCardUpdate = 0;
                Core.Config.MaxBuff = 8;
                if (!(await Core.Repository.ConfigRepository.Insert(Core.Config)))
                {
                    Console.WriteLine($"Core Load Fail! Can not Find {category}");
                    return;
                }
            }
            config.PlayerServerConfig = await Core.Repository.ConfigRepository.QueryPlayerServerConfig(playerServerCategory);
            if(config.PlayerServerConfig == null)
            {
                Console.WriteLine($"Core Load Fail! Can not Find {playerServerCategory}");
                return;
            }
            else Core.Config = config;
            Console.WriteLine("Core Load Success!");
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
                    skillCard.AuxiliaryHp = random.Next() % 100;
                    skillCard.AuxiliaryMp = random.Next() % 100;
                    skillCard.Description = "这是一段描述";
                    skillCard.EnemyHp = random.Next() % 100;
                    skillCard.EnemyMp = random.Next() % 100;
                    skillCard.MaxAuxiliary = random.Next() % 5;
                    skillCard.MaxEnemy = random.Next() % 5;
                    skillCard.Mp = random.Next() % 100;
                    skillCard.Probability = random.Next() % 100;
                    skillCard.RegisterDate = random.Next();
                    skillCard.AttributeUpdate = timeStamp;
                    for (int j = 0; j < random.Next() % 5; j++)
                    {
                        Buff buff = new Buff();
                        buff.Power = random.Next() % 100;
                        buff.Category = Buff.BuffCategory.Freeze;
                        buff.Duration = random.Next() % 100;
                        skillCard.AuxiliaryBuff.Add(buff);
                    }
                    for (int j = 0; j < random.Next() % 5; j++)
                    {
                        Buff buff = new Buff();
                        buff.Power = random.Next() % 100;
                        buff.Category = Buff.BuffCategory.Freeze;
                        buff.Duration = random.Next() % 100;
                        skillCard.EnemyBuff.Add(buff);
                    }
                    if (random.Next() % 2 == 1) skillCard.Category.Add(SkillCard.SkillCardCategory.Attack);
                    if (random.Next() % 2 == 1) skillCard.Category.Add(SkillCard.SkillCardCategory.Cure);
                    if (random.Next() % 2 == 1) skillCard.Category.Add(SkillCard.SkillCardCategory.Eternal);
                    if (random.Next() % 2 == 1) skillCard.Category.Add(SkillCard.SkillCardCategory.Magic);
                    if (random.Next() % 2 == 1) skillCard.Category.Add(SkillCard.SkillCardCategory.Physics);
                    long result = await Core.Repository.SkillCardRepository.Insert(skillCard);
                    if (result == -1)
                    {
                        Console.WriteLine("Core Load Fail!");
                        break;
                    }
                }
            }
            else
            {
                List<SkillCard> skillCards = await Core.Repository.SkillCardRepository.Query_All();
                foreach (SkillCard item in skillCards)
                {
                    Core.SkillCardByID.Add(item.Id, item);
                }
            }
            Console.WriteLine("SkillCard Load Success!");
        }
        public void AdventuresInit()
        {

        }
    }
}
