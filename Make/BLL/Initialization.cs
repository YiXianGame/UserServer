using Make.RPCClient.Request;
using Make.RPCServer.Request;
using Make.RPCServer.Service;
using Material.Entity;
using Material.Entity.Config;
using Material.MySQL;
using Material.Redis;
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
            CoreInit(UserServerConfig.UserServerCategory.StandardServer, PlayerServerConfig.PlayerServerCategory.StandardServer);
            Core.SoloMatchSystem.MatchPipelineOut += Core.SoloGroupMatchSystem.PiplineIn;
            Core.SoloGroupMatchSystem.MatchSuccessEvent += MatchSystemHelper.SoloGroupMatchSystem_MatchSuccessEvent;

            #region --RPCClient--
            if (Core.Config.PlayerServerConfig != null)
            {
                EtherealC.Model.RPCTypeConfig clientType = new EtherealC.Model.RPCTypeConfig();
                clientType.Add<int>("Int");
                clientType.Add<string>("String");
                clientType.Add<bool>("Bool");
                clientType.Add<long>("Long");
                clientType.Add<User>("User");
                clientType.Add<CardGroup>("CardGroup");
                clientType.Add<SkillCard>("SkillCard");
                clientType.Add<List<long>>("List<Long>");
                clientType.Add<List<SkillCard>>("List<SkillCard>");
                clientType.Add<List<CardItem>>("List<CardItem>");
                clientType.Add<List<CardGroup>>("List<CardGroup>");
                clientType.Add<List<Friend>>("List<Friend>");
                clientType.Add<List<User>>("List<User>");
                clientType.Add<List<Team>>("List<Team>");
                EtherealC.RPCNet.NetCore.Register(Core.Config.PlayerServerConfig.Ip, Core.Config.PlayerServerConfig.Port);
                //注册Client远程服务
                Core.PlayerServerRequest = EtherealC.RPCRequest.RequestCore.Register<PlayerServerRequest>(Core.Config.PlayerServerConfig.Ip, Core.Config.PlayerServerConfig.Port, "PlayerServer", clientType);
                //启动Client服务
                EtherealC.NativeClient.ClientCore.Register(Core.Config.PlayerServerConfig.Ip, Core.Config.PlayerServerConfig.Port).Start();
                PlayerServerLogin();    
            }
            #endregion

            #region --RPCServer--
            EtherealS.Model.RPCTypeConfig serverType = new EtherealS.Model.RPCTypeConfig();
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
            EtherealS.RPCService.ServiceConfig serverServiceConfig = new EtherealS.RPCService.ServiceConfig(serverType);
            EtherealS.RPCRequest.RequestConfig serverRequestConfig = new EtherealS.RPCRequest.RequestConfig(serverType);
            //适配Server远程客户端服务
            serverServiceConfig.InterceptorEvent += EtherealS.Extension.Authority.AuthorityCheck.ServiceCheck;
            EtherealS.RPCService.ServiceCore.Register<UserService>(Core.Config.Ip,Core.Config.Port, "UserServer", serverServiceConfig);
            EtherealS.RPCService.ServiceCore.Register<SkillCardService>( Core.Config.Ip, Core.Config.Port, "SkillCardServer", serverServiceConfig);
            EtherealS.RPCService.ServiceCore.Register<ReadyService>(Core.Config.Ip, Core.Config.Port, "ReadyServer", serverServiceConfig);
            EtherealS.RPCService.ServiceCore.Register<EquipService>(Core.Config.Ip, Core.Config.Port, "EquipServer", serverServiceConfig);
            //注册Server远程服务
            Core.UserRequest = EtherealS.RPCRequest.RequestCore.Register<UserRequest>( Core.Config.Ip, Core.Config.Port, "UserClient", serverRequestConfig);
            Core.SkillCardRequest = EtherealS.RPCRequest.RequestCore.Register<SkillCardRequest>(Core.Config.Ip, Core.Config.Port, "SkillCardClient", serverRequestConfig);
            Core.ReadyRequest = EtherealS.RPCRequest.RequestCore.Register<ReadyRequest>( Core.Config.Ip, Core.Config.Port, "ReadyClient", serverRequestConfig);
            Core.EquipRequest = EtherealS.RPCRequest.RequestCore.Register<EquipRequest>( Core.Config.Ip, Core.Config.Port, "EquipClient", serverRequestConfig);
            EtherealS.RPCNet.NetCore.Register(Core.Config.Ip, Core.Config.Port);
            EtherealS.NativeServer.ServerConfig serverNetConfig = new EtherealS.NativeServer.ServerConfig(() => new User());
            EtherealS.NativeServer.ServerCore.Register(Core.Config.Ip, Core.Config.Port,serverNetConfig).Start(); 
            #endregion

            Core.SoloMatchSystem.StartPolling(0, 5000);
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
                int buffCategory = 0;
                int skillCardCategory = 0;
                for (int i = 1; i <= 75; i++)
                {
                    SkillCard skillCard = new SkillCard();
                    skillCard.Name = $"第{i}张卡牌";
                    skillCard.AuthorId = 839336369;
                    skillCard.AuxiliaryHp = random.Next() % 100;
                    skillCard.AuxiliaryMp = random.Next() % 100;
                    skillCard.Description = "这是一段描述";
                    skillCard.EnemyHp = random.Next() % 100;
                    skillCard.EnemyMp = random.Next() % 100;
                    skillCard.MaxAuxiliary = random.Next() % 5 + 1;
                    skillCard.MaxEnemy = random.Next() % 5 + 1;
                    skillCard.Mp = random.Next() % 100;
                    skillCard.Probability = random.Next() % 100;
                    skillCard.RegisterDate = random.Next();
                    skillCard.AttributeUpdate = timeStamp;
                    Buff auxiliary_buff = new Buff();
                    auxiliary_buff.Power = random.Next() % 100;
                    auxiliary_buff.Category = (Buff.BuffCategory)buffCategory;
                    auxiliary_buff.Duration = random.Next() % 10000;
                    skillCard.AuxiliaryBuff.Add(auxiliary_buff);
                    Buff enemies_buff = new Buff();
                    enemies_buff.Power = random.Next() % 100;
                    enemies_buff.Category = (Buff.BuffCategory)buffCategory;
                    enemies_buff.Duration = random.Next() % 10000;
                    skillCard.EnemyBuff.Add(enemies_buff);
                    skillCard.Category = (SkillCard.SkillCardCategory)(skillCardCategory);
                    long result = await Core.Repository.SkillCardRepository.Insert(skillCard);
                    if (result == -1)
                    {
                        Console.WriteLine("Core Load Fail!");
                        break;
                    }
                    if (i % 15 == 0) skillCardCategory++;
                    buffCategory = (buffCategory + 1) % 15;
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
