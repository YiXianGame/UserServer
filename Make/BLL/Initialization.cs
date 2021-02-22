using Make.Model;
using Make.RPCClient.Request;
using Make.RPCServer.Adapt;
using Make.RPCServer.Request;
using Material.Entity;
using Material.Entity.Config;
using Material.Entity.Game;
using Material.MySQL;
using Material.Redis;
using Material.RPCServer;
using Material.RPCServer.TCP_Async_Event;
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
            CoreInit(UserServerConfig.UserServerCategory.StandardUserServer);
            Random random = new Random();

            for (int i = 0; i < 1000000; i++)
            {
                List<BaseUserToken> users = new List<BaseUserToken>();
                for (int j = 0; j < random.Next(1,5); j++)
                {
                    UserToken user = new UserToken();
                    users.Add(user);
                }
                Team team = new Team(users, random.Next(1, 9));
                Core.SoloMatchSystem.Add(team);
            }
            Core.SoloMatchSystem.MatchSucessEvent += SoloMatchSystem_MatchSucessEvent;
            Core.SoloGroupMatchSystem.MatchSucessEvent += SoloGroupMatchSystem_MatchSucessEvent;
            Core.SoloMatchSystem.Start();
            #region --RPCServer--
            Material.RPCServer.RPCType serverType = new Material.RPCServer.RPCType();
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
            Material.RPCServer.RPCAdaptFactory.Register<UserAdapt>("UserServer", "192.168.0.105", "28015", serverType);
            Material.RPCServer.RPCAdaptFactory.Register<SkillCardAdapt>("SkillCardServer", "192.168.0.105", "28015", serverType);
            //注册Server远程服务
            Core.UserRequest = Material.RPCServer.RPCRequestProxyFactory.Register<UserRequest>("UserClient", "192.168.0.105", "28015", serverType);
            Core.SkillCardRequest = Material.RPCServer.RPCRequestProxyFactory.Register<SkillCardRequest>("SkillCardClient", "192.168.0.105", "28015", serverType);
            //启动Server服务
            RPCNetServerFactory.StartServer("192.168.0.105", "28015", () => new UserToken());
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

            SkillCardInit();
            AdventuresInit();
            Console.WriteLine("Initialization Sucess!");
        }

        private void SoloGroupMatchSystem_MatchSucessEvent(List<TeamGroup<TeamGroup<Team>>> teamGroups)
        {
            int a = 2;
        }

        private void SoloMatchSystem_MatchSucessEvent(List<TeamGroup<Team>> teamGroups)
        {
            Console.WriteLine("组队成功，开始进行队伍对抗配对！");
            teamGroups.ForEach((value) => { value.SumRank = value.AverageRank; value.Count = 1; });
            Core.SoloGroupMatchSystem.Add(teamGroups);
            Core.SoloGroupMatchSystem.Start();
        }

        private async void CoreInit(UserServerConfig.UserServerCategory category)
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
                Core.Config.PlayerServerConfig = new PlayerServerConfig();
                Core.Config.PlayerServerConfig.Category = PlayerServerConfig.PlayerServerCategory.StandardPlayerServer;
                if (!(await Core.Repository.ConfigRepository.Insert(Core.Config)))
                {
                    Console.WriteLine("Core Load Fail!");
                }
                else
                {
                    config = await Core.Repository.ConfigRepository.Query(category);
                    Core.Config = config;
                }
            }
            else Core.Config = config;
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
            Console.WriteLine("SkillCard Load Sucess!");
        }
        public void AdventuresInit()
        {

        }
    }
}
