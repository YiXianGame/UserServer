using Make.RPCClient.Request;
using Make.RPCServer.Request;
using Make.RPCServer.Service;
using Material.Entity;
using Material.Entity.Config;
using Material.Entity.Match;
using Material.EtherealS.Annotation;
using Material.EtherealS.Extension.Authority;
using Material.EtherealS.Model;
using Material.EtherealS.Net;
using Material.EtherealS.Request;
using Material.EtherealS.Service;
using Material.MySQL;
using Material.Redis;
using System;
using System.Collections.Generic;
using System.Reflection;
using Material.EtherealC.Request;

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
            CoreInit(UserServerConfig.UserServerCategory.StandardServer, PlayerServerConfig.PlayerServerCategory.StandardServer);
            Core.SoloMatchSystem.MatchPipelineOut += Core.SoloGroupMatchSystem.PiplineIn;
            Core.SoloGroupMatchSystem.MatchSuccessEvent += MatchSystemHelper.SoloGroupMatchSystem_MatchSuccessEvent;

            #region --RPCClient--
            if (Core.Config.PlayerServerConfig != null)
            {
                Material.EtherealC.Model.RPCType clientType = new Material.EtherealC.Model.RPCType();
                clientType.Add<int>("Int");
                clientType.Add<string>("String");
                clientType.Add<bool>("Bool");
                clientType.Add<long>("Long");
                clientType.Add<User>("User");
                clientType.Add<CardGroup>("CardGroup");
                clientType.Add<SkillCard>("SkillCard");
                clientType.Add<List<long>>("List<long>");
                clientType.Add<List<SkillCard>>("List<SkillCard>");
                clientType.Add<List<CardItem>>("List<CardItem>");
                clientType.Add<List<CardGroup>>("List<CardGroup>");
                clientType.Add<List<Friend>>("List<Friend>");
                clientType.Add<List<User>>("List<User>");
                clientType.Add<List<Team>>("List<Team>");
                //注册Client远程服务
                Core.PlayerServerRequest = RPCRequestFactory.Register<PlayerServerRequest>("PlayerServer", Core.Config.PlayerServerConfig.Ip, Core.Config.PlayerServerConfig.Port, new RPCRequestConfig(clientType));
                //启动Client服务
                Material.EtherealC.Net.RPCNetFactory.StartClient(Core.Config.PlayerServerConfig.Ip, Core.Config.PlayerServerConfig.Port, new Material.EtherealC.Net.RPCNetConfig(1024));
                PlayerServerLogin();    
            }
            #endregion

            #region --RPCServer--
            RPCType serverType = new RPCType();
            serverType.Add<int>("Int");
            serverType.Add<string>("String");
            serverType.Add<bool>("Bool");
            serverType.Add<long>("Long");
            serverType.Add<User>("User");
            serverType.Add<CardGroup>("CardGroup");
            serverType.Add<SkillCard>("SkillCard");
            serverType.Add<List<long>>("List<long>");
            serverType.Add<List<SkillCard>>("List<SkillCard>");
            serverType.Add<List<CardItem>>("List<CardItem>");
            serverType.Add<List<CardGroup>>("List<CardGroup>");
            serverType.Add<List<Friend>>("List<Friend>");
            serverType.Add<List<User>>("List<User>");
            serverType.Add<List<Team>>("List<Team>");
            RPCNetServiceConfig serverServiceConfig = new RPCNetServiceConfig(serverType);
            RPCNetRequestConfig serverRequestConfig = new RPCNetRequestConfig(serverType);
            serverServiceConfig.Authoritable = true;
            //适配Server远程客户端服务
            RPCServiceFactory.Register<UserService>("UserServer", "192.168.80.1", "28015", serverServiceConfig);
            RPCServiceFactory.Register<SkillCardService>("SkillCardServer", "192.168.80.1", "28015", serverServiceConfig);
            RPCServiceFactory.Register<ReadyService>("ReadyServer", "192.168.80.1", "28015", serverServiceConfig);
            //注册Server远程服务
            Core.UserRequest = RPCNetRequestFactory.Register<UserRequest>("UserClient", "192.168.80.1", "28015", serverRequestConfig);
            Core.SkillCardRequest = RPCNetRequestFactory.Register<SkillCardRequest>("SkillCardClient", "192.168.80.1", "28015", serverRequestConfig);
            Core.ReadyRequest = RPCNetRequestFactory.Register<ReadyRequest>("ReadyClient", "192.168.80.1", "28015", serverRequestConfig);
            //启动Server服务
            RPCNetConfig serverNetConfig = new RPCNetConfig(() => new User());
            RPCNetFactory.StartServer("192.168.80.1", "28015", serverNetConfig);
            serverNetConfig.InterceptorEvent += OnAuthorityCheck;
            #endregion

            Core.SoloMatchSystem.StartPolling(0, 5000);
            SkillCardInit();
            AdventuresInit();
            Console.WriteLine("Initialization Success!");
        }

        private bool OnAuthorityCheck(RPCNetService service, MethodInfo method, BaseUserToken token)
        {
            RPCService annotation = method.GetCustomAttribute<RPCService>();
            if (annotation.Authority != null)
            {
                if ((token as IAuthorityCheck).Check(annotation))
                {
                    return true;
                }
                else return false;
            }
            else if (service.Config.Authoritable)
            {
                if ((token as IAuthorityCheck).Check((IAuthoritable)service))
                {
                    return true;
                }
                else return false;
            }
            else return true;
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
            Console.WriteLine($"Core-PlayerServerConfig-{playerServerCategory} Loading....");
            config.PlayerServerConfig = await Core.Repository.ConfigRepository.QueryPlayerServerConfig(playerServerCategory);
            if (config.PlayerServerConfig == null)
            {
                Console.WriteLine($"Core Load Fail! Can not Find {playerServerCategory}");
                return;
            }
            else Core.Config = config;
            Console.WriteLine($"Core-PlayerServerConfig-{playerServerCategory} Load Success....");
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
                    Core.SkillCards.Add(item.Id, item);
                }
            }
            Console.WriteLine("SkillCard Load Success!");
        }
        public void AdventuresInit()
        {

        }
        public void PlayerServerLogin()
        {
            Console.WriteLine("PlayerServer Login....");
            if (Core.PlayerServerRequest.Login(Core.Config.PlayerServerConfig.SecretKey))
            {
                Console.WriteLine("PlayerServer Login Success!");
            }
            else Console.WriteLine("PlayerServer Login Fail!");
        }
    }
}
